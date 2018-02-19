using System;
using System.Collections.Generic;
using System.Linq;

namespace Starship.Core.Utility {
    public static class AttributeCache<T> where T : Attribute {

        static AttributeCache() {
            Attributes = new Dictionary<Type, List<T>>();
        }

        public static List<T> GetAttributes(Type type) {
            lock (Attributes) {
                if (Attributes.ContainsKey(type)) {
                    return Attributes[type];
                }

                var classAttributes = type.GetCustomAttributes(typeof (T), true);
                var propertyAttributes = type.GetProperties().SelectMany(property => property.GetCustomAttributes(typeof (T), true));

                var attributes = classAttributes
                    .Concat(propertyAttributes)
                    .Cast<T>()
                    .ToList();

                Attributes.Add(type, attributes);

                return attributes;
            }
        }

        private static Dictionary<Type, List<T>> Attributes { get; set; }
    }
}