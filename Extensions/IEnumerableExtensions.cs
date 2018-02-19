using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Starship.Core.Extensions {
    public static class IEnumerableExtensions {
        public static List<T2> ForEachParallel<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> action) {
            var items = source.ToList();
            var results = new List<T2>();

            Parallel.ForEach(Partitioner.Create(0, items.Count), (range, state) => {
                var finished = new List<T2>();

                for (int i = range.Item1; i < range.Item2; i++) {
                    var item = items[i];
                    var returned = action(item);

                    if (returned != null) {
                        finished.Add(returned);
                    }
                }

                lock (results) {
                    results.AddRange(finished);
                }
            });

            return results;
        }

        public static void ForEachParallel<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> action, Action<List<T2>> after) {
            var items = source.ToList();

            Parallel.ForEach(Partitioner.Create(0, items.Count), (range, state) => {
                var results = new List<T2>();

                for (int i = range.Item1; i < range.Item2; i++) {
                    var item = items[i];
                    var returned = action(item);

                    if (returned != null) {
                        results.Add(action(item));
                    }
                }

                after(results);
            });
        }

        public static void ForEachParallel<T>(this IEnumerable<T> source, Action<T> action) {
            var items = source.ToList();

            Parallel.ForEach(Partitioner.Create(0, items.Count), (range, state) => {
                for (int i = range.Item1; i < range.Item2; i++) {
                    action(items[i]);
                }
            });
        }

        public static IEnumerable<Tuple<T, T>> ForEachPermutation<T>(this IEnumerable<T> source) {
            var list = source.ToList();

            foreach (var item1 in list) {
                foreach (var item2 in list) {
                    yield return new Tuple<T, T>(item1, item2);
                }
            }
        }

        public static IEnumerable<Tuple<T1, T2>> ForEachPermutation<T1, T2>(this IEnumerable<T1> source, IEnumerable<T2> target) {
            foreach (var item1 in source) {
                foreach (var item2 in target) {
                    yield return new Tuple<T1, T2>(item1, item2);
                }
            }
        }

        public static void IterateWith<T1, T2>(this IEnumerable<T1> source, IEnumerable<T2> target, Action<T1, T2> action) {
            var list1 = source.ToList();
            var list2 = target.ToList();

            for (var index = 0; index < list1.Count; index++) {
                action(list1[index], list2[index]);
            }
        }

        public static Type GetGenericType(this Type type) {
            if (type.GenericTypeArguments.Any()) {
                return type.GenericTypeArguments.FirstOrDefault();
            }

            return type.BaseType.GenericTypeArguments.FirstOrDefault();
        }

        public static IEnumerable<List<T>> Partition<T>(this IEnumerable<T> source, int size) {
            for (int i = 0; i < System.Math.Ceiling(source.Count()/(double) size); i++) {
                yield return new List<T>(source.Skip(size*i).Take(size));
            }
        }

        public static Type GetGenericType(this object target) {
            return target.GetType().GetGenericType();
        }

        public static Array ToArray(this IEnumerable source, Type type) {
            var param = Expression.Parameter(typeof(IEnumerable), "source");
            var cast = Expression.Call(typeof(Enumerable), "Cast", new[] {type}, param);
            var toArray = Expression.Call(typeof(Enumerable), "ToArray", new[] {type}, cast);
            var lambda = Expression.Lambda<Func<IEnumerable, Array>>(toArray, param).Compile();

            return lambda(source);
        }

        public static IList ToListOf<T>(this List<T> data, Type type) {
            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;

            foreach (var item in data) {
                list.Add(item);
            }

            return list;
        }

        public static IList ToListOf(this IEnumerable data, Type type) {
            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;

            foreach (var item in data) {
                list.Add(item);
            }

            return list;
        }
    }
}