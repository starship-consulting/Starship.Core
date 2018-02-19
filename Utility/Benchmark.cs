using System;
using System.Diagnostics;

namespace Starship.Core.Utility {
    public class Benchmark : IDisposable {
        public Benchmark(string message = "Completed in {0} milliseconds.") {
            StartTime = DateTime.Now;
            Message = message;
        }

        public static T Method<T>(Func<T> method, string message = "Completed in {0} milliseconds.") {
            using (new Benchmark(message)) {
                return method.Invoke();
            }
        }

        public void Dispose() {
            Time = DateTime.Now - StartTime;
            
            Debug.WriteLine(string.Format(Message, Time.TotalMilliseconds));
            Console.WriteLine(string.Format(Message, Time.TotalMilliseconds));
        }

        public TimeSpan Time { get; set; }

        private string Message { get; set; }

        private DateTime StartTime { get; set; }
    }
}