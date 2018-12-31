using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Starship.Core.Extensions;

namespace Starship.Core.Json {
    public class ConfigurableJsonContractResolver : DefaultContractResolver {

        public ConfigurableJsonContractResolver() {
            IncludeDefaultValues = true;
            IncludeUnderscoreProperties = false;
            ShowEmptyCollections = true;
            UseCamelCase = false;
            SortAlphabetically = true;
            IdPropertyAlwaysFirst = true;
        }
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            var property = base.CreateProperty(member, memberSerialization);

            if (member.MemberType == MemberTypes.Property && property.PropertyType == typeof(string)) {
                //property.ValueProvider = new NullStringValueProvider(property.ValueProvider);
            }

            /*if (!IncludeDefaultValues) {
                property.ShouldSerialize = instance => {
                    var value = member.Get(instance);
                    var defaultValue = GetDefault(property.PropertyType);

                    return value != defaultValue;
                };
            }*/

            if (!ShowEmptyCollections) {
                if (property.PropertyType.IsCollection()) {
                    property.ShouldSerialize = instance => (member.Get(instance) as IEnumerable<object>)?.Count() > 0;
                }
            }

            return property;
        }
        
        public object GetDefault(Type t) {
            return GetType().GetMethod("MakeDefault").MakeGenericMethod(t).Invoke(this, null);
        }

        public T MakeDefault<T>() {
            return default(T);
        }

        public override JsonContract ResolveContract(Type type) {
            var contract = base.ResolveContract(type);

            if(contract is JsonDictionaryContract) {
                var dictionaryContract = contract as JsonDictionaryContract;
            }

            return contract;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType) {
            var converter = base.ResolveContractConverter(objectType);
            return converter;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
            var properties = base.CreateProperties(type, memberSerialization);

            if (UseCamelCase) {
                NamingStrategy = new CamelCaseNamingStrategy(true, true);
            }

            if (!IncludeUnderscoreProperties) {
                properties = properties.Where(each => !each.PropertyName.StartsWith("_")).ToList();
            }

            if (IdPropertyAlwaysFirst && SortAlphabetically) {
                properties = properties.OrderByDescending(p => p.PropertyName.ToLower() == "id").ThenBy(each => each.PropertyName).ToList();
            }
            else if (IdPropertyAlwaysFirst) {
                properties = properties.OrderByDescending(p => p.PropertyName.ToLower() == "id").ToList();
            }
            else if (SortAlphabetically) {
                properties = properties.OrderByDescending(each => each.PropertyName).ToList();
            }

            return properties;
        }

        public bool IncludeDefaultValues { get; set; }

        public bool IdPropertyAlwaysFirst { get; set; }

        public bool SortAlphabetically { get; set; }

        public bool ShowEmptyCollections { get; set; }

        public bool IncludeUnderscoreProperties { get; set; }

        public bool UseCamelCase { get; set; }
    }
}