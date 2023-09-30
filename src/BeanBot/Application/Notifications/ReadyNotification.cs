namespace BeanBot.Application.Notifications;

using Discord.WebSocket;
using MediatR;

internal class ReadyNotification : INotification
{
  public ReadyNotification(DiscordSocketClient client)
  {
    Client = client ?? throw new ArgumentNullException(nameof(client));
  }

  public DiscordSocketClient Client { get; }
}
