using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Starship.Core.Processing;
using Starship.Core.Utility;

namespace Starship.Core.ChangeTracking {
    public class ChangeTracker<T> : Disposable {

        static ChangeTracker() {
            PropertyCache = new Dictionary<Type, List<PropertyInfo>>();
        }

        public ChangeTracker(T target) {
            Target = target;

            var type = Target.GetType();

            lock (PropertyCache) {
                if (!PropertyCache.ContainsKey(type)) {
                    PropertyCache.Add(type, type.GetProperties().Select(each => each).ToList());
                }
            }

            Properties = PropertyCache[type].Select(each => new ChangeTrackerProperty(each)).ToList();
            GetChanges();
        }

        public override void Disposed() {
            StopPolling();
        }

        public void StartPolling(TimeSpan interval, Action updateAction = null) {
            StopPolling();

            UpdateAction = updateAction;
            Worker = new Worker(interval, OnPoll);
            Worker.Start();
        }
        
        public void StopPolling() {
            if (Worker != null) {
                Worker.Dispose();
                Worker = null;
                UpdateAction = null;
            }
        }

        private void OnPoll() {
            if (UpdateAction != null) {
                UpdateAction();
            }

            var changes = GetChanges();

            if (changes.HasChanges && Changed != null) {
                Changed(Target, changes);
            }
        }

        public ChangeTrackerState GetChanges() {
            return new ChangeTrackerState(Properties.Where(each => each.HasChanged(Target)).ToList());
        }

        public event Action<T, ChangeTrackerState> Changed;

        public T Target { get; set; }

        private List<ChangeTrackerProperty> Properties { get; set; }

        private Worker Worker { get; set; }

        private Action UpdateAction { get; set; }

        private static Dictionary<Type, List<PropertyInfo>> PropertyCache { get; set; }
    }
}