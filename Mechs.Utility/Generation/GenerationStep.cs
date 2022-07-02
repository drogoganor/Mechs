namespace Mechs.Utility.Generation
{
    //public interface IGenerationStep<TData, TConfig>
    //    where TData : IGenerationData
    //    where TConfig : IGeneratorConfig
    //{
    //    TData Process(TData input);
    //}

    /// <summary>
    /// Generation step that takes an input and output
    /// </summary>
    public abstract class GenerationStep<TData, TConfig>
        where TData : GenerationData
        where TConfig : GeneratorConfig
    {
        protected TConfig Config { get; set; }
        public abstract string Name { get; }

        public GenerationStep(TConfig config)
        {
            Config = config;
        }

        public abstract TData Process(TData input);
    }
}
