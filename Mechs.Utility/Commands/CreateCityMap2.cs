using Mechs.Data;
using Mechs.Utility.Generation.CityMapGenerator;
using Mechs.Utility.Generation.CityMapGenerator.Data;
using Mechs.Utility.Generation.CityMapGenerator.Enums;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace Mechs.Utility.Commands
{
    internal class CreateCityMap2 : IUtilityCommand
    {
        public string CommandName => "CreateCityMap2";
        private int _length = 64;
        private int _height = 32;
        private int _width = 64;

        public void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("CreateCityMap requires 2 arguments: [length] [width]");
            }

            _length = Convert.ToInt32(args[0]);
            _width = Convert.ToInt32(args[1]);


            var config = new CityGenerationConfig
            {
                OutputDirectory = Environment.CurrentDirectory,
                MainRoadWidth = 4,
                MapName = "Test",
                MapSize = new Vector2(_length, _width),
                NumMainRoadPoints = 6,
                MainRoadsSamplerConfig = new RandomSamplerConfig
                {
                    MinDistanceFromEdge = 12,
                    //MinDistanceFromOthers = 32,
                    MinDistanceFromOthersAxis = 24,
                    Size = new Vector2(_length, _width),
                },
                TileTypeColors = new Dictionary<TileTypeEnum, Color>
                {
                    { TileTypeEnum.Grass, Color.Green },
                    { TileTypeEnum.Road, Color.DarkSlateGray },
                }
            };

            var generator = new CityGenerator(new CityGenerationData(), config);
            generator.Generate(new CityGenerationStep[]
            {
                new GenerateMainRoadsStep(config),
                new GenerationPngOutputStep(config)
            });

            Console.WriteLine($"Writing map: {config.MapName} ({_length}, {_height}, {_width}).");

        }
    }
}
