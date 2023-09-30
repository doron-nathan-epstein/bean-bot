namespace BeanBot.Application.Notifications;

using Discord.WebSocket;
using MediatR;

internal class LeftGuildNotification : INotification
{
  public LeftGuildNotification(SocketGuild guild)
  {
    Guild = guild ?? throw new ArgumentNullException(nameof(guild));
  }

  public SocketGuild Guild { get; }
}
