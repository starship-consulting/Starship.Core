using System;
using System.Timers;
using Starship.Core.Utility;

namespace Starship.Core.Processing {
    public class Worker : Disposable {

        public Worker(TimeSpan interval, Action action) {
            Interval = interval;
            Action = action;
        }

        public override void Disposed() {
            Stop();
        }

        public void Start() {
            Stop();

            Timer = new Timer(Interval.TotalMilliseconds);
            Timer.AutoReset = true;
            Timer.Elapsed += OnElapsed;
            Timer.Start();
        }

        private void Stop() {
            if (Timer != null) {
                Timer.Stop();
                Timer.Dispose();
                Action = null;
            }
        }

        private void OnElapsed(object sender, ElapsedEventArgs e) {
            if (IsProcessing) {
                return;
            }

            IsProcessing = true;

            if (Action != null) {
                Action();
            }

            IsProcessing = false;
        }

        private bool IsProcessing { get; set; }

        private Action Action { get; set; }

        private TimeSpan Interval { get; set; }

        private Timer Timer { get; set; }
    }
}