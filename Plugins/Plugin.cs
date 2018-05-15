using System;
using System.Threading.Tasks;
using Starship.Core.Events;

namespace Starship.Core.Plugins {
    public abstract class Plugin {

        protected Plugin() {
            Name = GetType().ToString();

            if (Name.ToLower().EndsWith("plugin")) {
                Name = Name.Substring(0, Name.Length - 6);
            }

            Status = TaskStatus.WaitingToRun;
        }

        public virtual void Ready() {
        }

        public void Start() {
            Status = TaskStatus.Running;
            Run();
        }

        protected virtual void Run() {
        }

        public void Stop() {
            Status = TaskStatus.Canceled;
            Stopped();
        }

        protected virtual void Stopped() {
        }

        protected void On<T>(Action<T> callback) {
            EventStream.On(callback);
        }

        protected void Publish(object e) {
            EventStream.Publish(e);
        }

        public string Name { get; set; }

        public TaskStatus Status { get; set; }
    }
}