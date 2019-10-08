using System;
using System.Collections.Generic;

namespace Starship.Core.EventSourcing {

    public static class EventSinkRegistrar {

        static EventSinkRegistrar() {
            Events = new Dictionary<string, EventSink>();
        }

        public static void Register(Type eventType, Type aggregateType, string methodName) {
            Events.Add(eventType.Name.ToLower(), new EventSink(eventType, aggregateType, methodName));
        }

        public static EventSink GetEventSink(string eventName) {
            eventName = eventName.ToLower();

            if (Events.ContainsKey(eventName.ToLower())) {
                return Events[eventName];
            }

            return null;
        }

        private static Dictionary<string, EventSink> Events { get; set; }
    }
}