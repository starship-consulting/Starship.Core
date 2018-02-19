namespace Starship.Core.Spatial {
    public class SpatialPoint {

        public SpatialPoint() {
        }

        public SpatialPoint(string id, int x, int y) {
            Id = id;
            X = x;
            Y = y;
        }

        public string Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}