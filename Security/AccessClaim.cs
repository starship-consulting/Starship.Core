using System;
using Newtonsoft.Json;

namespace Starship.Core.Security {
    public class AccessClaim {
        
        public AccessClaim() {
        }

        public AccessClaim(string type, string scope, params string[] rights) {
            Type = type;
            Scope = scope;
            Rights = rights;
        }

        [JsonProperty(PropertyName="type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName="scope")]
        public string Scope { get; set; }

        [JsonProperty(PropertyName="rights")]
        public string[] Rights { get; set; }
    }
}