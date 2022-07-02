using Humanizer;
using Mechs.Utility.Generation.CityMapGenerator.Enums;
using System.Text;

namespace Mechs.Utility.Generation
{
    public abstract class Generator<TData, TConfig, TGenerationStep>
        where TData : GenerationData
        where TConfig : GeneratorConfig
        where TGenerationStep : GenerationStep<TData, TConfig>
    {
        protected TData Data { get; set; }
        protected TConfig Config { get; set; }

        public Generator(TData data, TConfig config)
        {
            Data = data;
            Config = config;

            Data.TileData = new TileData[(int)config.MapSize.X, (int)config.MapSize.Y];

            for (var y = 0; y < config.MapSize.Y; y++)
            {
                for (var x = 0; x < config.MapSize.X; x++)
                {
                    data.TileData[x, y] = new TileData
                    {
                        TileType = config.DefaultTileType
                    };
                }
            }
        }

        public void Generate(TGenerationStep[] steps)
        {
            var start = DateTime.Now;

            var data = Data;
            foreach (var step in steps)
            {
                var stepStart = DateTime.Now;

                var output = step.Process(data);
                data = output;

                var stepEnd = DateTime.Now;
                var stepDuration = stepEnd - stepStart;
                Console.WriteLine($"Step '{step.Name}' took {stepDuration.Humanize(2)}.");
            }

            DrawToTextFile();

            var end = DateTime.Now;
            var duration = end - start;
            Console.WriteLine($"Level generation took {duration.Humanize(2)}.");
        }

        private void DrawToTextFile()
        {
            var sb = new StringBuilder();
            for (var x = 0; x < Config.MapSize.X; x++)
            {
                for (var y = 0; y < Config.MapSize.Y; y++)
                {
                    var gridVal = Data.TileData[x, y];
                    var tileChar = gridVal.TileType switch
                    {
                        TileTypeEnum.Grass => "_",
                        TileTypeEnum.Road => "x",
                        TileTypeEnum.Concrete => "O",
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
