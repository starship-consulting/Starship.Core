using System;
using System.Timers;

namespace Starship.Core.Scheduling {
    public class Schedule {

        public Schedule(TimeSpan every) {
            Every = every;
        }

        public Timer GetTimer() {
            var timer = new Timer(Every.TotalMilliseconds);
            timer.AutoReset = true;

            return timer;
        }

        public TimeSpan Every { get; set; }
    }
}