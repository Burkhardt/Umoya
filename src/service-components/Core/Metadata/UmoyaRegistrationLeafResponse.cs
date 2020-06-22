using Umoya.Protocol.Models;
using Newtonsoft.Json;

namespace Umoya.Core
{
    /// <summary>
    /// Umoya's extensions to a registration leaf response. These additions
    /// are not part of the official protocol.
    /// </summary>
    public class UmoyaRegistrationLeafResponse : RegistrationLeafResponse
    {
        [JsonProperty("downloads")]
        public long Downloads { get; set; }
    }
}
