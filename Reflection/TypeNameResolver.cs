using System;
using System.Collections.Generic;
using System.Linq;
using Starship.Core.Extensions;

namespace Starship.Core.Reflection {
    public class TypeNameResolver {

        public TypeNameResolver() {
            CaseInsensitive = true;
            AllowPlural = true;
        }

        public Type FindType(string typeName) {
            var type = TypeBinding().FirstOrDefault(each => each.GetTypeNames().Any(name => string.Equals(name, typeName, CaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)));

            if (AllowPlural) {
                if (type == null && typeName.ToLower().EndsWith("s")) {
                    return FindType(typeName.Substring(0, typeName.Length - 1));
                }
            }

            return type;
        }

        public bool AllowPlural { get; set; }

        public bool CaseInsensitive { get; set; }

        public Func<IEnumerable<Type>> TypeBinding { get; set; }
    }
}