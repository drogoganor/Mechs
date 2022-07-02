using System.Numerics;

namespace Mechs.Utility.Generation.CityMapGenerator.Data
{
    public class MapArea
    {
        public Vector2 TopLeft { get; set; }
        public Vector2 BottomRight { get; set; }
        public Vector2 Size
        {
            get
            {
                return BottomRight - TopLeft;
            }
            set
            {
                BottomRight = TopLeft + value;
            }
        }

        public MapArea()
        {

        }

        public MapArea(Vector2 topLeft, Vector2 bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        public bool Intersects(Vector2 point)
        {
            return point.X >= TopLeft.X && point.X < BottomRight.X && point.Y >= TopLeft.Y && point.Y < BottomRight.Y;
        }

        public Vector2[] AllPositions
        {
            get
            {
                var results = new List<Vector2>();
                for (var y = 0; y < (int)Size.Y; y++)
                {
                    for (var x = 0; x < (int)Size.X; x++)
                    {
                        results.Add(new Vector2(TopLeft.X + x, TopLeft.Y + y));
                    }
                }

                return results.ToArray();
            }
        }

        public Vector2 Position
        {
            get { return TopLeft; }
            set
            {
                var size = Size;
                TopLeft = value;
                BottomRight = value + size;
            }
        }
    }
}
