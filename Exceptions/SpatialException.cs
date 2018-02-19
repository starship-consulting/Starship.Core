using System;

namespace Starship.Core.Exceptions {
    public class SpatialException : Exception {
        public SpatialException(string message) : base(message) {
        }
    }
}