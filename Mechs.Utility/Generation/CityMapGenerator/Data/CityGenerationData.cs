using System.Numerics;

namespace Mechs.Utility.Generation.CityMapGenerator.Data
{
    public class CityGenerationData : GenerationData
    {
        public List<RoadSegment> MainRoadSegments { get; set; } = new List<RoadSegment>();
    }
}
