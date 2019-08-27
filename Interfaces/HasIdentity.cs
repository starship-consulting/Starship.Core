using Starship.Core.Extensions;

namespace Starship.Core.Interfaces {
    public interface HasId {
        string GetId();
        void SetId(object value);
    }

    public interface HasIdentity<T> : HasId {
        T Id { get; set; }
    }

    public interface HasIdentity : HasIdentity<int> {
    }

    public static class BaseIdentityExtensions {
        public static bool HasId(this HasId target, object id) {
            return Equals(target.GetId(), id);
        }

        /*public static object GetId(this BaseIdentity target) {
            return target.GetPropertyValue("Id");
        }*/
    }
}