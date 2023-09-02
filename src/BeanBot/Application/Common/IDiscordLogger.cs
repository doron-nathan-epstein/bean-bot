using Discord;

namespace BeanBot.Application.Common
{
  public interface IDiscordLogger
  {
    public Task OnLogAsync(LogMessage message);
  }
}
