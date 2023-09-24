using BeanBot.Application.Common;
using BeanBot.Application.DiscordClient;
using BeanBot.HostedServices;
using BeanBot.Infrastructure;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;

internal static class DependencyInjection
{
  public static IServiceCollection AddBotServices(this IServiceCollection services)
  {
    return services
      .AddDiscordServices()
      .AddLavalinkServices()
      .AddHostedServices()
      .AddApplicationServices()
      .AddInfrastructureServices();
  }

  private static IServiceCollection AddDiscordServices(this IServiceCollection services)
  {
    return services
      .AddSingleton<DiscordSocketClient>()
      .AddSingleton<InteractionService>();
  }

  private static IServiceCollection AddLavalinkServices(this IServiceCollection services)
  {
    return services.AddLavalink();
  }

  private static IServiceCollection AddHostedServices(this IServiceCollection services)
  {
    return services
      .AddHostedService<DiscordClientService>();
  }

  private static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    return services.AddSingleton<IDiscordClientStatusService, DiscordClientStatusService>();
  }

  private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
  {
    return services.AddSingleton<IDiscordLogger, DiscordLogger>();
  }
}
