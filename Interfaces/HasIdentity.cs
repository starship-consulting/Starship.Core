using Starship.Core.Extensions;

namespace Starship.Core.Interfaces {
    public interface BaseIdentity {
    }

    public interface HasIdentity : HasIdentity<int> {
    }

    public interface HasIdentity<T> : BaseIdentity {
        T Id { get; set; }
    }

    public static class BaseIdentityExtensions {
        public static bool HasId(this BaseIdentity target, object id) {
            return Equals(target.GetId(), id);
        }

        public static object GetId(this BaseIdentity target) {
            return target.GetPropertyValue("Id");
        }
    }
}