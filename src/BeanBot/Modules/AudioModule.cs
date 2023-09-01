using Discord;
using Discord.Interactions;
using System.Text.RegularExpressions;

namespace BeanBot.Modules
{
  public sealed class AudioModule : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand("join", "Make the bot join a channel.", true, RunMode.Async)]
    public async Task JoinAsync(IVoiceChannel channel = null)
    {
      channel ??= (Context.User as IGuildUser)?.VoiceChannel;
      if (channel == null)
        await ReplyAsync("You are not connected to a voice channel for me to join!");
      else
        await AudioService.JoinAudio(Context.Guild, channel, Context.Channel as ITextChannel);
    }

    [SlashCommand("leave", "Make the bot leave the channel.", true, RunMode.Async)]
    public async Task LeaveAsync() => await AudioService.LeaveAudio(Context.Guild);

    [SlashCommand("skip", "Make the bot skip the current audio.", true, RunMode.Async)]
    public async Task SkipAsync() => await AudioService.SkipAudio(Context.Guild, Context.Channel as ITextChannel);


    [SlashCommand("tts", "Make the bot say something.", true, RunMode.Async)]
    public async Task TTSAsync(string textInput)
    {
      string message = Regex.Replace(textInput, @"@\\w[a-zA-Z0-9()]{0,75}#[0-9]{0,4}", "").Trim().ToLower();
      if (message.Length < 1)
      {
        await ReplyAsync("The message you entered is invalid");
        return;
      }

      await AudioService.AddQueue(Context.Guild, message, AudioService.AudioQueue.AudioType.TTS);
    }
  }
}

