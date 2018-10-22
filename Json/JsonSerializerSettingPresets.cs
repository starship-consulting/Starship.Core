using System;
using Newtonsoft.Json;

namespace Starship.Core.Json {
    public static class JsonSerializerSettingPresets {

        public static JsonSerializerSettings Minimal => new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            ContractResolver = new ConfigurableJsonContractResolver { UseCamelCase = false, ShowEmptyCollections = false },
            //DefaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling = NullValueHandling.Ignore
            //NullValueHandling = NullValueHandling.Ignore,
            //DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
        };
    }
}