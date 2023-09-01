using BeanBot.Application.Audio;
using BeanBot.Application.Common;
using BeanBot.Infrastructure;
using BeanBot.Services;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BeanBot
{
  internal static class DependencyInjection
  {
    public static IServiceCollection AddBotServices(this IServiceCollection services)
    {
      return services
        .AddDiscord()
        .AddHostedServices()
        .AddApplication()
        .AddInfrastructure();
    }

    private static IServiceCollection AddDiscord(this IServiceCollection services)
    {
      return services
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton<InteractionService>()
        .AddSingleton<CommandService>();
    }

    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
      return services
        .AddHostedService<DiscordStartupService>()
        .AddHostedService<InteractionHandlingService>();
    }

    private static IServiceCollection AddApplication(this IServiceCollection services)
    {
      return services
        .AddSingleton<IAudioService, AudioService>();
    }

    private static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
      return services
        .AddSingleton<ITtsClient, TtsClient>();
    }
  }
}
