using System;

namespace Starship.Core.Contexts {
    public interface IsAppContextProvider {
        void Clear();
        void Set(string key, object value);
        object Get(string key);
        void Dispose(string key);
    }
}