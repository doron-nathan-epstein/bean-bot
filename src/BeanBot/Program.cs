using BeanBot;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
      //config.AddYamlFile("_config.yml", false);       // Add the config file to IConfiguration variables
    })
    .ConfigureServices(services => services.AddBotServices())
    .Build();

await host.RunAsync();
