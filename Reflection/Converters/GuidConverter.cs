using System;
using Starship.Core.Interfaces;

namespace Starship.Core.Reflection.Converters {
    public class GuidConverter : IConverter {
        public bool CanConvert(Type type, object instance) {
            if (instance == null) {
                return false;
            }

            Guid guid;
            return Guid.TryParse(instance.ToString(), out guid);
        }

        public object Convert(MethodInvoker invoker, Type type, object instance) {
            return Guid.Parse(instance.ToString());
        }
    }
}