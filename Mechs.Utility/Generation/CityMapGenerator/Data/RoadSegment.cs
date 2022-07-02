using System.Numerics;

namespace Mechs.Utility.Generation.CityMapGenerator.Data
{
    public enum DirectionEnum : int
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
    }

    public class RoadSegment
    {
        private const int MaxGrowSize = Int32.MaxValue;
        public static Vector2[] DirectionVectors = new Vector2[]
        {
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(-1, 0),
        };

        public MapArea Area { get; set; }
        public float RoadWidth
        {
            get
            {
                return Direction switch
                {
                    DirectionEnum.North => Area.Size.X,
                    DirectionEnum.East => Area.Size.Y,
                    DirectionEnum.South => Area.Size.X,
                    DirectionEnum.West => Area.Size.Y,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public DirectionEnum Direction { get; set; }

        public RoadSegment(MapArea area, DirectionEnum direction)
        {
            Area = area;
            Direction = direction;
        }

        /// <summary>
        /// Grow the road in its specified direction by this magnitude
        /// </summary>
        /// <param name="magnitude"></param>
        public void Resize(int magnitude)
        {
            var growVector = DirectionVectors[(int)Direction] * magnitude;

            var newSize = new Vector2(Area.Size.X, Area.Size.Y);
            var newPosition = new Vector2(Area.Position.X, Area.Position.Y);
            if (growVector.X < 0)
            {
                newPosition.X += growVector.X;
                newSize.X -= growVector.X;
            }
            else if (growVector.X > 0)
            {
                newSize.X += growVector.X;
            }

            if (growVector.Y < 0)
            {
                newPosition.Y += growVector.Y;
                newSize.Y -= growVector.Y;
            }
            else if (growVector.Y > 0)
            {
                newSize.Y += growVector.Y;
            }

            Area.Position = newPosition;
            Area.Size = newSize;
        }

        /// <summary>
        /// Get the number of tiles this road can grow before hitting the edge of the map or another road
        /// </summary>
        /// <param name="roadSegments"></param>
        public int GetMaxGrowLength(List<RoadSegment> roadSegments, Vector2 mapSize)
        {
            var growVector = DirectionVectors[(int)Direction];
            int growSize = 0;
            var currentPos = Area.Position;
            do
            {
                currentPos += growVector;

                if (IsComplete(currentPos))
                {
                    return growSize;
                }

                growSize++;
            } while (growSize < MaxGrowSize);

            return growSize;

            ////////////

            bool IsComplete(Vector2 pos)
            {
                // Is out of bounds
                if (pos.X < 0 || pos.Y < 0 || pos.X + (RoadWidth - 1) >= mapSize.X || pos.Y + (RoadWidth - 1) >= mapSize.Y)
                {
                    return true;
                }

                if (roadSegments.Any(x => x.Area.Intersects(pos)))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Split road into two segments
        /// </summary>
        /// <returns></returns>
        public RoadSegment[] SplitRoadInTwo()
        {
            var results = new List<RoadSegment>();
            var random = new Random();
            var directions = Enum.GetValues<DirectionEnum>()
                .Where(x => x != Direction)
                .OrderBy(x => random.Next())
                .Take(2);

            foreach (var direction in directions)
            {
                // Move the road in the appropriate direction
                var offsetVector = DirectionVectors[(int)direction] * RoadWidth;

                // Requires double movement in negative directions
                //if (direction == DirectionEnum.North || direction == DirectionEnum.West)
                //{
                //    offsetVector *= 2;
                //}

                var newPosition = Area.Position + offsetVector;

                var segment = new RoadSegment(new MapArea(newPosition, newPosition + new Vector2(RoadWidth, RoadWidth)), direction);
                results.Add(segment);
            }

            return results.ToArray();
        }
    }
}
