using System;

namespace Starship.Core.Csv {

    public class CsvFormatAttribute : Attribute {

        public CsvFormatAttribute() {
        }

        public CsvFormatAttribute(bool quotationDelimitedStrings) {
            QuotationDelimitedStrings = quotationDelimitedStrings;
        }

        public bool QuotationDelimitedStrings { get; set; }
    }
}