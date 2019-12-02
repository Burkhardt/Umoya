using System;
using System.Collections.Generic;
using Umoya.Protocol.Converters;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Umoya.Protocol
{
    /// <summary>
    /// The entry point for a NuGet package source used by the client to find APIs.
    /// Documentation: https://docs.microsoft.com/en-us/nuget/api/overview
    /// NuGet.org: https://api.nuget.org/v3-index/index.json
    /// </summary>
    public class ServiceIndexResponse
    {
        public ServiceIndexResponse(NuGetVersion version, IReadOnlyList<ServiceIndexResource> resources)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        /// <summary>
        /// The service index's version.
        /// </summary>
        [JsonConverter(typeof(NuGetVersionConverter))]
        public NuGetVersion Version { get; }

        /// <summary>
        /// The resource contained by this service index.
        /// </summary>
        public IReadOnlyList<ServiceIndexResource> Resources { get; }
    }
}
