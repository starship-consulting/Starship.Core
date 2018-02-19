using System;
using System.Linq;

namespace Starship.Core.Utility {

    public class Randomizer {

        public Randomizer() {
            Random = new Random();
        }

        public Randomizer(int seed) {
            Random = new Random(seed);
        }

        public int Number(int minimum, int maximum) {
            return Random.Next(minimum, maximum);
        }

        public string String(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private Random Random { get; set; }
    }
}