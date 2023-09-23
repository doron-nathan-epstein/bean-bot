namespace BeanBot.Modules;

using Discord.Interactions;

public sealed class SimpleModule : InteractionModuleBase<SocketInteractionContext>
{
  [SlashCommand("say", "Make the bot say something.")]
  public async Task Say(string text) => await ReplyAsync(text).ConfigureAwait(false);
}
