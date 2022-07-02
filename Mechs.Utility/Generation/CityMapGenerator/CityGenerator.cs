using Mechs.Utility.Generation.CityMapGenerator.Data;

namespace Mechs.Utility.Generation.CityMapGenerator
{
    public class CityGenerator : Generator<CityGenerationData, CityGenerationConfig, CityGenerationStep>
    {
        public CityGenerator(CityGenerationData data, CityGenerationConfig config) : base(data, config)
        {
        }
    }
}
