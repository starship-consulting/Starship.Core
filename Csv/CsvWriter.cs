using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Starship.Core.Extensions;

namespace Starship.Core.Csv {
    public class CsvWriter {

        public string Write(IEnumerable collection) {

            var result = new StringBuilder();

            foreach (var item in collection) {
                var index = 0;
                var quotationDelimitedStrings = false;

                item.GetType().WithAttribute<CsvFormatAttribute>(attribute => quotationDelimitedStrings = attribute.QuotationDelimitedStrings);

                foreach (var property in item.GetType().GetProperties()) {
                    if (index > 0) {
                        result.Append(",");
                    }

                    var propertyValue = property.GetValue(item);

                    if (quotationDelimitedStrings) {
                        result.Append("\"");
                    }

                    result.Append(propertyValue);

                    if (quotationDelimitedStrings) {
                        result.Append("\"");
                    }

                    index += 1;
                }

                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }
    }
}