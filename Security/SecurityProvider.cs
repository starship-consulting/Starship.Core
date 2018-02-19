using System;
using System.Collections.Concurrent;

namespace Starship.Core.Security {
    public class SecurityProvider {

        public SecurityProvider() {
            Contexts = new ConcurrentDictionary<string, PermissionContext>();
        }

        public void Set(string id, Type type) {
            Set(id, type, PermissionTypes.Full, string.Empty);
        }

        public void Set(string id, Type type, PermissionTypes permission, string key) {
            var context = Contexts.GetOrAdd(id, factory => new PermissionContext());

            context.Set(type, permission, key);
        }

        public PermissionTypes Get(string id, Type type, string key = null) {
            PermissionContext context;
            Contexts.TryGetValue(id, out context);

            if (context == null) {
                return PermissionTypes.None;
            }

            return context.Get(type, key);
        }

        public PermissionContext Remove(string id) {
            PermissionContext context;
            Contexts.TryRemove(id, out context);
            return context;
        }

        private ConcurrentDictionary<string, PermissionContext> Contexts { get; set; }
    }
}