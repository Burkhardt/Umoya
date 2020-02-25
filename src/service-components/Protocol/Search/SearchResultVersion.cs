using System;
using Umoya.Protocol.Converters;
using Newtonsoft.Json;
using NuGet.Versioning;

namespace Umoya.Protocol
{
    /// <summary>
    /// The version of a package that matched a search query.
    /// See: <see cref="SearchResult"/>.
    /// Documentation: https://docs.microsoft.com/en-us/nuget/api/search-query-service-resource#search-result
    /// </summary>
    public class SearchResultVersion
    {        

        public SearchResultVersion(
            string registrationLeafUrl,
            NuGetVersion version,
            long downloads)
        {
            version = version ?? throw new ArgumentNullException(nameof(version));

            RegistrationLeafUrl = registrationLeafUrl;
            Version = version;
            Downloads = downloads;
        }

        public SearchResultVersion(string registrationLeafUrl, NuGetVersion version, long downloads, DateTime published)
        {
            RegistrationLeafUrl = registrationLeafUrl;
            Version = version;
            Downloads = downloads;
            Published = published;
        }

        [JsonProperty(PropertyName = "@id")]
        public string RegistrationLeafUrl { get; }

        [JsonConverter(typeof(NuGetVersionConverter), NuGetVersionConversionFlags.IncludeBuildMetadata)]
        public NuGetVersion Version { get; }

        public long Downloads { get; }

        public DateTime Published { get; }

    }
}
