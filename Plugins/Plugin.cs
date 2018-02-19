using System;
using Starship.Core.Events;

namespace Starship.Core.Plugins {
    public abstract class Plugin {

        protected Plugin() {
            Name = GetType().ToString();

            if (Name.ToLower().EndsWith("plugin")) {
                Name = Name.Substring(0, Name.Length - 6);
            }
        }

        public virtual void Ready() {
        }

        public virtual void Start() {
        }

        public virtual void Stop() {
        }

        protected void On<T>(Action<T> callback) {
            EventStream.On(callback);
        }

        protected void Publish(object e) {
            EventStream.Publish(e);
        }

        public string Name { get; set; }
    }
}