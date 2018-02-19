using System;
using Starship.Core.Interfaces;

namespace Starship.Core.Reflection.Converters {
    public class DateTimeConverter : IConverter {
        public bool CanConvert(Type type, object instance) {
            return instance is string && (typeof (DateTime).IsAssignableFrom(type) || typeof (DateTime?).IsAssignableFrom(type));
        }

        public object Convert(MethodInvoker invoker, Type type, object instance) {
            return DateTime.Parse(instance.ToString());
        }
    }
}