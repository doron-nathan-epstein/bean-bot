namespace BeanBot.Application.Notifications;

using Discord.Interactions;
using MediatR;

internal class InteractionCreatedNotification : INotification
{
  public InteractionCreatedNotification(SocketInteractionContext context)
  {
    Context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public SocketInteractionContext Context { get; }
}
