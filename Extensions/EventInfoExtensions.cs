using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Starship.Core.Reflection;

namespace Starship.Core.Extensions {
    public static class EventInfoExtensions {
        public static void InvokeStaticEvent(this object source, string name, params object[] parameters) {
            var field = source.GetType().GetField(name, AllBindings);

            if (field == null) {
                throw new Exception("Unknown event.");
            }

            var eventDelegate = field.GetValue(null) as Delegate;

            if (eventDelegate == null) {
                throw new Exception("Unknown event: " + source.GetType().Name + "." + name);
            }

            foreach (var handler in eventDelegate.GetInvocationList()) {
                if (handler.Method.IsDynamic()) {
                    continue;
                }

                var invoker = MethodInvoker.GetDefault(handler.Method);
                invoker.Invoke(handler.Target, parameters.ToList());
            }
        }

        public static List<Delegate> ExtractDelegates(this EventInfo eventInfo) {
            var delegates = new List<Delegate>();
            var events = GetEvents(eventInfo);

            if (events != null) {
                foreach (var each in events) {
                    delegates.Add(each);
                    eventInfo.RemoveEventHandler(null, each);
                }
            }

            return delegates;
        }

        public static List<Delegate> GetEvents(this EventInfo eventInfo, object source = null) {
            var field = eventInfo.DeclaringType.GetField(eventInfo.Name, AllBindings);
            var delegates = field.GetValue(source) as Delegate;

            if (delegates != null) {
                return delegates.GetInvocationList().ToList();
            }

            return new List<Delegate>();
        }

        public static void ClearEvents(this EventInfo eventInfo, object source = null) {
            var events = GetEvents(eventInfo, source);

            if (events != null) {
                foreach (var each in events) {
                    eventInfo.RemoveEventHandler(null, each);
                }
            }
        }

        public static void InsertEvent(this EventInfo eventInfo, Delegate del) {
            var field = eventInfo.DeclaringType.GetField(eventInfo.Name, AllBindings);
            var delegates = field.GetValue(null) as Delegate;

            if (delegates != null) {
                var invocationList = delegates.GetInvocationList();

                foreach (var ev in invocationList) {
                    eventInfo.RemoveEventHandler(null, ev);
                }

                eventInfo.AddEventHandler(null, del);

                foreach (var ev in invocationList) {
                    eventInfo.AddEventHandler(null, ev);
                }
            }
            else {
                eventInfo.AddEventHandler(null, del);
            }
        }

        public static void AddDynamicHandler(this EventInfo eventInfo, Action<object[]> callback) {
            var type = eventInfo.EventHandlerType;

            var invokeMethod = type.GetMethod("Invoke");
            var parameters = invokeMethod.GetParameters().Select(parm => Expression.Parameter(parm.ParameterType, parm.Name)).ToList();

            var instance = callback.Target == null ? null : Expression.Constant(callback.Target);
            var converted = parameters.Select(each => Expression.Convert(each, typeof (object))).ToList();
            converted.Insert(0, Expression.Convert(Expression.Constant(eventInfo), typeof (object)));

            var call = Expression.Call(instance, callback.Method, Expression.NewArrayInit(typeof (object), converted));
            var lambda = Expression.Lambda(type, call, parameters);

            eventInfo.InsertEvent(lambda.Compile());
        }

        private const BindingFlags AllBindings = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    }
}