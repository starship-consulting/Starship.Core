using System;
using System.Reflection;

namespace Starship.Core.Reflection {
    public class PropertyInstance {
        public object Value { get; set; }

        public PropertyInfo Property { get; set; }

        public object Owner { get; set; }
    }
}