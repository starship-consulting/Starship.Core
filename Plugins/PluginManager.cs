using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Starship.Core.Processing;

namespace Starship.Core.Plugins {
    public class PluginManager {

        public PluginManager(List<Type> pluginTypes) {
            Plugins = new List<Plugin>();
            PluginTypes = pluginTypes;
            EventRouter = new TypeRouter();
        }

        public void With<T>(Action<T> action) where T : Plugin {
            var plugin = (T) Plugins.FirstOrDefault(each => each is T);

            if(plugin != null) {
                action(plugin);
            }
        }

        public void Load(string jsonConfiguration) {
            JsonConfiguration = jsonConfiguration;

            var plugins = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(JsonConfiguration);

            foreach (var plugin in plugins) {
                var type = PluginTypes.FirstOrDefault(each => each.Name == plugin.Key + "Plugin");

                if (type == null) {
                    throw new Exception("Invalid plugin: " + plugin.Key);
                }

                var instance = plugin.Value.ToObject(type) as Plugin;

                Plugins.Add(instance);
            }
        }

        public void Start() {
            foreach (var plugin in Plugins) {
                plugin.Ready();
            }

            foreach (var plugin in Plugins) {
                plugin.Start();
            }
        }
        
        private TypeRouter EventRouter { get; set; }

        private List<Plugin> Plugins { get; }

        private List<Type> PluginTypes { get; }

        private string JsonConfiguration { get; set; }
    }
}