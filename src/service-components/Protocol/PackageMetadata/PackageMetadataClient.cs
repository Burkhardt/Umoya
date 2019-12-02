using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace Umoya.Protocol
{
    /// <summary>
    /// The client to interact with an upstream source's Package Metadata resource.
    /// </summary>
    public class PackageMetadataClient : IPackageMetadataService
    {
        private readonly IUrlGeneratorFactory _urlGenerator;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Create a new Package Metadata client.
        /// </summary>
        /// <param name="urlGenerator">The service to generate URLs to upstream resources.</param>
        /// <param name="httpClient">The HTTP client used to send requests.</param>
        public PackageMetadataClient(IUrlGeneratorFactory urlGenerator, HttpClient httpClient)
        {
            _urlGenerator = urlGenerator ?? throw new ArgumentNullException(nameof(urlGenerator));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc />
        public async Task<RegistrationIndexResponse> GetRegistrationIndexOrNullAsync(string id, CancellationToken cancellationToken = default)
        {
            var urlGenerator = await _urlGenerator.CreateAsync();
            var url = urlGenerator.GetRegistrationIndexUrl(id);
            var response = await _httpClient.DeserializeUrlAsync<RegistrationIndexResponse>(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return response.GetResultOrThrow();
        }

        /// <inheritdoc />
        public async Task<RegistrationPageResponse> GetRegistrationPageOrNullAsync(
            string id,
            NuGetVersion lower,
            NuGetVersion upper,
            CancellationToken cancellationToken = default)
        {
            var urlGenerator = await _urlGenerator.CreateAsync();
            var url = urlGenerator.GetRegistrationPageUrl(id, lower, upper);
            var response = await _httpClient.DeserializeUrlAsync<RegistrationPageResponse>(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return response.GetResultOrThrow();
        }

        /// <inheritdoc />
        public async Task<RegistrationLeafResponse> GetRegistrationLeafOrNullAsync(
            string id,
            NuGetVersion version,
            CancellationToken cancellationToken = default)
        {
            var urlGenerator = await _urlGenerator.CreateAsync();
            var url = urlGenerator.GetRegistrationLeafUrl(id, version);
            var response = await _httpClient.DeserializeUrlAsync<RegistrationLeafResponse>(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return response.GetResultOrThrow();
        }
    }
}