using Newtonsoft.Json;

namespace Umoya.Protocol
{
    public class AutocompleteContext
    {
        public static readonly AutocompleteContext Default = new AutocompleteContext
        {
            Vocab = "http://schema.nuget.org/schema#"
        };

        [JsonProperty("@vocab")]
        public string Vocab { get; set; }
    }
}
