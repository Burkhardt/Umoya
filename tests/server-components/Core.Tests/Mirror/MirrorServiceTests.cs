using System.Threading.Tasks;
using Umoya.Protocol;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Umoya.Core;

namespace Umoya.Core.Tests.Mirror
{
    public class MirrorServiceTests
    {
        public class FindPackageVersionsOrNullAsync : FactsBase
        {
            [Fact]
            public async Task MergesLocalAndUpstream()
            {
                await Task.Yield();
            }
        }

        public class FindPackagesOrNullAsync : FactsBase
        {
            [Fact]
            public async Task MergesLocalAndUpstream()
            {
                await Task.Yield();
            }
        }

        public class MirrorAsync : FactsBase
        {
            [Fact]
            public async Task SkipsIfAlreadyMirrored()
            {
                await Task.Yield();
            }

            [Fact]
            public async Task SkipsIfUpstreamDoesntHavePackage()
            {
                await Task.Yield();
            }

            [Fact]
            public async Task MirrorsPackage()
            {
                await Task.Yield();
            }
        }

        public class FactsBase
        {
            private readonly Mock<IPackageService> _packages;
            private readonly Mock<NuGetClient> _upstream;
            private readonly Mock<IPackageIndexingService> _indexer;

            private readonly MirrorService _target;

            public FactsBase()
            {
                _packages = new Mock<IPackageService>();
                _upstream = new Mock<NuGetClient>();
                _indexer = new Mock<IPackageIndexingService>();

                _target = new MirrorService(
                    _packages.Object,
                    _upstream.Object,
                    _indexer.Object,
                    Mock.Of<ILogger<MirrorService>>());
            }
        }
    }
}
