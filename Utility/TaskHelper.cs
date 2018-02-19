using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Starship.Core.Utility {
    public static class TaskHelper {

        public static List<Task> RunAndWait(params Action[] actions) {
            var tasks = new List<Task>();
            tasks.AddRange(actions.Select(Task.Run));

            foreach (var task in tasks) {
                task.Wait();
            }

            return tasks;
        }
    }
}