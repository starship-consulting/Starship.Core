using System.Linq;

namespace Starship.Core.Data {
    public interface IsDataProvider {
        T Add<T>(T entity);

        void Save();

        IQueryable<T> Get<T>();
    }

    public static class IsDataProviderExtensions {
        public static T Save<T>(this IsDataProvider provider, T entity) {
            provider.Add(entity);
            provider.Save();
            return entity;
        }
    }
}