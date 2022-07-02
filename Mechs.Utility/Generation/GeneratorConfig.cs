using Mechs.Utility.Generation.CityMapGenerator.Enums;
using System.Numerics;

namespace Mechs.Utility.Generation
{
    //public interface IGeneratorConfig
    //{
    //    Vector2 MapSize { get; }
    //    public string MapName { get; }
    //    public string OutputDirectory { get; }
    //    public TileTypeEnum DefaultTileType { get; }
    //}

    public class GeneratorConfig //: IGeneratorConfig
    {
        public Vector2 MapSize { get; set; }
        public string MapName { get; set; }
        public string OutputDirectory { get; set; }
        public TileTypeEnum DefaultTileType => TileTypeEnum.Grass;
    }
}
