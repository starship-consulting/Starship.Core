using System;
using System.Collections.Generic;

namespace Starship.Core.Collections {
    public class NestedDictionary<T1, T2, T3> {
        public NestedDictionary() {
            InnerDictionary = new Dictionary<T1, Dictionary<T2, T3>>();
        }

        public T3 Add(T1 value1, T2 value2, T3 value3) {
            if (!InnerDictionary.ContainsKey(value1)) {
                InnerDictionary.Add(value1, new Dictionary<T2, T3>());
            }

            InnerDictionary[value1].Add(value2, value3);
            return value3;
        }

        public T3 Get(T1 key1, T2 key2) {
            if (InnerDictionary.ContainsKey(key1) && InnerDictionary[key1].ContainsKey(key2)) {
                return InnerDictionary[key1][key2];
            }

            return default(T3);
        }

        private Dictionary<T1, Dictionary<T2, T3>> InnerDictionary { get; set; }
    }

    /*public class NestedDictionary<KEY, VALUE> {
        public NestedDictionary() {
            InnerDictionary = new Dictionary<KEY, NestedDictionary<KEY, VALUE>>();
        }

        public VALUE Add(KEY key, VALUE value) {
            if (!InnerDictionary.ContainsKey(key)) {
                InnerDictionary.Add(key, new NestedDictionary<KEY, VALUE>());
            }

            InnerDictionary[key].Add(value2, value3);
            return value3;
        }

        public T3 Get(KEY key1, T2 key2) {
            if (InnerDictionary.ContainsKey(key1) && InnerDictionary[key1].ContainsKey(key2)) {
                return InnerDictionary[key1][key2];
            }

            return default(T3);
        }

        private Dictionary<KEY, NestedDictionary<KEY, VALUE>> InnerDictionary { get; set; }
    }*/
}
 