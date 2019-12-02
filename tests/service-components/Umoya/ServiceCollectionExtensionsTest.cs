using System;
using System.Collections.Generic;
using Umoya.Core.Configuration;
using Umoya.Core.Entities;
using Umoya.Database.Sqlite;
using Umoya.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Umoya.Tests
{
    public class ServiceCollectionExtensionsTest
    {
        private readonly string DatabaseTypeKey = $"{nameof(UmoyaOptions.Database)}:{nameof(DatabaseOptions.Type)}";
        private readonly string ConnectionStringKey = $"{nameof(UmoyaOptions.Database)}:{nameof(DatabaseOptions.ConnectionString)}";

        [Fact]
        public void AskServiceProviderForNotConfiguredDatabaseOptions()
        {
            var provider = new ServiceCollection()
                .ConfigureBaGet(new ConfigurationBuilder().Build()) // Method Under Test
                .BuildServiceProvider();

            var expected = Assert.Throws<InvalidOperationException>(
                () => provider.GetRequiredService<IContext>().Database);

            Assert.Contains(nameof(UmoyaOptions.Database), expected.Message);
        }

        [Fact]
        public void AskServiceProviderForWellConfiguredDatabaseOptions()
        {
            // Create a IConfiguration with a minimal "Database" object.
            var initialData = new Dictionary<string, string>();
            initialData.Add(DatabaseTypeKey, DatabaseType.Sqlite.ToString());
            initialData.Add(ConnectionStringKey, "blabla");

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData)
                .Build();

            var provider = new ServiceCollection()
                .ConfigureBaGet(configuration) // Method Under Test
                .BuildServiceProvider();

            Assert.NotNull(provider.GetRequiredService<IContext>());
        }

        [Fact]
        public void AskServiceProviderForWellConfiguredSqliteContext()
        {
            // Create a IConfiguration with a minimal "Database" object.
            var initialData = new Dictionary<string, string>();
            initialData.Add(DatabaseTypeKey, DatabaseType.Sqlite.ToString());
            initialData.Add(ConnectionStringKey, "blabla");

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData)
                .Build();

            var provider = new ServiceCollection()
                .ConfigureBaGet(configuration) // Method Under Test
                .BuildServiceProvider();

            Assert.NotNull(provider.GetRequiredService<SqliteContext>());
        }

        [Theory]
        [InlineData("<invalid>")]
        [InlineData("")]
        [InlineData(" ")]
        public void AskServiceProviderForInvalidDatabaseType(string databaseType)
        {
            // Create a IConfiguration with a minimal "Database" object.
            var initialData = new Dictionary<string, string>();
            initialData.Add(DatabaseTypeKey, databaseType);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData)
                .Build();

            var provider = new ServiceCollection()
                .ConfigureBaGet(configuration) // Method Under Test
                .BuildServiceProvider();

            var expected = Assert.Throws<InvalidOperationException>(
                () => provider.GetRequiredService<IContext>().Database);
        }
    }
}
