using BeanBot.Application.Common;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace BeanBot.Services
{
  internal sealed class InteractionHandlingService : IHostedService
  {
    private readonly DiscordSocketClient _discordClient;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly IDiscordLogger _logger;

    public InteractionHandlingService(
        DiscordSocketClient discord,
        InteractionService interactions,
        IServiceProvider services,
        IDiscordLogger logger)
    {
      _discordClient = discord;
      _interactions = interactions;
      _services = services;
      _logger = logger;

      _interactions.Log += msg => _logger.OnLogAsync(msg);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      _discordClient.Ready += () => _interactions.RegisterCommandsGloballyAsync(true);
      _discordClient.InteractionCreated += OnInteractionAsync;

      await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _interactions.Dispose();
      return Task.CompletedTask;
    }

    private async Task OnInteractionAsync(SocketInteraction interaction)
    {
      try
      {
        var context = new SocketInteractionContext(_discordClient, interaction);
        var result = await _interactions.ExecuteCommandAsync(context, _services);

        if (!result.IsSuccess)
          await context.Channel.SendMessageAsync(result.ToString());
      }
      catch
      {
        if (interaction.Type == InteractionType.ApplicationCommand)
        {
          await interaction.GetOriginalResponseAsync()
              .ContinueWith(msg => msg.Result.DeleteAsync());
        }
      }
    }
  }
}
