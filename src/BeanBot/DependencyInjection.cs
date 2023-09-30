using BeanBot.Application.Common;
using BeanBot.Application.DiscordClient;
using BeanBot.HostedServices;
using BeanBot.Infrastructure;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

internal static class DependencyInjection
{
  public static IServiceCollection AddBotServices(this IServiceCollection services)
  {
    return services
      .AddDiscordServices()
      .AddLavalinkServices()
      .AddMediatRServices()
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

  private static IServiceCollection AddMediatRServices(this IServiceCollection services)
  {
    return services.AddMediatR(Assembly.GetExecutingAssembly());
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
