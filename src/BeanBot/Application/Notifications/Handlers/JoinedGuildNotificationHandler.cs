namespace BeanBot.Application.Notifications.Handlers;

using BeanBot.Application.DiscordClient;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

internal class JoinedGuildNotificationHandler : INotificationHandler<JoinedGuildNotification>
{
  private readonly ILogger<JoinedGuildNotificationHandler> _logger;
  private readonly IDiscordClientStatusService _statusService;

  public JoinedGuildNotificationHandler(ILogger<JoinedGuildNotificationHandler> logger, IDiscordClientStatusService statusService)
  {
    _logger = logger;
    _statusService = statusService;
  }

  public async Task Handle(JoinedGuildNotification notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Bot: Joined {id}:{name}.", notification.Guild.Id, notification.Guild.Name);

    await _statusService.SetStatusAsync();
  }
}
