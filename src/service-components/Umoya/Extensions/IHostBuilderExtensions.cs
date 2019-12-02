using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Umoya.Extensions
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder ConfigureUmoyaConfiguration(this IHostBuilder builder, string[] args)
        {
            return builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddEnvironmentVariables();

                config
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });
        }

        public static IHostBuilder ConfigureUmoyaLogging(this IHostBuilder builder)
        {
            return builder
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                });
        }

        public static IHostBuilder ConfigureUmoyaServices(this IHostBuilder builder)
        {
            return builder
                .ConfigureServices((context, services) => services.ConfigureBaGet(context.Configuration));
        }
    }
}
