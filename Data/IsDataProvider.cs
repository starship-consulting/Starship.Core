using System.Linq;
using System.Threading.Tasks;

namespace Starship.Core.Data {
    public interface IsDataProvider {

        T Add<T>(T entity);

        void Save();

        Task SaveAsync();

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