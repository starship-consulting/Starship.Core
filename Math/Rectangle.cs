using System;

namespace Starship.Core.Math {
    public struct Rectangle {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public int X => Left;
        public int Y => Top;
        public int Width => Right - Left;
        public int Height => Bottom - Top;
    }
}