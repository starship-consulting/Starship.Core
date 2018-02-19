using System;

namespace Starship.Core.Reflection {
    public static class Automapper {

        public static T Map<T>(object from) where T : new() {
            var instance = new T();
            Map(from, instance);
            return instance;
        }

        public static void Map(object from, object to) {
            var toType = to.GetType();

            foreach (var property in from.GetType().GetProperties()) {
                var toProperty = toType.GetProperty(property.Name);

                if (toProperty != null && toProperty.PropertyType == property.PropertyType) {
                    toProperty.SetValue(to, property.GetValue(from));
                }
            }
        }
    }
}