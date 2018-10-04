using System;

namespace Starship.Core.Time {

    public static class TimeProvider {

        private static DateTime? _forcedNow;

        public static DateTime Now {
            get { return (_forcedNow ?? DateTime.UtcNow) + Offset; }
        }

        public static DateTime StartOfMonthTime {
            get { return new DateTime(Now.Year, Now.Month, 1); }
        }

        public static void Forward(TimeSpan time) {
            Offset += time;
        }

        public static void Set(DateTime time) {
            _forcedNow = null;
            Offset = time - DateTime.UtcNow;
        }

        public static void NowIs(DateTime time) {
            _forcedNow = time;
            Offset = TimeSpan.Zero;
        }

        public static void Reset() {
            Offset = TimeSpan.Zero;
            _forcedNow = null;
        }

        public static TimeSpan Offset { get; set; }
    }
}