using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Starship.Core.Expressions;
using Starship.Core.Expressions.Interfaces;

namespace Starship.Core.Processing {
    public class Pipeline {

        public Pipeline() {
            Sinks = new ConcurrentDictionary<Type, IExpressionConverter>();
        }

        public void Add<T>(Func<T, object> callback) {
            Sinks.TryAdd(typeof(T), new ConversionAdapter<T, object>(callback));
        }

        public virtual async Task<object> Process(object input) {
            var type = input.GetType();

            if (Sinks.ContainsKey(type)) {
                return await Sinks[type].ConvertAsync(input);
            }

            return null;
        }

        private ConcurrentDictionary<Type, IExpressionConverter> Sinks { get; set; }
    }
}