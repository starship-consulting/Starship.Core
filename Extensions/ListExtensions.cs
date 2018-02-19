using System;
using System.Collections.Generic;
using System.Linq;

namespace Starship.Core.Extensions {
    public static class ListExtensions {

        public static void ForEachReverse<T>(this IList<T> source, Action<T> action) {
            for (int count = source.Count - 1; count >= 0; count--) {
                action(source[count]);
            }
        }

        public static List<string> ToStringList<T>(this IEnumerable<T> input) {
            return input.Select(each => each.ToString()).ToList();
        } 

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> fnRecurse) {
            foreach (T item in source) {
                yield return item;

                IEnumerable<T> seqRecurse = fnRecurse(item);
                if (seqRecurse != null) {
                    foreach (T itemRecurse in Traverse(seqRecurse, fnRecurse)) {
                        yield return itemRecurse;
                    }
                }
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this List<TSource> source, Func<TSource, TKey> keySelector) {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }

        public static List<T> Shuffle<T>(this List<T> list) {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}