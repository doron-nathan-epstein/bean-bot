using BeanBot.Utility;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BeanBot.Services
{
  internal sealed class DiscordStartupService : IHostedService
  {
    private readonly DiscordSocketClient _discordClient;
    private readonly IConfiguration _config;
    private readonly ILogger<DiscordSocketClient> _logger;

    public DiscordStartupService(DiscordSocketClient discord, IConfiguration config, ILogger<DiscordSocketClient> logger)
    {
      _discordClient = discord;
      _config = config;
      _logger = logger;

      _discordClient.Log += msg => LogHelper.OnLogAsync(_logger, msg);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      await _discordClient.LoginAsync(TokenType.Bot, _config["DiscordToken"]);
      await _discordClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      await _discordClient.LogoutAsync();
      await _discordClient.StopAsync();
    }
  }
}
