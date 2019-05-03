using System;
using Newtonsoft.Json;

namespace Starship.Core.Security {
    public class EntityParticipant {
        
        public EntityParticipant() {
        }

        public EntityParticipant(string id, string role) {
            Id = id;
            Role = role;
        }

        [JsonProperty(PropertyName="id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName="role")]
        public string Role { get; set; }
    }
}