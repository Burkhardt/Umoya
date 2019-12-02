using System;
using Umoya.Core.Mirror;
using Umoya.Extensions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Umoya
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "umoya",
                Description = "A light-weight NuGet service",
            };

            app.HelpOption(inherited: true);

            app.Command("import", import =>
            {
                import.Command("downloads", downloads =>
                {
                    downloads.OnExecute(async () =>
                    {
                        var provider = CreateHostBuilder(args).Build().Services;

                        await provider
                            .GetRequiredService<DownloadsImporter>()
                            .ImportAsync();
                    });
                });
            });

            app.OnExecute(() =>
            {
                CreateWebHostBuilder(args).Build().Run();
            });

            app.Execute(args);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = null;
                })
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var root = Environment.GetEnvironmentVariable("CONFIG_ROOT");
                    if (!string.IsNullOrEmpty(root))
                        config.SetBasePath(root);
                })
                .UseUrls("http://+:8007");

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder()
                .ConfigureUmoyaConfiguration(args)
                .ConfigureUmoyaServices()
                .ConfigureUmoyaLogging();
        }
    }
}
