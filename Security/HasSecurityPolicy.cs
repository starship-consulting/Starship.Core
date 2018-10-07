using System;
using System.Linq;

namespace Starship.Core.Security
{
    public interface HasSecurityPolicy {
    }

    public interface HasSecurityPolicy<T> : HasSecurityPolicy {
        IQueryable<T> ApplySecurity(IQueryable<T> source, AccessTypes permissionType);
    }

    public static class HasSecurityPolicyExtensions {
        
        public static IQueryable ApplySecurity(this HasSecurityPolicy policy, IQueryable source, AccessTypes permission) {
            var type = policy.GetType();
            var method = type.GetMethod("ApplySecurity");
            var result = method.Invoke(policy, new object[] { source, permission });
            return (IQueryable)result;
        }
    }
}
