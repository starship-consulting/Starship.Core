using System;
using Starship.Core.Processing;

namespace Starship.Core.Events {
    public static class EventStream {
        static EventStream() {
            Router = new TypeRouter();
        }

        public static void On<T>(Action<T> callback) {
            Router.On(callback);
        }

        public static void Publish(object e) {
            Router.Publish(e);
        }

        private static TypeRouter Router { get; set; }
    }
}