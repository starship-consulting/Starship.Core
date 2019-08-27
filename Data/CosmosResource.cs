using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Starship.Core.Interfaces;

namespace Starship.Core.Data {
    public class CosmosResource : Dictionary<string, object>, HasId {

        public string Get(string key) {
            return Get<string>(key);
        }

        public T Get<T>(string key) {

            if(!ContainsKey(key)) {
                return default;
            }
            
            if(this[key] is JObject jObject) {
                return jObject.ToObject<T>();
            }

            if(this[key] is JArray jArray) {
                return jArray.ToObject<T>();
            }

            var value = this[key];

            try {
                if(value != null && typeof(T) != value.GetType() && !typeof(IConvertible).IsAssignableFrom(typeof(T))) {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
                }
            }
            catch {
            }
            
            return (T)Convert.ChangeType(this[key], typeof(T));
        }
        
        public void Set(string key, object value) {

            if(!ContainsKey(key)) {
                Add(key, value);
            }
            else {
                this[key] = value;
            }
        }

        public string GetId() {
            return Id;
        }

        public void SetId(object value) {
            Id = value.ToString();
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