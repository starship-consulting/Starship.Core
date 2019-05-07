using Newtonsoft.Json;

namespace Starship.Core.Extensions {
    public static class GenericExtensions {
        public static T CloneAs<T>(this object instance) where T : new() {
            var clone = new T();
            var type = instance.GetType();

            foreach (var field in typeof (T).GetProperties()) {
                field.SetValue(clone, type.GetProperty(field.Name).GetValue(instance));
            }

            return clone;
        }

        public static T Clone<T>(this object instance) where T : new() {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance));
        }
    }
}