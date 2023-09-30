namespace BeanBot.Application.Notifications.Handlers;

using BeanBot.Application.DiscordClient;
using Discord.Interactions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

internal class ReadyNotificationHandler : INotificationHandler<ReadyNotification>
{
  private readonly InteractionService _interactionService;
  private readonly ILogger<ReadyNotificationHandler> _logger;
  private readonly IDiscordClientStatusService _statusService;

  public ReadyNotificationHandler(InteractionService interactionService, ILogger<ReadyNotificationHandler> logger, IDiscordClientStatusService statusService)
  {
    _interactionService = interactionService;
    _logger = logger;
    _statusService = statusService;
  }

  public async Task Handle(ReadyNotification notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Discord Client: Deregister Commands Globally.");
    var tasks = new List<Task>();
    tasks.AddRange(notification.Client.Guilds.Select(guild => guild.DeleteApplicationCommandsAsync()));
    Task.WaitAll(tasks.ToArray(), cancellationToken);

    _logger.LogInformation("Interaction Service: Register Commands Globally.");
    await _interactionService.RegisterCommandsGloballyAsync(true).ConfigureAwait(false);

    await _statusService.SetStatusAsync();
  }
}
