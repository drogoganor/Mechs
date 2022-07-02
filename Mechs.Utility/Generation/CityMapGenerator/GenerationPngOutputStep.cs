using Mechs.Utility.Generation.CityMapGenerator.Data;
using System.Drawing;

namespace Mechs.Utility.Generation.CityMapGenerator
{
    /// <summary>
    /// Generates a Png image from output
    /// </summary>
    public class GenerationPngOutputStep : CityGenerationStep
    {
        public override string Name => "GeneratePng";

        public GenerationPngOutputStep(CityGenerationConfig config) : base(config)
        {
        }

        public override CityGenerationData Process(CityGenerationData input)
        {
            using var bitmap = new Bitmap((int)Config.MapSize.X, (int)Config.MapSize.Y);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                for (var y = 0; y < Config.MapSize.Y; y++)
                {
                    for (var x = 0; x < Config.MapSize.X; x++)
                    {
                        var color = Config.TileTypeColors[input.TileData[x, y].TileType];
                        bitmap.SetPixel(x, y, color);
                    }
                }
            }

            var destPath = Path.Combine(Config.OutputDirectory, $"{Config.MapName}-output.png");

            bitmap.Save(destPath, System.Drawing.Imaging.ImageFormat.Png);

            return input;
        }
    }
}
