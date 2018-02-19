using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;
using Starship.Core.Interfaces;

namespace Starship.Core.Reflection.Converters {
    public class JArrayConverter : IConverter {
        public bool CanConvert(Type type, object instance) {
            return instance is JArray;
        }

        public object Convert(MethodInvoker invoker, Type type, object instance) {
            var collection = Activator.CreateInstance(type) as IList;

            foreach (var item in instance as JArray) {
                var converted = invoker.TryConvert(type.GetGenericArguments().First(), item);

                collection.Add(converted);
            }

            return collection;
        }
    }
}