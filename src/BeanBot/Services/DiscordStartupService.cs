using BeanBot.Application.Common;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BeanBot.Services
{
  internal sealed class DiscordStartupService : IHostedService
  {
    private readonly DiscordSocketClient _discordClient;
    private readonly IConfiguration _config;
    private readonly IDiscordLogger _logger;

    public DiscordStartupService(DiscordSocketClient discord, IConfiguration config, IDiscordLogger logger)
    {
      _discordClient = discord;
      _config = config;
      _logger = logger;

      _discordClient.Log += msg => _logger.OnLogAsync(msg);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      await _discordClient.LoginAsync(TokenType.Bot, _config["DISCORD_TOKEN"]);
      await _discordClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      await _discordClient.LogoutAsync();
      await _discordClient.StopAsync();
    }
  }
}
