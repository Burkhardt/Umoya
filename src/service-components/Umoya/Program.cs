using System.Threading.Tasks;
using Umoya.Core;
using Umoya.Hosting;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Umoya
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "Umoya",
                Description = "The spirit of Machine Learning and Deep Learning. Versioning resource(like model, data and code) and manage its dependencies for ai projects.",
            };

            app.OnExecuteAsync(async cancellationToken =>
            {
                var host = CreateWebHostBuilder(args).Build();

                await host.RunMigrationsAsync(cancellationToken);
                await host.RunAsync(cancellationToken);
            });
            await app.ExecuteAsync(args);          
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            CreateHostBuilder(args)
                .ConfigureWebHostDefaults(web =>
                {
                    web.ConfigureKestrel(options =>
                    {
                        // Remove the upload limit from Kestrel. If needed, an upload limit can
                        // be enforced by a reverse proxy server, like IIS.
                        options.Limits.MaxRequestBodySize = null;
                    });
                    web.UseStartup<Startup>();
                    web.UseUrls("https://+:8007;http://+:8006");
                });

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseUmoya();
                
    }
}
