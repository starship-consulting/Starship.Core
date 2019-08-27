using System.Linq;
using System.Threading.Tasks;
using Starship.Core.Interfaces;

namespace Starship.Core.Data {
    public interface IsDataProvider {

        T Add<T>(T entity) where T : HasId;

        void Save();

        Task SaveAsync();

        IQueryable<T> Get<T>();
    }

    public static class IsDataProviderExtensions {

        public static T Save<T>(this IsDataProvider provider, T entity) where T : HasId {
            provider.Add(entity);
            provider.Save();
            return entity;
        }
    }
}