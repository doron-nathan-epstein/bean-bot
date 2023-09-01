using Discord;

namespace BeanBot.Application.Audio
{
  public interface IAudioService
  {
    public Task JoinAudioAsync(IGuild server, IVoiceChannel voiceChannel, ITextChannel textChannel);
    public Task AddQueueAsync(IGuild server, string message, AudioType type);
    public Task SkipAudioAsync(IGuild server, ITextChannel textChannel);
    public Task LeaveAudioAsync(IGuild server);
  }
}
