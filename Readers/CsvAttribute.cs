using System;

namespace Starship.Core.Readers
{
    public class CsvAttribute : Attribute
    {
        public CsvAttribute(int index) {
            Index = index;
        }

        public int Index { get; set; }
    }
}
