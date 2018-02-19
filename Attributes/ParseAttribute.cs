using System;

namespace Starship.Core.Attributes {
    public class ParseAttribute : Attribute {
        public ParseAttribute(int index, int length) {
            Index = index;
            Length = length;
        }

        public int Index { get; set; }

        public int Length { get; set; }
    }
}