using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starship.Core.Processing {
    public class TypeRouter {
        public TypeRouter() {
            Callbacks = new Dictionary<Type, List<MulticastDelegate>>();
        }

        public void On<T>(Action<T> callback) {
            if (!Callbacks.ContainsKey(typeof(T))) {
                Callbacks.Add(typeof(T), new List<MulticastDelegate>());
            }

            Callbacks[typeof(T)].Add(callback);
        }

        public void On<T>(Func<T, Task> callback) {
            if (!Callbacks.ContainsKey(typeof(T))) {
                Callbacks.Add(typeof(T), new List<MulticastDelegate>());
            }

            Callbacks[typeof(T)].Add(callback);
        }

        public void Publish(object obj) {
            var type = obj.GetType();

            if (Callbacks.ContainsKey(type)) {
                foreach (var callback in Callbacks[type]) {
                    callback.DynamicInvoke(new[] {obj});
                }
            }
        }

        public async Task PublishAsync(object obj) {
            var type = obj.GetType();

            if (Callbacks.ContainsKey(type)) {
                foreach (var callback in Callbacks[type]) {
                    await (Task) callback.DynamicInvoke(new[] {obj});
                }
            }
        }

        private Dictionary<Type, List<MulticastDelegate>> Callbacks { get; set; }
    }
}