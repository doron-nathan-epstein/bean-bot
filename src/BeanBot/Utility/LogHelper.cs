using Discord;
using Microsoft.Extensions.Logging;

namespace BeanBot.Utility
{
  public static class LogHelper
  {
    public static Task OnLogAsync(ILogger logger, LogMessage message)
    {
      switch (message.Severity)
      {
        case LogSeverity.Verbose:
          logger.LogInformation(message.Exception, message.Message, null);
          break;

        case LogSeverity.Info:
          logger.LogInformation(message.Exception, message.Message, null);
          break;

        case LogSeverity.Warning:
          logger.LogWarning(message.Exception, message.Message, null);
          break;

        case LogSeverity.Error:
          logger.LogError(message.Exception, message.Message, null);
          break;

        case LogSeverity.Critical:
          logger.LogCritical(message.Exception, message.Message, null);
          break;
      }
      return Task.CompletedTask;
    }
  }
}
