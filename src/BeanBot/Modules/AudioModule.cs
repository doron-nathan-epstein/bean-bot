using BeanBot.Application.Audio;
using Discord;
using Discord.Interactions;
using System.Text.RegularExpressions;

namespace BeanBot.Modules
{
  public sealed class AudioModule : InteractionModuleBase<SocketInteractionContext>
  {
    private readonly IAudioService _audioService;

    public AudioModule(IAudioService audioService)
    {
      _audioService = audioService;
    }

    [SlashCommand("join", "Make the bot join a channel.", true, RunMode.Async)]
    public async Task JoinAsync(IVoiceChannel channel = null)
    {
      channel ??= (Context.User as IGuildUser)?.VoiceChannel;
      if (channel == null)
        await ReplyAsync("You are not connected to a voice channel for me to join!");
      else
        await _audioService.JoinAudioAsync(Context.Guild, channel, Context.Channel as ITextChannel);
    }

    [SlashCommand("leave", "Make the bot leave the channel.", true, RunMode.Async)]
    public async Task LeaveAsync() => await _audioService.LeaveAudioAsync(Context.Guild);

    [SlashCommand("skip", "Make the bot skip the current audio.", true, RunMode.Async)]
    public async Task SkipAsync() => await _audioService.SkipAudioAsync(Context.Guild, Context.Channel as ITextChannel);

    [SlashCommand("tts", "Make the bot say something.", true, RunMode.Async)]
    public async Task TTSAsync(string message)
    {
      message = Regex.Replace(message, @"@\\w[a-zA-Z0-9()]{0,75}#[0-9]{0,4}", "").Trim().ToLower();
      if (message.Length < 1)
      {
        await ReplyAsync("The message you entered is invalid");
        return;
      }

      await _audioService.AddQueueAsync(Context.Guild, message, AudioType.TTS);
    }
  }
}

