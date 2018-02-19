using System;
using System.Collections.Generic;
using System.Linq;

namespace Starship.Core.Utility {
    public class ObjectComparer<T> where T : class {

        public ObjectComparer(params T[] targets) {
            Targets = targets.ToList();
        }

        public void Add(T target) {
            Targets.Add(target);
        }

        public bool Compare(Func<T, T, bool> predicate) {
            foreach (var target1 in Targets) {
                foreach (var target2 in Targets) {
                    if (target1 == target2) {
                        continue;
                    }

                    if (predicate(target1, target2)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<T> Targets { get; set; }
    }
}