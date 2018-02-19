using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Starship.Core.Json {
    public class AlphanumericContractResolver : DefaultContractResolver {

        public AlphanumericContractResolver(List<string> top = null, List<string> bottom = null) {
            Top = top ?? new List<string>();
            Bottom = bottom ?? new List<string>();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
            var properties = base.CreateProperties(type, memberSerialization);
            var top = properties.Where(property => Top.Contains(property.PropertyName));
            var bottom = properties.Where(property => Bottom.Contains(property.PropertyName));
            var remaining = properties.Where(property => !top.Contains(property) && !bottom.Contains(property));

            return top.Concat(remaining).Concat(bottom).ToList();
        }

        private List<string> Top { get; set; } 

        private List<string> Bottom { get; set; } 
    }
}