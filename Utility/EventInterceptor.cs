using System.Collections.Generic;
using Starship.Core.Utility.Events;

namespace Starship.Core.Utility {
    public class EventInterceptor {
        public EventInterceptor(object source) {
            Source = source;
            Hooks = new List<EventHook>();
            EventHistory = new Queue<EventItem>();
        }

        public void BeginListening() {
            lock (Hooks) {
                foreach (var eachEvent in Source.GetType().GetEvents()) {
                    Hooks.Add(new EventHook(Source, eachEvent, InterceptEvent));
                }
            }
        }

        public void StopListening() {
            lock (Hooks) {
                Hooks.ForEach(each => each.Dispose());
            }
        }

        private void InterceptEvent(EventItem item) {
            lock (Hooks) {
                EventHistory.Enqueue(item);
            }
        }

        public Queue<EventItem> EventHistory { get; set; }

        private object Source { get; set; }

        private List<EventHook> Hooks { get; set; }
    }
}