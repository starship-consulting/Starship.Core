using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starship.Core.Extensions {
    public static class TaskExtensions {

        public static T GetResult<T>(this Task<T> task) {
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static Task Then(this Task first, Task second) {
            return Then(first,
                () => {
                    second.Start();
                    second.Wait();
                });
        }

        public static Task Then(this Task first, Action second) {
            return new Task(
                () => {
                    if (first.Status == TaskStatus.Created) first.Start();
                    first.Wait();
                    second();
                });
        }

        public static Task Then<T>(this Task first, Task<T> second) {
            return new Task<T>(
                () => {
                    if (first.Status == TaskStatus.Created) first.Start();
                    first.Wait();
                    second.Start();
                    return second.Result;
                });
        }

        public static Task Then<T>(this Task<T> first, Action<T> second) {
            return new Task(
                () => {
                    if (first.Status == TaskStatus.Created) first.Start();
                    var firstResult = first.Result;
                    second(firstResult);
                });
        }

        public static Task<TSecond> Then<TFirst, TSecond>(this Task<TFirst> first, Func<TFirst, TSecond> second) {
            return new Task<TSecond>(
                () => {
                    if (first.Status == TaskStatus.Created) first.Start();
                    return second(first.Result);
                });
        }
    }
}