using System;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Starship.Core.Reflection {
    public class UniversalTypeConverter {

        public object Convert(object source, Type type) {
            if (type.IsInstanceOfType(source)) {
                return source;
            }

            if (source is JObject) {
                source = ((JObject) source).ToObject(type);
            }
            else if (source is string) {
                source = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(source.ToString());
            }
            else if (source != null && !type.IsInstanceOfType(source)) {
                if (source is IConvertible) {
                    source = System.Convert.ChangeType(source, type);
                }
            }

            return source;
        }
    }
}