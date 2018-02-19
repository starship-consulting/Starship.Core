using System;

namespace Starship.Core.Events.Standard {
    public class EntityModified {
        public EntityModified() {
        }

        public EntityModified(object entity) {
            Entity = entity;
        }

        public object Entity { get; set; }
    }
}