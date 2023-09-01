using BeanBot.Services;
using Discord;
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
        .AddHostedServices();
    }

    private static IServiceCollection AddDiscord(this IServiceCollection services)
    {
      return services
        .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig()
        {
          GatewayIntents = GatewayIntents.All,
        }))
        .AddSingleton<InteractionService>()
        .AddSingleton<CommandService>();
    }

    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
      return services
        .AddHostedService<DiscordStartupService>()
        .AddHostedService<InteractionHandlingService>();
    }
  }
}
