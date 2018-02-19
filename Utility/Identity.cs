using System.Collections.Concurrent;

namespace Starship.Core.Utility {
    public static class Identity {

        static Identity() {
            Cache = new ConcurrentDictionary<string, int>();
        }

        public static int Next(string key) {
            return Cache.AddOrUpdate(key, 1, (b, c) => c + 1);
        }

        public static int Next<T>() {
            return Cache.AddOrUpdate(typeof(T).Name, 1, (b, c) => c + 1);
        }

        private static ConcurrentDictionary<string, int> Cache { get; set; }
    }
}