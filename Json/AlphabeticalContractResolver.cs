using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Starship.Core.Json {
  public class AlphabeticalContractResolver : DefaultContractResolver {
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {

      var property = base.CreateProperty(member, memberSerialization);
      /*var attributes = property.AttributeProvider.GetAttributes(typeof(SecureAttribute), true);

      if (attributes.Any()) {
        property.ShouldSerialize = instance => UserContext.Current.Role >= RoleTypes.SiteAdmin;
        return property;
      }*/
      
      if (property.PropertyType == typeof (string)) {
        property.ValueProvider = new NullStringValueProvider(property.ValueProvider);
      }

      return property;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
      return base.CreateProperties(type, memberSerialization)
        .OrderByDescending(p => p.PropertyName.ToLower() == "id")
        .ThenBy(each => each.PropertyName)
        .ToList();
    }
  }
}