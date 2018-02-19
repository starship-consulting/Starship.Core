using System;
using System.Reflection;

namespace Starship.Core.Models {
    public struct ExtractedProperty<T> {
        public PropertyInfo Property { get; set; }

        public T Value { get; set; }
    }
}