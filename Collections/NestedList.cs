using System;
using System.Collections;
using System.Collections.Generic;

namespace Starship.Core.Collections {
    public class NestedList<T> {

        public NestedList() {
        }

        public void Add(T item) {
            if (InnerList == null) {
                InnerList = new NestedList<T>();
            }

            InnerList.Add(item);
        }

        //public IEnumerable<T> ForEach() {
            
        //}

        //public bool Exists() {
            
        //}

        private NestedList<T> InnerList { get; set; }
    }
}