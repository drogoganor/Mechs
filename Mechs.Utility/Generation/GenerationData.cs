using Mechs.Utility.Generation.CityMapGenerator.Enums;

namespace Mechs.Utility.Generation
{
    //public interface IGenerationData
    //{
    //    TileData[,] TileData { get; set; }
    //}

    /// <summary>
    /// Stores the level data for this generation step
    /// </summary>
    public abstract class GenerationData //: IGenerationData
    {
        public TileData[,] TileData { get; set; }
    }

    public class TileData
    {
        public TileTypeEnum TileType { get; set; }
    }
}
