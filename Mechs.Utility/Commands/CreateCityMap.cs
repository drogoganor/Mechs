using Mechs.Data;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace Mechs.Utility.Commands
{
    internal class RoadSegment
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }

        public int Direction { get; set; }

        public bool Test(Vector3 point)
        {
            if (point.X >= Start.X && point.X < End.X &&
                point.Z >= Start.Z && point.Z < End.Z)
                return true;
            return false;
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

        private readonly List<RoadSegment> _roadSegments = new();
        private readonly int[,] _grid;

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

            //CreateGroundPlane();
            GenerateMainRoads();
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

        private void GenerateMainRoads()
        {
            const int minDistanceFromEdge = 5;
            const int roadWidth = 4;
            const int minimumRoadLength = 8;

            // Choose an edge to start
            var startEdge = _random.Next(4);

            var isEven = startEdge % 2 == 0;

            var length = isEven ? _length : _width;
            var width = !isEven ? _length : _width;

            var startCoordA = startEdge > 2 ? length : 0;

            var startCoordB = _random.Next(width - 1 - (2 * minDistanceFromEdge)) + minDistanceFromEdge;

            var randLength = _random.Next(length - minimumRoadLength) + minimumRoadLength;

            Console.WriteLine($"Facing was {startEdge}");

            var startCoord = startEdge switch
            {
                0 => new Vector3(startCoordA, 0, startCoordB),
                1 => new Vector3(startCoordB, 0, startCoordA),
                2 => new Vector3(startCoordA, 0, startCoordB),
                3 => new Vector3(startCoordB, 0, startCoordA - randLength),
                _ => throw new NotImplementedException(),
            };

            var endCoord = startEdge switch
            {
                0 => startCoord + new Vector3(randLength, 0, roadWidth),
                1 => startCoord + new Vector3(roadWidth, 0, randLength),
                2 => new Vector3(startCoordA + randLength, 0, startCoordB + roadWidth),
                3 => new Vector3(startCoordB + roadWidth, 0, startCoordA),
                _ => throw new NotImplementedException(),
            };

            Console.WriteLine($"Start: {startCoord}   End: {endCoord}");

            var roadSegment = new RoadSegment
            {
                Direction = startEdge,
                Start = startCoord,
                End = endCoord
            };

            _roadSegments.Add(roadSegment);
            _roadSegments.Add(CreateMainRoadTurn(roadSegment, roadWidth));
        }

        private RoadSegment CreateMainRoadTurn(RoadSegment lastSegment, int roadWidth)
        {
            const int minimumRoadLength = 8;
            var startEdge = 0;
            var directionDoublesBack = true;
            do
            {
                directionDoublesBack = false;
                startEdge = _random.Next(4);
                var bothEven = lastSegment.Direction % 2 == 0 && startEdge % 2 == 0;
                var bothOdd = lastSegment.Direction % 2 != 0 && startEdge % 2 != 0;

                if ((bothEven || bothOdd) && lastSegment.Direction != startEdge)
                {
                    directionDoublesBack = true;
                }
            } while (directionDoublesBack);

            Console.WriteLine($"Facing was {startEdge}");

            var isEven = startEdge % 2 == 0;

            var length = isEven ? _length : _width;
            var width = !isEven ? _length : _width;

            var randLength = _random.Next(length - minimumRoadLength) + minimumRoadLength;

            var startCoord = startEdge switch
            {
                0 => new Vector3(lastSegment.End.X, 0, lastSegment.End.Z),
                1 => new Vector3(lastSegment.End.Z, 0, lastSegment.End.X),
                2 => new Vector3(lastSegment.End.X, 0, lastSegment.End.Z),
                3 => new Vector3(lastSegment.End.Z, 0, lastSegment.End.X - randLength),
                _ => throw new NotImplementedException(),
            };

            var endCoord = startEdge switch
            {

                0 => startCoord + new Vector3(randLength, 0, roadWidth),
                1 => startCoord + new Vector3(roadWidth, 0, randLength),
                2 => new Vector3(startCoord.X + randLength, 0, startCoord.Z + roadWidth),
                3 => new Vector3(startCoord.Z + roadWidth, 0, startCoord.X),
                _ => throw new NotImplementedException(),
            };

            Console.WriteLine($"-Start: {startCoord}   End: {endCoord}");


            var roadSegment = new RoadSegment
            {
                Direction = startEdge,
                Start = startCoord,
                End = endCoord
            };

            return roadSegment;
        }

        private void DrawRoads()
        {
            const int roadTexture = 6;

            foreach (var block in _blocks)
            {
                foreach (var segment in _roadSegments)
                {
                    var isRoad = segment.Test(block.Position);
                    if (isRoad)
                    {
                        block.FaceTextures[0] = roadTexture;
                        break;
                    }
                }
            }
        }

        private void DrawToTextFile()
        {
            var sb = new StringBuilder();
            for (var z = 0; z < _width; z++)
            {
                for (var x = 0; x < _length; x++)
                {
                    var isRoad = false;
                    int segmentIndex = 0;
                    foreach (var segment in _roadSegments)
                    {
                        if (segment.Test(new Vector3(x, 0, z)))
                        {
                            isRoad = true;
                            break;
                        }
                        else
                        {
                            segmentIndex++;
                        }
                    }

                    if (isRoad)
                    {
                        if (segmentIndex == 0)
                        {
                            sb.Append("x");
                        }
                        else
                        {
                            sb.Append("y");
                        }
                    }
                    else
                    {
                        sb.Append("_");
                    }
                }

                sb.AppendLine();
            }


            var contentDir = Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\");
            var mapJsonFilePath = Path.Combine(contentDir, "road-test.txt");
            using var outputFile = new StreamWriter(mapJsonFilePath);
            outputFile.Write(sb.ToString());
        }
    }
}
