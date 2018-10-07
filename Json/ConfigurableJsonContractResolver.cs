using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Starship.Core.Extensions;

namespace Starship.Core.Json {
    public class ConfigurableJsonContractResolver : DefaultContractResolver {

        public ConfigurableJsonContractResolver() {
            ShowEmptyCollections = true;
            UseCamelCase = false;
            SortAlphabetically = true;
            IdPropertyAlwaysFirst = true;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            var property = base.CreateProperty(member, memberSerialization);

            if(!ShowEmptyCollections) {
                if (property.PropertyType.IsCollection()) {
                    property.ShouldSerialize = instance => (member.Get(instance) as IEnumerable<object>)?.Count() > 0;
                }
            }

            return property;
        }

        public override JsonContract ResolveContract(Type type) {
            if(UseCamelCase) {
                NamingStrategy = new CamelCaseNamingStrategy(true, true);
            }

            return base.ResolveContract(type);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {

            var properties = base.CreateProperties(type, memberSerialization);

            if(IdPropertyAlwaysFirst && SortAlphabetically) {
                properties = properties.OrderByDescending(p => p.PropertyName.ToLower() == "id").ThenBy(each => each.PropertyName).ToList();
            }
            else if(IdPropertyAlwaysFirst) {
                properties = properties.OrderByDescending(p => p.PropertyName.ToLower() == "id").ToList();
            }
            else if(SortAlphabetically) {
                properties = properties.OrderByDescending(each => each.PropertyName).ToList();
            }
            
            return properties;
        }

        public bool IdPropertyAlwaysFirst { get; set; }

        public bool SortAlphabetically { get; set; }

        public bool ShowEmptyCollections { get; set; }

        public bool UseCamelCase { get; set; }
    }
}