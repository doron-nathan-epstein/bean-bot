using BeanBot.Observability;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BeanBot
{
  internal class BotServer
  {
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    internal BotServer()
    {
      _client = new DiscordSocketClient(new DiscordSocketConfig
      {
        LogLevel = LogSeverity.Info,
      });

      _commands = new CommandService(new CommandServiceConfig
      {
        LogLevel = LogSeverity.Info,
        CaseSensitiveCommands = false,
      });

      _client.Log += LoggingHelpers.Log;
      _commands.Log += LoggingHelpers.Log;

      _services = ConfigureServices();
    }

    // If any services require the client, or the CommandService, or something else you keep on hand,
    // pass them as parameters into this method as needed.
    // If this method is getting pretty long, you can seperate it out into another file using partials.
    private static IServiceProvider ConfigureServices()
    {
      var map = new ServiceCollection();

      return map.BuildServiceProvider();
    }

    private async Task InitCommands()
    {
      await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

      _client.MessageReceived += HandleCommandAsync;
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
      var msg = arg as SocketUserMessage;
      if (msg == null) return;

      if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

      int pos = 0;

      if (msg.HasCharPrefix('!', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
      {
        var context = new SocketCommandContext(_client, msg);

        var result = await _commands.ExecuteAsync(context, pos, _services);

        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
          await msg.Channel.SendMessageAsync(result.ErrorReason);
      }
    }

    public async Task StartAsync()
    {
      await InitCommands();

      await _client.LoginAsync(TokenType.Bot,
          Environment.GetEnvironmentVariable("DiscordToken"));
      await _client.StartAsync();

      await Task.Delay(Timeout.Infinite);
    }
  }
}
