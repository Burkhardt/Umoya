using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Umoya
{
    // TODO: Move this to Umoya.Hosting.
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder UseBaGet(this IHostBuilder host)
        {
            host.ConfigureServices((context, services) =>
            {
                services.AddBaGet(context.Configuration);
            });

            host.ConfigureAppConfiguration((context, config) =>
            {
                var root = Environment.GetEnvironmentVariable("BAGET_CONFIG_ROOT");

                if (!string.IsNullOrEmpty(root))
                {
                    config.SetBasePath(root);
                }
            });

            return host;
        }
    }
}
