using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
      //config.AddYamlFile("_config.yml", false);       // Add the config file to IConfiguration variables
    })
    .ConfigureLogging(logging => {
      logging.SetMinimumLevel(LogLevel.Information);
      logging.AddSimpleConsole(options =>
      {
        options.SingleLine = true;
      });
    })
    .ConfigureServices(services => services.AddBotServices())
    .Build();

await host.RunAsync().ConfigureAwait(false);
