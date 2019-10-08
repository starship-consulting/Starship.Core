using System;
using System.Reflection;

namespace Starship.Core.EventSourcing {
    public class EventSink {

        public EventSink(Type eventType, Type aggregateType, string methodName) {
            EventType = eventType;
            AggregateType = aggregateType;
            ApplyMethod = EventType.GetMethod(methodName);
        }

        public void Apply(object eventObject, object aggregateObject) {
            ApplyMethod.Invoke(eventObject, new [] { aggregateObject });
        }

        public Type EventType { get; set; }

        public Type AggregateType { get; set; }

        private MethodInfo ApplyMethod { get; set; }
    }
}