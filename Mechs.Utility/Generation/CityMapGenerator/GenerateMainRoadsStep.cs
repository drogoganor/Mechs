using Mechs.Utility.Generation.CityMapGenerator.Data;
using System.Numerics;

namespace Mechs.Utility.Generation.CityMapGenerator
{
    public class GenerateMainRoadsStep : CityGenerationStep
    {
        public override string Name => "Generate Main Roads";

        private readonly RandomSampler _randomSampler;

        public GenerateMainRoadsStep(CityGenerationConfig config) : base(config)
        {
            _randomSampler = new RandomSampler(config.MainRoadsSamplerConfig);
        }

        public override CityGenerationData Process(CityGenerationData input)
        {
            var randomPoints = _randomSampler.GetRandomPoints(Config.NumMainRoadPoints);
            var randomDirections = _randomSampler.GetRandomDirections(Config.NumMainRoadPoints);

            input.MainRoadSegments = randomPoints
                .Select((randomPoint, i) => new RoadSegment(
                    new MapArea
                    {
                        TopLeft = new Vector2(randomPoint.X, randomPoint.Y),
                        BottomRight = randomPoint + new Vector2(Config.MainRoadWidth),
                    },
                    randomDirections[i])
                )
                .ToList();

            var grownRoadSegments = new List<RoadSegment>();
            foreach (var roadSegment in input.MainRoadSegments)
            {
                var newRoadSegments = new List<RoadSegment>();
                newRoadSegments.Add(roadSegment);

                var splitRoads = roadSegment.SplitRoadInTwo();
                newRoadSegments.AddRange(splitRoads);

                foreach (var road in newRoadSegments)
                {
                    // Grow this road right to the edge or until it contacts another road
                    var growLength = road.GetMaxGrowLength(grownRoadSegments, Config.MapSize);
                    if (growLength > 0)
                    {
                        road.Resize(growLength);
                    }
                    else
                    {
                        Console.Error.WriteLine("Didn't resize road");
                    }

                    grownRoadSegments.Add(road);
                }
            }

            input.MainRoadSegments = grownRoadSegments;


            // Apply to level data
            foreach (var segment in input.MainRoadSegments)
            {
                var points = segment.Area.AllPositions;
                foreach (var point in points)
                {
                    if (!segment.Area.Intersects(point))
                    {
                        continue;
                    }

                    if (point.X < 0 || point.Y < 0 || point.X >= Config.MapSize.X || point.Y >= Config.MapSize.Y)
                    {
                        continue;
                    }

                    input.TileData[(int)point.X, (int)point.Y].TileType = Config.RoadTileType;
                }
            }

            return input;
        }
    }
}
