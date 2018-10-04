using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Starship.Core.Extensions {
    public static class ResourceManagerExtensions {

        public static Dictionary<string, string> Read(this ResourceManager manager) {
            var results = new Dictionary<string, string>();
            var resourceSet = manager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            foreach (DictionaryEntry entry in resourceSet) {
                results.Add(entry.Key.ToString(), entry.Value.ToString());
            }

            return results;
        }
    }
}