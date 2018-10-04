using Newtonsoft.Json.Serialization;

namespace Starship.Core.Json
{
    public class NullStringValueProvider : IValueProvider {

        public NullStringValueProvider(IValueProvider provider) {
            Provider = provider;
        }

        public object GetValue(object target) {
            return Provider.GetValue(target) ?? "";
        }

        public void SetValue(object target, object value) {
            Provider.SetValue(target, value);
        }

        private readonly IValueProvider Provider;
    }
}
