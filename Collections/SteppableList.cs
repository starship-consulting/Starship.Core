using System;
using System.Collections.Generic;
using System.Linq;

namespace Starship.Core.Collections {
    public class SteppableList<T> {

        public SteppableList(IEnumerable<T> items) {
            List = items.ToList();
        }

        public bool Step() {
            if (Index < List.Count - 1) {
                Index += 1;
                return true;
            }
            
            return false;
        }

        public int GetDistance(Func<T, bool> predicate) {
            var next = GetNext(predicate);

            if (next != null) {
                return List.IndexOf(next) - Index;
            }

            return 0;
        }

        public T GetNext(Func<T, bool> predicate) {
            return List.Skip(Index).FirstOrDefault(predicate);
        }

        public IEnumerable<T> GetRemaining() {
            return List.Skip(Index);
        }

        public List<T> ToList() {
            return List;
        }

        public T Previous => Index <= 0 ? default(T) : List[Index-1];

        public T Current => List[Index];

        public T Next => Index < List.Count - 1 ? List[Index + 1] : default(T);

        public T NextNext => Index < List.Count - 2 ? List[Index + 2] : default(T);

        public T Last => List[List.Count - 1];

        public T First => List[0];
        
        public int Index { get; set; }

        private List<T> List { get; set; }
    }
}