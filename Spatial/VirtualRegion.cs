using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Starship.Core.Spatial {
    public class VirtualRegion {

        public VirtualRegion() {
            Entities = new ConcurrentDictionary<string, SpatialPoint>();
        }

        public List<SpatialPoint> GetEntities(int left, int top, int right, int bottom) {
            var points = new List<SpatialPoint>();

            foreach (var entity in Entities) {
                if (entity.Value.X >= left && entity.Value.Y >= top && entity.Value.X <= right && entity.Value.Y <= bottom) {
                    points.Add(entity.Value);
                }
            }

            return points;
        }

        public bool Update(SpatialPoint point) {
            return Entities.TryAdd(point.Id, point);
        }

        public SpatialPoint Remove(string id) {
            SpatialPoint point;
            Entities.TryRemove(id, out point);
            return point;
        }

        private ConcurrentDictionary<string, SpatialPoint> Entities { get; set; }
    }
}