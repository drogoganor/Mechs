using Mechs.Data;
using System.Numerics;
using System.Text.Json;

namespace Mechs.Utility.Commands
{
    internal class CreateCityMap : IUtilityCommand
    {
        public string CommandName => "CreateCityMap";
        private int _length = 64;
        private int _height = 32;
        private int _width = 64;
        private int _texture = 1;
        private int _altTexture = 2;
        private string _filename = "mapcity.json";
        private List<MapBlock> _blocks = new();

        public void Execute(string[] args)
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("CreateCityMap requires 3 arguments: [length] [width] [texture]");
            }

            _length = Convert.ToInt32(args[0]);
            _width = Convert.ToInt32(args[1]);
            _texture = Convert.ToInt32(args[2]);
            _altTexture = args.Length > 3 ? Convert.ToInt32(args[3]) : _texture;
            //_height = _length; // TODO: Make an argument
            _filename = "mapcity.json";

            Console.WriteLine($"Writing map file: {_filename} ({_length}, {_height}, {_width}) with texture {_texture}.");

            for (var x = 0; x < _length; x++)
            {
                for (var z = 0; z < _width; z++)
                {
                    var thisTexture = _texture;
                    if ((z % 2 != 0 && x % 2 == 0) || (z % 2 == 0 && x % 2 != 0))
                    {
                        thisTexture = _altTexture;
                    }

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

            var contentDir = Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\...\\..\\..\\Mechs.Game\\Content");
            var mapJsonFilePath = Path.Combine(contentDir, _filename);
            using var outputFile = new StreamWriter(mapJsonFilePath);
            outputFile.Write(mapJson);

            Console.WriteLine($"Wrote map file: {mapJsonFilePath}.");
        }
    }
}
