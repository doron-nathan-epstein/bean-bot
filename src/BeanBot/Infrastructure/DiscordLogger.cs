using BeanBot.Application.Common;
using Discord;
using Microsoft.Extensions.Logging;

namespace BeanBot.Infrastructure
{
  internal class DiscordLogger : IDiscordLogger
  {
    private readonly ILogger<DiscordLogger> _logger;

    public DiscordLogger(ILogger<DiscordLogger> logger)
    {
      _logger = logger;
    }

    public Task OnLogAsync(LogMessage message)
    {
      switch (message.Severity)
      {
        case LogSeverity.Verbose:
          _logger.LogInformation(message.Exception, message.Message, null);
          break;

        case LogSeverity.Info:
          _logger.LogInformation(message.Exception, message.Message, null);
          break;

        case LogSeverity.Warning:
          _logger.LogWarning(message.Exception, message.Message, null);
          break;

        case LogSeverity.Error:
          _logger.LogError(message.Exception, message.Message, null);
          break;

        case LogSeverity.Critical:
          _logger.LogCritical(message.Exception, message.Message, null);
          break;
      }

      return Task.CompletedTask;
    }
  }
}
