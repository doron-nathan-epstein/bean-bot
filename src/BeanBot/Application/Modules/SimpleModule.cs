namespace BeanBot.Application.Modules;

using Discord.Interactions;

public sealed class SimpleModule : InteractionModuleBase<SocketInteractionContext>
{
  [SlashCommand("say", "Make the bot say something.", runMode: RunMode.Async)]
  public async Task Say(string text) => await ReplyAsync($"Say: {text}").ConfigureAwait(false);

  [SlashCommand("ping", "Make the bot respond with pong.", runMode: RunMode.Async)]
  public async Task Ping() => await ReplyAsync($"pong!").ConfigureAwait(false);
}
