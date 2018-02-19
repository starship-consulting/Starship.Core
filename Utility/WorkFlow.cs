using System;
using System.Collections.Generic;

namespace Starship.Core.Utility {
    public class WorkFlow {
        public WorkFlow() {
            Processes = new Dictionary<Type, MulticastDelegate>();
        }

        public WorkFlow On<T>(Action<T> action) {
            Processes.Add(typeof(T), action);
            return this;
        }

        public WorkFlow On<T>(Func<T, object> action) {
            Processes.Add(typeof(T), action);
            return this;
        }

        public void Process(params object[] source) {
            foreach (var target in source) {
                var targetType = target.GetType();

                foreach (var type in Processes.Keys) {
                    if (type.IsAssignableFrom(targetType)) {
                        var result = Processes[type].DynamicInvoke(target);

                        if (result != null) {
                            Next?.Process(result);
                        }

                        if (FinallyAction != null) {
                            FinallyAction.DynamicInvoke(result ?? target);
                        }

                        return;
                    }
                }
            }
        }

        public WorkFlow Finally<T>(Action<T> action) {
            FinallyAction = action;
            return this;
        }

        public WorkFlow Then(Action<WorkFlow> workflowAction) {
            if (Next == null) {
                Next = new WorkFlow();
                workflowAction(Next);
            }

            return this;
        }

        private Dictionary<Type, MulticastDelegate> Processes { get; set; }

        private MulticastDelegate FinallyAction { get; set; }

        private WorkFlow Next { get; set; }
    }
}