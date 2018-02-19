using System.Collections.Concurrent;

namespace Starship.Core.IoC {
    public class TypeContainer {
        public TypeContainer() {
            Items = new ConcurrentDictionary<string, object>();
        }

        public T Model<T>() where T : new() {
            return (T)Items.GetOrAdd("_" + typeof (T).Name, type => new T());
        }

        public T Model<T>(string id) where T : new() {
            return (T)Items.GetOrAdd(id, type => new T());
        }

        private ConcurrentDictionary<string, object> Items { get; set; } 
    }
}