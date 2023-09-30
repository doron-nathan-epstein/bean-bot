namespace BeanBot.Application.Notifications.Handlers;

using BeanBot.Application.DiscordClient;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

internal class LeftGuildNotificationHandler : INotificationHandler<LeftGuildNotification>
{
  private readonly ILogger<LeftGuildNotificationHandler> _logger;
  private readonly IDiscordClientStatusService _statusService;

  public LeftGuildNotificationHandler(ILogger<LeftGuildNotificationHandler> logger, IDiscordClientStatusService statusService)
  {
    _logger = logger;
    _statusService = statusService;
  }

  public async Task Handle(LeftGuildNotification notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Bot: Left {id}:{name}.", notification.Guild.Id, notification.Guild.Name);

    await _statusService.SetStatusAsync();
  }
}
