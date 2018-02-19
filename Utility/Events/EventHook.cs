using System;
using System.Reflection;

namespace Starship.Core.Utility.Events {
    public class EventHook : IDisposable {

        public EventHook(object source, EventInfo eventInfo, Action<EventItem> callback) {
            Source = source;
            Event = eventInfo;
            Callback = callback;
            Interceptor = InterceptEvent;

            Event.AddEventHandler(Source, Interceptor);
        }

        public void Dispose() {
            Event.RemoveEventHandler(Source, Interceptor);
        }

        private void InterceptEvent(object eventObject) {
            Callback(new EventItem {
                EventObject = eventObject,
                EventTime = DateTime.UtcNow
            });
        }

        private Action<object> Interceptor { get; set; }

        private Action<EventItem> Callback { get; set; }

        private EventInfo Event { get; set; }

        private object Source { get; set; }
    }
}