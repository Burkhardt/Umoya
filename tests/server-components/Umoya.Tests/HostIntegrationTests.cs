using System;
using System.Collections.Generic;
using Umoya.Core;
using Umoya.Database.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Umoya.Tests
{
    public class HostIntegrationTests
    {
        private readonly string DatabaseTypeKey = $"{nameof(UmoyaOptions.Database)}:{nameof(DatabaseOptions.Type)}";
        private readonly string ConnectionStringKey = $"{nameof(UmoyaOptions.Database)}:{nameof(DatabaseOptions.ConnectionString)}";

        [Fact]
        public void ThrowsIfDatabaseTypeInvalid()
        {
            var provider = BuildServiceProvider(new Dictionary<string, string>
            {
                { DatabaseTypeKey, "InvalidType" }
            });

            Assert.Throws<InvalidOperationException>(
                () => provider.GetRequiredService<IContext>());
        }

        [Fact]
        public void ReturnsDatabaseContext()
        {
            var provider = BuildServiceProvider(new Dictionary<string, string>
            {
                { DatabaseTypeKey, DatabaseType.Sqlite.ToString() },
                { ConnectionStringKey, "..." }
            });

            Assert.NotNull(provider.GetRequiredService<IContext>());
        }

        [Fact]
        public void ReturnsSqliteContext()
        {
            var provider = BuildServiceProvider(new Dictionary<string, string>
            {
                { DatabaseTypeKey, DatabaseType.Sqlite.ToString() },
                { ConnectionStringKey, "..." }
            });

            Assert.NotNull(provider.GetRequiredService<SqliteContext>());
        }

        [Fact]
        public void DefaultsToSqlite()
        {
            var provider = BuildServiceProvider();

            var context = provider.GetRequiredService<IContext>();

            Assert.IsType<SqliteContext>(context);
        }

        private IServiceProvider BuildServiceProvider(Dictionary<string, string> configs = null)
        {
            var hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.UseUmoya();
            hostBuilder.ConfigureAppConfiguration((ctx, config) =>
            {
                config.AddInMemoryCollection(configs ?? new Dictionary<string, string>());
            });

            return hostBuilder.Build().Services;
        }
    }
}
