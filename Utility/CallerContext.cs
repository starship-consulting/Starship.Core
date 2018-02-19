using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Starship.Core.Utility {
    public static class CallerContext {
        static CallerContext() {
            Callers = new Dictionary<string, MethodContext>();
        }

        public static MethodInfo GetCallingMethod(string caller, string path) {
            return GetMethodContext(caller, path).Method;
        }

        public static List<T> GetCallerAttributes<T>(string caller, string path) where T : Attribute {
            return GetMethodContext(caller, path).MethodAttributes.OfType<T>().ToList();
        }

        public static List<T> GetParameterAttributes<T>(string caller, string path) where T : Attribute {
            return GetMethodContext(caller, path).MethodAttributes.OfType<T>().ToList();
        }

        private static MethodContext GetMethodContext(string caller, string path) {
            var key = caller + path;

            lock (Callers) {
                if (!Callers.ContainsKey(key)) {
                    var context = new MethodContext();

                    var filename = path.Split('\\').Last().Replace(".cs", "");
                    var type = ReflectionCache.Lookup(filename);
                    var method = type.GetMethod(caller);

                    if (method != null) {
                        var attributes = method.GetCustomAttributes(true);

                        context.Method = method;
                        context.MethodAttributes = attributes.Cast<Attribute>().ToList();
                    }

                    Callers.Add(key, context);
                }

                return Callers[key];
            }
        }

        private static Dictionary<string, MethodContext> Callers { get; set; }

        internal class MethodContext {
            public MethodContext() {
                MethodAttributes = new List<Attribute>();
            }

            public MethodInfo Method { get; set; }

            public List<Attribute> MethodAttributes { get; set; }
        }
    }
}