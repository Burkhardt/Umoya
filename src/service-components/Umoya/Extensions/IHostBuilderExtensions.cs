using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Umoya
{
    // TODO: Move this to Umoya.Hosting.
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder UseUmoya(this IHostBuilder host)
        {
            host.ConfigureServices((context, services) =>
            {
                services.AddUmoya(context.Configuration);
            });
            
            return host;
        }
    }
}
