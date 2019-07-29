using System.Collections.Generic;
using Newtonsoft.Json;

namespace Starship.Core.Data {
    public class CosmosResource : Dictionary<string, object> {

        public string Get(string key) {
            return Get<string>(key);
        }

        public T Get<T>(string key) {

            if(!ContainsKey(key)) {
                return default(T);
            }

            return (T)this[key];
        }
        
        public void Set(string key, object value) {

            if(!ContainsKey(key)) {
                Add(key, value);
            }
            else {
                this[key] = value;
            }
        }
        
        [JsonProperty(PropertyName="id")]
        public string Id {
            get => Get<string>("id");
            set => Set("id", value);
        }

        [JsonProperty(PropertyName="$type")]
        public string Type {
            get => Get<string>("$type");
            set => Set("$type", value);
        }
    }
}