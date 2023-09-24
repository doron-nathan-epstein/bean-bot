namespace BeanBot.HostedServices;

using BeanBot.Application.Common;
using BeanBot.Application.DiscordClient;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

internal sealed class DiscordClientService : IHostedService
{
  private readonly DiscordSocketClient _discordClient;
  private readonly InteractionService _interactionService;
  private readonly IServiceProvider _serviceProvider;
  private readonly IConfiguration _config;
  private readonly ILogger<DiscordClientService> _logger;
  private readonly IDiscordClientStatusService _statusService;

  public DiscordClientService(DiscordSocketClient discord,
    InteractionService interactionService,
    IServiceProvider serviceProvider,
    IConfiguration config,
    ILogger<DiscordClientService> logger,
    IDiscordLogger discordLogger,
    IDiscordClientStatusService statusService)
  {
    _discordClient = discord;
    _interactionService = interactionService;
    _serviceProvider = serviceProvider;
    _config = config;
    _logger = logger;
    _statusService = statusService;
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
  private async Task OnReadyAsync()
  {
    _logger.LogInformation("Interaction Service: Register Commands Gloablly.");
    await _interactionService.RegisterCommandsGloballyAsync(true).ConfigureAwait(false);

    await _statusService.SetStatusAsync();
  }

  private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
  {
    try
    {
      var context = new SocketInteractionContext(_discordClient, interaction);
      var result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider).ConfigureAwait(false);

      if (!result.IsSuccess)
      {
        await context.Channel.SendMessageAsync(result.ToString()).ConfigureAwait(false);
      }
    }
    catch
    {
      if (interaction.Type == InteractionType.ApplicationCommand)
      {
        await interaction.GetOriginalResponseAsync()
            .ContinueWith(msg => msg.Result.DeleteAsync()).ConfigureAwait(false);
      }
    }
  }

  private async Task OnJoinedGuildAsync(SocketGuild arg)
  {
    await _statusService.SetStatusAsync();
  }

  private async Task OnLeftGuildAsync(SocketGuild arg)
  {
    await _statusService.SetStatusAsync();
  }
  #endregion
}

