using Mechs.Data;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace Mechs.Utility.Commands
{
    public class GrowthTracking
    {
        public Vector2[] Corners = new Vector2[4];
    }

    internal class RoadGrowth
    {
        public Vector2 Start { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Direction { get; set; }

        public List<GrowthTracking> Tracking { get; set; } = new List<GrowthTracking>();

        public int Momentum { get; set; }
        public int Facing { get; set; }
        public float NoiseDirection { get; set; }
    }

    internal class RoadGrowthConfig
    {
        public int RoadWidth { get; set; }
        public int Momentum { get; set; }
        public int TurnCount { get; set; }
        public int SpawnCount { get; set; }
    }

    internal class ZoneConsolidation
    {
        private HashSet<Vector2> zoneMembers = new HashSet<Vector2>();
        private List<Vector2> frontier = new();
        private readonly int _length;
        private readonly int _width;
        public int Count => zoneMembers.Count;

        public ZoneConsolidation(int length, int width, Vector2 sample)
        {
            frontier.Add(sample);
            _length = length;
            _width = width;
        }

        public bool Contains(Vector2 pos)
        {
            return zoneMembers.Contains(pos);
        }

        public void Run(ref int[,] grid)
        {
            bool done;
            do
            {
                done = Spread(ref grid);
            } while (!done);
        }

        public bool Spread(ref int[,] grid)
        {
            var newFrontier = new List<Vector2>();
            foreach (var pair in frontier)
            {
                int x = (int)pair.X;
                int y = (int)pair.Y;

                var top = new Vector2(x, y - 1);
                var bottom = new Vector2(x, y + 1);
                var left = new Vector2(x - 1, y);
                var right = new Vector2(x + 1, y);

                var sides = new[] { top, bottom, left, right };

                foreach (var side in sides)
                {
                    if (side.X < 0 || side.X >= _length || side.Y < 0 || side.Y >= _width)
                    {
                        continue;
                    }

                    if (!zoneMembers.Contains(side) && grid[(int)side.X, (int)side.Y] == 0)
                    {
                        zoneMembers.Add(side);

                        newFrontier.Add(side);
                    }
                }
            }

            frontier = newFrontier;

            if (frontier.Count > 0)
                return false;
            return true;
        }
    }

    internal class CreateCityMap : IUtilityCommand
    {
        public string CommandName => "CreateCityMap";
        private int _length = 64;
        private int _height = 32;
        private int _width = 64;
        private int _texture = 1;
        private string _filename = "mapcity.json";
        private readonly List<MapBlock> _blocks = new();
        private readonly Random _random = new();

        private readonly Queue<RoadGrowth> roadGrowths = new();
        private int[,] _grid;
        private RoadGrowthConfig _growthConfig;
        private readonly Queue<Vector2> zoneSamples = new();
        private readonly List<RoadGrowth> mainRoads = new();

        public void Execute(string[] args)
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("CreateCityMap requires 3 arguments: [length] [width] [texture]");
            }

            _length = Convert.ToInt32(args[0]);
            _width = Convert.ToInt32(args[1]);
            _texture = Convert.ToInt32(args[2]);
            //_height = _length; // TODO: Make an argument
            _filename = "mapcity.json";

            Console.WriteLine($"Writing map file: {_filename} ({_length}, {_height}, {_width}) with texture {_texture}.");

            _grid = new int[_length, _width];

            _growthConfig = new RoadGrowthConfig
            {
                Momentum = 8,
                RoadWidth = 8,
                SpawnCount = 1,
            };

            //CreateGroundPlane();
            GenerateMainRoads();
            RandomSampleZones();
            //DrawRoads();
            //WriteMap();

            DrawToTextFile();
        }

        private void CreateGroundPlane()
        {
            for (var x = 0; x < _length; x++)
            {
                for (var z = 0; z < _width; z++)
                {
                    var thisTexture = _texture;
                    var block = new MapBlock
                    {
                        Position = new Vector3(x, 0, z),
                        FaceTextures = new[] { thisTexture, 0, 0, 0, 0, 0 },
                    };

                    if (x == 0 || x == _length - 1)
                    {
                        block.FaceTextures[2] = thisTexture;
                    }

                    if (z == 0 || z == _width - 1)
                    {
                        block.FaceTextures[4] = thisTexture;
                    }

                    _blocks.Add(block);
                }
            }
        }

        private void WriteMap()
        {
            var map = new GameMap
            {
                Blocks = _blocks.ToArray(),
                Name = "Generated Map",
                Size = new Vector3(_width, _height, _length),
            };

            var mapJson = JsonSerializer.Serialize(map, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
            });

            var contentDir = Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\Mechs.Game\\bin\\Debug\\net6.0\\Content");
            var mapJsonFilePath = Path.Combine(contentDir, _filename);
            using var outputFile = new StreamWriter(mapJsonFilePath);
            outputFile.Write(mapJson);

            Console.WriteLine($"Wrote map file: {mapJsonFilePath}.");
        }

        private void RandomSampleZones()
        {
            int randomSampleCount = (int)((_length * _width) / _length);
            var sampleList = new Queue<Vector2>();

            do
            {
                var x = _random.Next(_length);
                var y = _random.Next(_width);
                var vec = new Vector2(x, y);

                if (_grid[x, y] == 0 && !sampleList.Contains(vec))
                {
                    sampleList.Enqueue(vec);
                }
            } while (sampleList.Count < randomSampleCount);

            var firstSample = sampleList.Dequeue();
            var firstZone = new ZoneConsolidation(_length, _width, firstSample);
            firstZone.Run(ref _grid);

            var zones = new List<ZoneConsolidation>() { firstZone };

            while (sampleList.Count > 0)
            {
                var sample = sampleList.Dequeue();
                var belongsToZone = zones.FirstOrDefault(zone => zone.Contains(sample));
                if (belongsToZone == null)
                {
                    var newZone = new ZoneConsolidation(_length, _width, sample);
                    newZone.Run(ref _grid);
                    zones.Add(newZone);
                }
            }

            foreach (var zone in zones)
            {
                Console.WriteLine($"Zone: {zone.Count}");
            }
        }

        private void GenerateMainRoads()
        {
            const int minDistanceFromEdge = 5;

            // Choose an edge to start
            var startEdge = 0; // _random.Next(4);

            var isEven = startEdge % 2 == 0;

            var length = isEven ? _length : _width;
            var width = !isEven ? _length : _width;

            var startCoordA = startEdge > 2 ? length : 0;

            var startCoordB = _random.Next(width - 1 - (2 * minDistanceFromEdge)) + minDistanceFromEdge;

            var randLength = _random.Next(length - _growthConfig.Momentum) + _growthConfig.Momentum;

            Console.WriteLine($"Facing was {startEdge}");

            var startCoord = startEdge switch
            {
                0 => new Vector2(startCoordA, startCoordB),
                1 => new Vector2(startCoordB, startCoordA),
                2 => new Vector2(startCoordA, startCoordB),
                3 => new Vector2(startCoordB, startCoordA - randLength),
                _ => throw new NotImplementedException(),
            };

            //var endCoord = startEdge switch
            //{
            //    0 => startCoord + new Vector3(randLength, 0, roadWidth),
            //    1 => startCoord + new Vector3(roadWidth, 0, randLength),
            //    2 => new Vector3(startCoordA + randLength, 0, startCoordB + roadWidth),
            //    3 => new Vector3(startCoordB + roadWidth, 0, startCoordA),
            //    _ => throw new NotImplementedException(),
            //};

            Console.WriteLine($"Start: {startCoord}");

            var roadSegment = new RoadGrowth
            {
                Direction = GetDirectionFromFacing(startEdge),
                Start = startCoord,
                Size = new Vector2(_growthConfig.RoadWidth, _growthConfig.RoadWidth),
                Momentum = _growthConfig.Momentum,
                Facing = startEdge,
                NoiseDirection = (float)startEdge,
            };

            roadGrowths.Enqueue(roadSegment);
            mainRoads.AddRange(GrowRoads());
        }

        private List<RoadGrowth> GrowRoads()
        {
            var roads = new List<RoadGrowth>();
            while (roadGrowths.Count > 0)
            {
                var road = roadGrowths.Dequeue();
                while (GrowRoad(road))
                {
                    ;
                }

                roads.Add(road);
            }

            return roads;
        }

        private int GetRandomDirectionExcept(int[] except)
        {
            int startEdge;
            do
            {
                startEdge = _random.Next(4);
            } while (except.Contains(startEdge));

            return startEdge;
        }

        private bool GrowRoad(RoadGrowth growth)
        {
            // Grow
            for (var i = 0; i < growth.Momentum; i++)
            {
                // Debug markers
                var topLeft = new Vector2(growth.Start.X - 1, growth.Start.Y - 1);
                var topRight = new Vector2(growth.Start.X + growth.Size.X, growth.Start.Y - 1);
                var bottomLeft = new Vector2(growth.Start.X - 1, growth.Start.Y + growth.Size.Y);
                var bottomRight = new Vector2(growth.Start.X + growth.Size.X, growth.Start.Y + growth.Size.Y);
                var growthTrack = new GrowthTracking
                {
                    Corners = new[] { topLeft, topRight, bottomLeft, bottomRight },
                };
                growth.Tracking.Add(growthTrack);

                MarkRoadBlocks(growth);
                growth.Start += growth.Direction;

                if (growth.Start.X < -growth.Size.X || growth.Start.X > _length || growth.Start.Y < -growth.Size.Y || growth.Start.Y > _width)
                {
                    return false;
                }
            }

            // Subtract momentum
            growth.Momentum /= 2;
            if (growth.Momentum <= 0)
            {
                growth.Momentum = 1;
            }

            // Do some random number generation
            var swingRandom = (2f * (_random.NextSingle() - .5f)) / 1f;
            growth.NoiseDirection += swingRandom / growth.Momentum;

            var roadWidth = _growthConfig.RoadWidth;
            Console.WriteLine($"NoiseDirection: {growth.NoiseDirection}");

            var startEdge = WrapFacing((int)growth.NoiseDirection);

            var reverseCurrentFacing = WrapFacing(growth.Facing + 2);
            if (startEdge != growth.Facing)
            {
                // Increase momentum
                growth.Momentum = _growthConfig.Momentum;
                growth.Direction = GetDirectionFromFacing(startEdge);
                growth.Facing = startEdge;

                // Consider branching
                var willBranch = (_random.NextSingle() * (1f / _growthConfig.SpawnCount)) > 0.25f;
                if (willBranch)
                {
                    Console.WriteLine($"Get new direction except {growth.Facing}, {reverseCurrentFacing}");
                    _growthConfig.SpawnCount++;
                    var newBranchFacing = GetRandomDirectionExcept(new [] { growth.Facing, reverseCurrentFacing });
                    Console.WriteLine($"Branched to facing {newBranchFacing}");

                    var roadSegment = new RoadGrowth
                    {
                        Direction = GetDirectionFromFacing(newBranchFacing),
                        Start = growth.Start,
                        Size = new Vector2(roadWidth, roadWidth),
                        Momentum = _growthConfig.Momentum,
                        Facing = newBranchFacing,
                        NoiseDirection = (float)newBranchFacing,
                    };

                    roadGrowths.Enqueue(roadSegment);
                }
            }

            return true;
        }

        private Vector2 GetDirectionFromFacing(int facing)
        {
            var roadWidth = _growthConfig.RoadWidth;
            return facing switch
            {
                0 => new Vector2(roadWidth, 0),
                1 => new Vector2(0, roadWidth),
                2 => new Vector2(-roadWidth, 0),
                3 => new Vector2(0, -roadWidth),
                _ => throw new NotImplementedException(),
            };
        }

        private int WrapFacing(int facing)
        {
            return facing < 0 ? 4 + facing : facing % 4;
        }

        private void MarkRoadBlocks(RoadGrowth growth)
        {
            for (int x = (int)growth.Start.X; x < (int)(growth.Start.X + growth.Size.X); x++)
            {
                for (int y = (int)growth.Start.Y; y < (int)(growth.Start.Y + growth.Size.Y); y++)
                {
                    if (x < 0 || y < 0 || x >= _length || y >= _width)
                    {
                        continue;
                    }

                    _grid[x, y] = 1;
                }
            }

            // Mark road growths
            foreach (var growthTrack in growth.Tracking)
            {
                foreach (var coord in growthTrack.Corners)
                {
                    var x = (int)coord.X;
                    var y = (int)coord.Y;
                    if (x < 0 || y < 0 || x >= _length || y >= _width)
                    {
                        continue;
                    }

                    _grid[x, y] = 2;
                }
            }
        }


        private void DrawRoads()
        {
            const int roadTexture = 6;

            foreach (var block in _blocks)
            {
                //foreach (var segment in _roadSegments)
                //{
                //    var isRoad = segment.Test(block.Position);
                //    if (isRoad)
                //    {
                //        block.FaceTextures[0] = roadTexture;
                //        break;
                //    }
                //}
            }
        }

        private void DrawToTextFile()
        {
            var sb = new StringBuilder();
            for (var z = 0; z < _width; z++)
            {
                for (var x = 0; x < _length; x++)
                {
                    var gridVal = _grid[x, z];
                    var tileChar = gridVal switch
                    {
                        0 => "_",
                        1 => "x",
                        2 => "O",
                        _ => "?"
                    };
                    
                    sb.Append(tileChar);
                }

                sb.AppendLine();
            }


            var contentDir = Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\");
            var mapJsonFilePath = Path.Combine(contentDir, "road-test.txt");
            using var outputFile = new StreamWriter(mapJsonFilePath);
            outputFile.Write(sb.ToString());

            Console.WriteLine($"Wrote debug file: {mapJsonFilePath}");
        }
    }
}
