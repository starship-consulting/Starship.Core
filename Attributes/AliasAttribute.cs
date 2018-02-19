using System;

namespace Starship.Core.Attributes {
    public class AliasAttribute : Attribute {
        public AliasAttribute(string value) {
            Value = value;
        }

        public string Value { get; set; }
    }
}