using System.Collections.Generic;
using Umoya.Protocol.Models;
using Newtonsoft.Json;

namespace Umoya.Core
{
    /// <summary>
    /// Umoya's extensions to the package metadata model. These additions
    /// are not part of the official protocol.
    /// </summary>
    public class UmoyaPackageMetadata : PackageMetadata
    {
        [JsonProperty("downloads")]
        public long Downloads { get; set; }

        [JsonProperty("hasReadme")]
        public bool HasReadme { get; set; }

        [JsonProperty("packageTypes")]
        public IReadOnlyList<string> PackageTypes { get; set; }

        /// <summary>
        /// The package's release notes.
        /// </summary>
        [JsonProperty("releaseNotes")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("repositoryUrl")]
        public string RepositoryUrl { get; set; }

        [JsonProperty("repositoryType")]
        public string RepositoryType { get; set; }

    }
}
