using Mechs.Data;
using System.Numerics;
using System.Text.Json;

namespace Mechs.Utility.Commands
{
    internal class CreateMap : IUtilityCommand
    {
        public string CommandName => "CreateMap";

        public void Execute(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException("CreateMap requires 3 arguments: [length] [width] [texture]");
            }

            var length = Convert.ToInt32(args[0]);
            var width = Convert.ToInt32(args[1]);
            var texture = Convert.ToInt32(args[2]);
            var height = length; // TODO: Make an argument
            var fileName = "mapdemo2.json";

            Console.WriteLine($"Writing map file: {fileName} ({length}, {height}, {width}) with texture {texture}.");

            var blocks = new List<MapBlock>();

            for (var x = 0; x < length; x++)
            {
                for (var z = 0; z < width; z++)
                {
                    var block = new MapBlock
                    {
                        Position = new Vector3(x, 0, z),
                        FaceTextures = new[] { texture, 0, 0, 0, 0, 0 },
                    };

                    if (x == 0 || x == length - 1)
                    {
                        block.FaceTextures[2] = texture;
                    }

                    if (z == 0 || z == width - 1)
                    {
                        block.FaceTextures[4] = texture;
                    }

                    blocks.Add(block);
                }
            }

            var map = new GameMap
            {
                Blocks = blocks.ToArray(),
                Name = "Generated Map",
                Size = new Vector3(width, height, length),
            };

            var mapJson = JsonSerializer.Serialize(map, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
            });

            var contentDir = Path.Combine(AppContext.BaseDirectory, "Content");
            var mapJsonFilePath = Path.Combine(contentDir, fileName);
            using var outputFile = new StreamWriter(mapJsonFilePath);
            outputFile.Write(mapJson);

            Console.WriteLine($"Wrote map file: {mapJsonFilePath}.");
        }
    }
}
