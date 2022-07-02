using Mechs.Utility.Generation.CityMapGenerator.Enums;
using System.Drawing;

namespace Mechs.Utility.Generation.CityMapGenerator.Data
{
    public class CityGenerationConfig : GeneratorConfig
    {
        public TileTypeEnum RoadTileType => TileTypeEnum.Road;
        public int NumMainRoadPoints { get; set; } = 4;
        public int MainRoadWidth { get; set; } = 6;

        public Dictionary<TileTypeEnum, Color> TileTypeColors { get; set; }

        public RandomSamplerConfig MainRoadsSamplerConfig { get; set; }
    }
}