using Mechs.Data;
using System.Numerics;
using System.Text.Json;

namespace Mechs.Utility.Commands
{
    internal class RoadSegment
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }

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

            CreateGroundPlane();
            GenerateMainRoads();
            DrawRoads();
            WriteMap();
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

            // Choose an edge to start
            var startEdge = _random.Next(4);

            var isEven = startEdge % 2 == 0;

            var length = isEven ? _length : _width;
            var width = !isEven ? _length : _width;

            var startCoordA = startEdge > 2 ? length - 1 : 0;

            var startCoordB = _random.Next(width - 1 - (2 * minDistanceFromEdge)) + minDistanceFromEdge;

            var randLength = _random.Next(length - 1);

            var startCoord = startEdge switch
            {
                0 => new Vector3(startCoordA, 0, startCoordB),
                1 => new Vector3(startCoordB, 0, startCoordA),
                2 => new Vector3(startCoordA, 0, startCoordB),
                3 => new Vector3(startCoordB, 0, startCoordA),
                _ => throw new NotImplementedException(),
            };

            var endCoord = startEdge switch
            {
                0 => startCoord + new Vector3(randLength, 0, roadWidth),
                1 => startCoord + new Vector3(roadWidth, 0, randLength),
                2 => startCoord - new Vector3(randLength, 0, -roadWidth),
                3 => startCoord - new Vector3(-roadWidth, 0, randLength),
                _ => throw new NotImplementedException(),
            };

            var roadSegment = new RoadSegment
            {
                Start = startCoord,
                End = endCoord
            };

            _roadSegments.Add(roadSegment);
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
    }
}
