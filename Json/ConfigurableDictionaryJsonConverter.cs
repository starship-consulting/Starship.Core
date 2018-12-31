using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Starship.Core.Extensions;

namespace Starship.Core.Json {

    public class ConfigurableDictionaryJsonConverter : JsonConverter {

        public override bool CanRead => false;

        public override bool CanConvert(Type type) {
            return typeof(Dictionary<string, object>).IsAssignableFrom(type);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteStartObject();
            WriteObjectProperties(writer, (Dictionary<string, object>) value, serializer);
            writer.WriteEndObject();
        }

        protected virtual void WriteObjectProperties(JsonWriter writer, IEnumerable<KeyValuePair<string, object>> properties, JsonSerializer serializer) {
            
            if (!IncludeUnderscoreProperties) {
                properties = properties.Where(each => !each.Key.StartsWith("_"));
            }
            
            if (IdPropertyAlwaysFirst && SortAlphabetically) {
                properties = properties.OrderByDescending(p => p.Key.ToLower() == "id").ThenBy(each => each.Key);
            }
            else if (IdPropertyAlwaysFirst) {
                properties = properties.OrderByDescending(p => p.Key.ToLower() == "id");
            }
            else if (SortAlphabetically) {
                properties = properties.OrderByDescending(each => each.Key);
            }

            foreach (var property in properties) {

                if(!IncludeDefaultValues) {

                    if(property.Value == null) {
                        continue;
                    }

                    if(property.Value is string && string.IsNullOrEmpty((string) property.Value)) {
                        continue;
                    }
                    
                    var defaultValue = GetDefault(property.Value.GetType());

                    if(property.Value == defaultValue) {
                        continue;
                    }
                }

                writer.WritePropertyName(property.Key);

                if(property.Value.GetType().IsCollection()) {
                    serializer.Serialize(writer, property.Value);
                }
                else {
                    writer.WriteValue(property.Value);
                }
            }
        }

        public object GetDefault(Type t) {
            return GetType().GetMethod("MakeDefault").MakeGenericMethod(t).Invoke(this, null);
        }

        public T MakeDefault<T>() {
            return default(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public bool IncludeDefaultValues { get; set; }

        public bool IdPropertyAlwaysFirst { get; set; }

        public bool SortAlphabetically { get; set; }

        public bool ShowEmptyCollections { get; set; }

        public bool IncludeUnderscoreProperties { get; set; }

        public bool UseCamelCase { get; set; }
    }
}