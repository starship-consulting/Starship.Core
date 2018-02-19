using System;
using System.Threading.Tasks;
using Starship.Core.Expressions.Interfaces;

namespace Starship.Core.Expressions {
    public class ConversionAdapter<INPUT, OUTPUT> : IExpressionConverter {
        public ConversionAdapter(Func<INPUT, OUTPUT> converter) {
            Converter = converter;
        }

        public async Task<object> ConvertAsync(object input) {

            var source = new TaskCompletionSource<object>();

            var callback = new AsyncCallback(result => source.SetResult(Converter.EndInvoke(result)));

            Converter.BeginInvoke((INPUT) input, callback, null);

            return await source.Task;
        }

        public object Convert(object input) {
            return Converter.Invoke((INPUT)input);
        }

        private Func<INPUT, OUTPUT> Converter { get; set; }
    }
}