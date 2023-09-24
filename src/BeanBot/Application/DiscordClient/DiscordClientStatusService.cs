using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BeanBot.Application.DiscordClient
{
  internal sealed class DiscordClientStatusService : IDiscordClientStatusService
  {
    private readonly DiscordSocketClient _discordClient;
    private readonly ILogger<DiscordClientStatusService> _logger;

    public DiscordClientStatusService(DiscordSocketClient discordClient, ILogger<DiscordClientStatusService> logger)
    {
      _discordClient = discordClient;
      _logger = logger;
    }

    public async Task SetStatusAsync()
    {
      _logger.LogInformation("Bot: Set Status.");

      await _discordClient.SetCustomStatusAsync($"Currently serving {_discordClient.Guilds.Count} servers!").ConfigureAwait(false);
    }
  }
}
