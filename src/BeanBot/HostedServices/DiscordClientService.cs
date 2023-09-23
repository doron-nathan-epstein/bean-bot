namespace BeanBot.HostedServices;

using BeanBot.Application.Common;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

internal sealed class DiscordClientService : IHostedService
{
  private readonly DiscordSocketClient _discordClient;
  private readonly IConfiguration _config;
  private readonly IDiscordLogger _logger;

  public DiscordClientService(DiscordSocketClient discord, IConfiguration config, IDiscordLogger logger)
  {
    _discordClient = discord;
    _config = config;
    _logger = logger;

    _discordClient.Log += msg => _logger.OnLogAsync(msg);
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await _discordClient.LoginAsync(TokenType.Bot, _config["DISCORD_TOKEN"]).ConfigureAwait(false);
    await _discordClient.StartAsync().ConfigureAwait(false);
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    await _discordClient.LogoutAsync().ConfigureAwait(false);
    await _discordClient.StopAsync().ConfigureAwait(false);
  }
}

