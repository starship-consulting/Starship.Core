using System;
using Newtonsoft.Json.Linq;
using Starship.Core.Interfaces;

namespace Starship.Core.Reflection.Converters {
    public class JObjectConverter : IConverter {
        public bool CanConvert(Type type, object instance) {
            return instance is JObject;
        }

        public object Convert(MethodInvoker invoker, Type type, object instance) {
            return ((JObject) instance).ToObject(type);
        }
    }
}