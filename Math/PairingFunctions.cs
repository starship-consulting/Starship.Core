using MathNet.Spatial.Euclidean;

namespace Starship.Core.Math {
    public static class PairingFunctions {

        public static ulong CantorHash(Point2D point) {
            return CantorHash(point.X, point.Y);
        }

        public static ulong CantorHash(double a, double b) {
            var A = (ulong) (a >= 0 ? 2*(long) a : -2*(long) a - 1);
            var B = (ulong) (b >= 0 ? 2*(long) b : -2*(long) b - 1);

            return (A + B)*(A + B + 1)/2 + A;
        }

        public static long SzudzikHash(Point2D point) {
            return SzudzikHash(point.X, point.Y);
        }

        public static long SzudzikHash(double a, double b) {
            var A = (ulong) (a >= 0 ? 2*(long) a : -2*(long) a - 1);
            var B = (ulong) (b >= 0 ? 2*(long) b : -2*(long) b - 1);
            var C = (long) ((A >= B ? A*A + A + B : A + B*B)/2);

            return a < 0 && b < 0 || a >= 0 && b >= 0 ? C : -C - 1;
        }
    }
}