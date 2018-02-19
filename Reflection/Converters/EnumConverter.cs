using System;
using Starship.Core.Interfaces;

namespace Starship.Core.Reflection.Converters {
    public class EnumConverter : IConverter {
        public bool CanConvert(Type type, object instance) {
            return type.IsEnum;
        }

        public object Convert(MethodInvoker invoker, Type type, object instance) {
            return Enum.ToObject(type, instance);
        }
    }
}