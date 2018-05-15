using System;

namespace Starship.Core.DependencyInjection {
    public static class DependencyContainer {

        public static IsDependencyProvider Provider { get; set; }

        public static void Clear() {
            if (Provider == null) {
                throw new Exception("AppContext.Provider must be set.");
            }

            Provider.Clear();
        }

        public static void Set(string key, object value) {
            if (Provider == null) {
                throw new Exception("AppContext.Provider must be set.");
            }

            Provider.Set(key, value);
        }

        public static T Get<T>(string key) {
            return (T)Get(key);
        }

        public static object Get(string key) {
            if (Provider == null) {
                throw new Exception("AppContext.Provider must be set.");
            }

            return Provider.Get(key);
        }
    }
}