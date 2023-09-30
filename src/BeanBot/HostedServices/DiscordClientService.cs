namespace BeanBot.HostedServices;

using BeanBot.Application.Common;
using BeanBot.Application.Notifications;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading;

internal sealed class DiscordClientService : IHostedService
{
  private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

  private readonly DiscordSocketClient _discordClient;
  private readonly InteractionService _interactionService;
  private readonly IServiceProvider _serviceProvider;
  private readonly IConfiguration _config;
  private readonly ILogger<DiscordClientService> _logger;
  private readonly IMediator _mediator;

  public DiscordClientService(DiscordSocketClient discord,
    InteractionService interactionService,
    IServiceProvider serviceProvider,
    IConfiguration config,
    ILogger<DiscordClientService> logger,
    IMediator mediator,
    IDiscordLogger discordLogger)
  {
    _discordClient = discord;
    _interactionService = interactionService;
    _serviceProvider = serviceProvider;
    _config = config;
    _logger = logger;
    _mediator = mediator;

    _discordClient.Log += msg => discordLogger.OnLogAsync(msg);
    _interactionService.Log += msg => discordLogger.OnLogAsync(msg);
  }

  #region IHostedService
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Discord Client: Configure Events.");
    _discordClient.Ready += OnReadyAsync;
    _discordClient.InteractionCreated += OnInteractionCreatedAsync;
    _discordClient.JoinedGuild += OnJoinedGuildAsync;
    _discordClient.LeftGuild += OnLeftGuildAsync;

    _logger.LogInformation("Interaction Service: Configure Dependency Injection.");
    _ = await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider).ConfigureAwait(false);

    _logger.LogInformation("Discord Client: Perform Login.");
    await _discordClient.LoginAsync(TokenType.Bot, _config["DISCORD_TOKEN"]).ConfigureAwait(false);

    _logger.LogInformation("Discord Client: Start.");
    await _discordClient.StartAsync().ConfigureAwait(false);
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Discord Client: Unconfigure Events.");
    _discordClient.Ready -= OnReadyAsync;
    _discordClient.InteractionCreated -= OnInteractionCreatedAsync;
    _discordClient.JoinedGuild -= OnJoinedGuildAsync;
    _discordClient.LeftGuild -= OnLeftGuildAsync;

    _logger.LogInformation("Discord Client: Perform Logout.");
    await _discordClient.LogoutAsync().ConfigureAwait(false);

    _logger.LogInformation("Discord Client: Stop.");
    await _discordClient.StopAsync().ConfigureAwait(false);
  }
  #endregion

  #region DiscordSocketClient Events
  private async Task OnReadyAsync() => await _mediator.Publish(new ReadyNotification(_discordClient), _cancellationToken).ConfigureAwait(false);
  private async Task OnInteractionCreatedAsync(SocketInteraction interaction) => await _mediator.Publish(new InteractionCreatedNotification(new SocketInteractionContext(_discordClient, interaction)), _cancellationToken).ConfigureAwait(false);
  private async Task OnJoinedGuildAsync(SocketGuild guild) => await _mediator.Publish(new JoinedGuildNotification(guild), _cancellationToken).ConfigureAwait(false);
  private async Task OnLeftGuildAsync(SocketGuild guild) => await _mediator.Publish(new LeftGuildNotification(guild), _cancellationToken).ConfigureAwait(false);
  #endregion
}

