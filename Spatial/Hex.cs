using System;
using System.Collections.Generic;

namespace Starship.Core.Spatial {

    public class Hex {
        public int Width { get; set; }

        public int Height { get; set; }

        public Location Location { get; set; }

        public List<Location> GetVertices() {
            var vertices = new List<Location>();

            for (var i = 0; i < 6; i++)
            {
                var angle = 2 * System.Math.PI / 6 * (i + 0.5);
                var xPosition = Location.X + Width * System.Math.Cos(angle);
                var yPosition = Location.Y + Height * System.Math.Sin(angle);

                vertices.Add(new Location {
                    X = Convert.ToInt32(xPosition),
                    Y = Convert.ToInt32(yPosition)
                });
            }

            return vertices;
        }
    }

    public class HexMap {
        
    }
}