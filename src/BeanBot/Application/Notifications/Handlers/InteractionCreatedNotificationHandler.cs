namespace BeanBot.Application.Notifications.Handlers;

using Discord;
using Discord.Interactions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

internal class InteractionCreatedNotificationHandler : INotificationHandler<InteractionCreatedNotification>
{
  private readonly InteractionService _interactionService;
  private readonly IServiceProvider _serviceProvider;
  private readonly ILogger<InteractionCreatedNotificationHandler> _logger;

  public InteractionCreatedNotificationHandler(InteractionService interactionService, IServiceProvider serviceProvider, ILogger<InteractionCreatedNotificationHandler> logger)
  {
    _interactionService = interactionService;
    _serviceProvider = serviceProvider;
    _logger = logger;
  }

  public async Task Handle(InteractionCreatedNotification notification, CancellationToken cancellationToken)
  {
    try
    {
      var result = await _interactionService.ExecuteCommandAsync(notification.Context, _serviceProvider).ConfigureAwait(false);

      if (!result.IsSuccess)
      {
        await notification.Context.Channel.SendMessageAsync(result.ToString()).ConfigureAwait(false);
      }
    }
    catch
    {
      if (notification.Context.Interaction.Type == InteractionType.ApplicationCommand)
      {
        await notification.Context.Interaction.GetOriginalResponseAsync()
            .ContinueWith(msg => msg.Result.DeleteAsync()).ConfigureAwait(false);
      }
    }
  }
}
