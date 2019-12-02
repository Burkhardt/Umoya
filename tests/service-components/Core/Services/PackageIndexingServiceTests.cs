using System.Threading.Tasks;
using Umoya.Core.Configuration;
using Umoya.Core.Indexing;
using Umoya.Core.Metadata;
using Umoya.Core.Search;
using Umoya.Core.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Umoya.Core.Tests.Services
{
    public class PackageIndexingServiceTests
    {
        private readonly Mock<IPackageService> _packages;
        private readonly Mock<IPackageStorageService> _storage;
        private readonly Mock<IUmoyaSearchService> _search;
        private readonly PackageIndexingService _target;

        public PackageIndexingServiceTests()
        {
            _packages = new Mock<IPackageService>();
            _storage = new Mock<IPackageStorageService>();
            _search = new Mock<IUmoyaSearchService>();

            _target = new PackageIndexingService(
                _packages.Object,
                _storage.Object,
                _search.Object,
                Mock.Of<IOptionsSnapshot<UmoyaOptions>>(),
                Mock.Of<ILogger<PackageIndexingService>>());
        }

        // TODO: Add malformed package tests

        [Fact]
        public async Task WhenPackageAlreadyExists_ReturnsPackageAlreadyExists()
        {
            await Task.Yield();
        }

        [Fact]
        public async Task WhenDatabaseAddFailsBecausePackageAlreadyExists_ReturnsPackageAlreadyExists()
        {
            await Task.Yield();
        }

        [Fact]
        public async Task IndexesPackage()
        {
            await Task.Yield();
        }

        [Fact]
        public async Task WhenPackageHasNoReadme_SavesNullReadmeStream()
        {
            await Task.Yield();
        }

        [Fact]
        public async Task ThrowsWhenStorageSaveThrows()
        {
            await Task.Yield();
        }
    }
}
