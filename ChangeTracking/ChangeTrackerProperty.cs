﻿using System;
using System.Reflection;

namespace Starship.Core.ChangeTracking {
    public class ChangeTrackerProperty {

        public ChangeTrackerProperty(PropertyInfo property) {
            Property = property;
        }

        public bool HasChanged(object source) {
            OldValue = NewValue;
            NewValue = Property.GetValue(source);

            if (OldValue != null) {
                return !OldValue.Equals(NewValue);
            }

            if (NewValue != null) {
                return !NewValue.Equals(OldValue);
            }
            
            return false;
        }

        public PropertyInfo Property { get; set; }

        public object OldValue { get; set; }

        public object NewValue { get; set; }
    }
}