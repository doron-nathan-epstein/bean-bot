using Discord.Interactions;

namespace BeanBot.Modules
{
  public sealed class SimpleModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand("say", "Make the bot say something.")]
    public Task Say(string text)
        => ReplyAsync(text);
  }
}
