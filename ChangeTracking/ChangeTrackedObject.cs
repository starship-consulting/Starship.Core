using System;
using System.ComponentModel;

namespace Starship.Core.ChangeTracking {
    public abstract class ChangeTrackedObject : INotifyPropertyChanged {
        protected ChangeTrackedObject() {
            Tracker = new ChangeTracker<ChangeTrackedObject>(this);
        }

        public void Edit(Action editAction) {
            editAction();

            if (PropertyChanged != null) {
                var changeset = Tracker.GetChanges();

                if (changeset.HasChanges) {
                    foreach (var change in changeset.Changes) {
                        PropertyChanged(this, new PropertyChangedEventArgs(change.Property.Name));
                    }

                    if (Edited != null) {
                        Edited();
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action Edited;

        private ChangeTracker<ChangeTrackedObject> Tracker { get; set; }
    }
}