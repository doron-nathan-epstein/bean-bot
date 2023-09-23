namespace BeanBot.HostedServices;

using BeanBot.Application.Common;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using System.Reflection;

internal sealed class InteractionHandlingService : IHostedService
{
  private readonly DiscordSocketClient _discordClient;
  private readonly InteractionService _interactionService;
  private readonly IServiceProvider _serviceProvider;
  private readonly IDiscordLogger _logger;

  public InteractionHandlingService(
      DiscordSocketClient discord,
      InteractionService interactionService,
      IServiceProvider serviceProvider,
      IDiscordLogger logger)
  {
    _discordClient = discord;
    _interactionService = interactionService;
    _serviceProvider = serviceProvider;
    _logger = logger;

    _interactionService.Log += msg => _logger.OnLogAsync(msg);
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _ = await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider).ConfigureAwait(false);

    _discordClient.Ready += ClientReadyAsync;
    _discordClient.InteractionCreated += OnInteractionAsync;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _discordClient.Ready -= ClientReadyAsync;
    _discordClient.InteractionCreated -= OnInteractionAsync;
    
    _interactionService.Dispose();
    return Task.CompletedTask;
  }

  private async Task ClientReadyAsync()
  {
    await _interactionService.RegisterCommandsGloballyAsync(true).ConfigureAwait(false);
  }

  private async Task OnInteractionAsync(SocketInteraction interaction)
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
}

