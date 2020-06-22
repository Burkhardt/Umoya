using System;
using System.Collections.Generic;
using System.IO;
using Umoya;
using Umoya.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;


namespace Umoya.Tests
{
    public class UmoyaWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly string DatabaseTypeKey = $"{nameof(UmoyaOptions.Database)}:{nameof(DatabaseOptions.Type)}";
        private readonly string ConnectionStringKey = $"{nameof(UmoyaOptions.Database)}:{nameof(DatabaseOptions.ConnectionString)}";
        private readonly string StorageTypeKey = $"{nameof(UmoyaOptions.Storage)}:{nameof(StorageOptions.Type)}";
        private readonly string FileSystemStoragePathKey = $"{nameof(UmoyaOptions.Storage)}:{nameof(FileSystemStorageOptions.Path)}";
        private readonly string SearchTypeKey = $"{nameof(UmoyaOptions.Search)}:{nameof(SearchOptions.Type)}";
        private readonly string MirrorEnabledKey = $"{nameof(UmoyaOptions.Mirror)}:{nameof(MirrorOptions.Enabled)}";

        public WebApplicationFactory<Startup> WithOutput(ITestOutputHelper output)
        {
            return WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging =>
                {
                    logging.AddProvider(new XunitLoggerProvider(output));
                });
            });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Create temporary storage paths.
            var tempPath = Path.Combine(
                Path.GetTempPath(),
                "BaGetTests",
                Guid.NewGuid().ToString("N"));
            var sqlitePath = Path.Combine(tempPath, "Umoya.db");
            var storagePath = Path.Combine(tempPath, "Packages");

            Directory.CreateDirectory(tempPath);

            builder
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(config =>
                {
                    // Setup the integration test configuration.
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { DatabaseTypeKey, DatabaseType.Sqlite.ToString() },
                        { ConnectionStringKey, $"Data Source={sqlitePath}" },
                        { StorageTypeKey, StorageType.FileSystem.ToString() },
                        { FileSystemStoragePathKey, storagePath },
                        { SearchTypeKey, nameof(SearchType.Database) },
                        { MirrorEnabledKey, false.ToString() },
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    // Setup the integration test database.
                    var provider = services.BuildServiceProvider();
                    var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

                    using (var scope = scopeFactory.CreateScope())
                    {
                        scope.ServiceProvider
                            .GetRequiredService<IContext>()
                            .Database
                            .Migrate();
                    }
                });
        }
    }
}
