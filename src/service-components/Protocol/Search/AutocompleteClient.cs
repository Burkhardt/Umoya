using System;
using System.Threading;
using System.Threading.Tasks;
using Umoya.Protocol.Models;

namespace Umoya.Protocol
{
    public partial class NuGetClientFactory
    {
        private class AutocompleteClient : IAutocompleteClient
        {
            private readonly NuGetClientFactory _clientfactory;

            public AutocompleteClient(NuGetClientFactory clientFactory)
            {
                _clientfactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            }

            public async Task<AutocompleteResponse> AutocompleteAsync(
                string query = null,
                int skip = 0,
                int take = 20,
                bool includePrerelease = true,
                bool includeSemVer2 = true,
                CancellationToken cancellationToken = default)
            {
                // TODO: Support search failover.
                var client = await _clientfactory.GetAutocompleteClientAsync(cancellationToken);

                return await client.AutocompleteAsync(query, skip, take, includePrerelease, includeSemVer2, cancellationToken);
            }

            public async Task<AutocompleteResponse> ListPackageVersionsAsync(
                string packageId,
                bool includePrerelease = true,
                bool includeSemVer2 = true,
                CancellationToken cancellationToken = default)
            {
                // TODO: Support search failover.
                var client = await _clientfactory.GetAutocompleteClientAsync(cancellationToken);

                return await client.ListPackageVersionsAsync(packageId, includePrerelease, includeSemVer2, cancellationToken);
            }
        }
    }
}
