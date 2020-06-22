using Umoya.Protocol.Models;
using Newtonsoft.Json;

namespace Umoya.Core
{
    /// <summary>
    /// Umoya's extensions to a registration index response. These additions
    /// are not part of the official protocol.
    /// </summary>
    public class UmoyaRegistrationIndexResponse : RegistrationIndexResponse
    {
        /// <summary>
        /// How many times all versions of this package have been downloaded.
        /// </summary>
        [JsonProperty("totalDownloads")]
        public long TotalDownloads { get; set; }
    }
}
