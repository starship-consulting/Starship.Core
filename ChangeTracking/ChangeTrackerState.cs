using System;
using System.Collections.Generic;
using System.Linq;

namespace Starship.Core.ChangeTracking {
    public class ChangeTrackerState {

        public ChangeTrackerState(List<ChangeTrackerProperty> changes) {
            Changes = changes;
        }

        public List<ChangeTrackerProperty> Changes { get; set; }

        public bool HasChanges => Changes.Any();
    }
}