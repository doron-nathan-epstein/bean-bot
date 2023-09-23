namespace BeanBot.Application.Common;

using Discord;

public interface IDiscordLogger
{
  public Task OnLogAsync(LogMessage message);
}
