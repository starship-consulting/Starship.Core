using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Starship.Core.Security;

namespace Starship.Core.Json {
    public class SecureJsonContractResolver : DefaultContractResolver {

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
      
            var property = base.CreateProperty(member, memberSerialization);
            var attributes = property.AttributeProvider.GetAttributes(typeof(SecureAttribute), true);

            if(member.MemberType == MemberTypes.Property) {
                property.ValueProvider = new NullStringValueProvider(property.ValueProvider);
            }

            if (attributes.Any()) {
                if(ShouldSerialize == null) {
                    property.ShouldSerialize = instance => false;
                    return property;
                }

                property.ShouldSerialize = instance => ShouldSerialize();
            }
      
            return property;
        }

        public Func<bool> ShouldSerialize;
    }
}