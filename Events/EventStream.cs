using System;
using System.Collections.Generic;
using Starship.Core.Processing;

namespace Starship.Core.Events {
    public static class EventStream {

        static EventStream() {
            Router = new TypeRouter();
            GlobalCallbacks = new List<MulticastDelegate>();
        }
        
        public static void On(Action<object> callback) {
            GlobalCallbacks.Add(callback);
        }

        public static void On<T>(Action<T> callback) {
            Router.On(callback);
        }

        public static void Publish(object e) {
            Router.Publish(e);

            foreach(var callback in GlobalCallbacks) {
                callback.DynamicInvoke(e);
            }
        }

        private static TypeRouter Router { get; set; }

        private static List<MulticastDelegate> GlobalCallbacks { get; set; }
    }
}