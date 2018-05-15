namespace Starship.Core.DependencyInjection {
    public interface IsDependencyProvider {
        void Clear();
        void Set(string key, object value);
        object Get(string key);
        void Dispose(string key);
    }
}