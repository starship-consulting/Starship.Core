using System;
using Starship.Core.Interfaces;

namespace Starship.Core.Reflection.Converters {
    public class NumberConverter : IConverter {
        public bool CanConvert(Type type, object instance) {
            if (instance is string) {
                if (type == typeof (int) || type == typeof (decimal) || type == typeof (float) || type == typeof (double)) {
                    return true;
                }
            }

            return false;
        }

        public object Convert(MethodInvoker invoker, Type type, object instance) {
            if (type == typeof (int)) {
                return int.Parse(instance.ToString());
            }

            if (type == typeof (decimal)) {
                return decimal.Parse(instance.ToString());
            }

            if (type == typeof (float)) {
                return float.Parse(instance.ToString());
            }

            if (type == typeof (double)) {
                return double.Parse(instance.ToString());
            }

            return instance;
        }
    }
}