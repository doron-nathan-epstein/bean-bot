namespace BeanBot.Application.Notifications;

using Discord.WebSocket;
using MediatR;

internal class JoinedGuildNotification : INotification
{
  public JoinedGuildNotification(SocketGuild guild)
  {
    Guild = guild ?? throw new ArgumentNullException(nameof(guild));
  }

  public SocketGuild Guild { get; }
}
