using Mechs.Utility.Generation.CityMapGenerator.Data;

namespace Mechs.Utility.Generation.CityMapGenerator
{
    public abstract class CityGenerationStep : GenerationStep<CityGenerationData, CityGenerationConfig>
    {
        public CityGenerationStep(CityGenerationConfig config) : base(config)
        {
        }
    }
}
