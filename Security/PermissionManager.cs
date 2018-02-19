using System;
using System.Collections.Generic;

namespace Starship.Core.Security {
    public class PermissionManager {

        public PermissionManager() {
        }

        public PermissionTypes Get(Type type, string key) {
            if (Permissions.ContainsKey(type) && Permissions[type].ContainsKey(key)) {
                return Permissions[type][key].Type;
            }

            return PermissionTypes.None;
        }

        public void Set(Type type, PermissionTypes permissionType, string key) {
            if (!Permissions.ContainsKey(type)) {
                Permissions.Add(type, new Dictionary<string, Permission>());
            }

            var permission = Permissions[type];

            if (!permission.ContainsKey(key)) {
                permission.Add(key, new Permission());
            }

            permission[key].Type = permissionType;
        }

        private Dictionary<Type, Dictionary<string, Permission>> Permissions { get; set; }
    }
}