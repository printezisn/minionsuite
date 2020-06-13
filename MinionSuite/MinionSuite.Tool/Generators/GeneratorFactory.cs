using System;

namespace MinionSuite.Tool.Generators
{
    /// <summary>
    /// Factory that returns the appropriate generator
    /// </summary>
    public static class GeneratorFactory
    {
        /// <summary>
        /// Returns the appropriate generator
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        /// <returns>The generator</returns>
        public static IGenerator GetGenerator(ArgReader argReader)
        {
            return argReader.Generator switch
            {
                "servicegen" => new ServiceGenerator(),
                "servicegen:test" => new ServiceTestGenerator(),
                "pagemodel" => new PageModelGenerator(),
                "resultmodel" => new ResultModelGenerator(),
                "mvccontroller" => new MvcControllerGenerator(),
                "mvccontroller:test" => new MvcControllerTestGenerator(),
                "apicontroller" => new ApiControllerGenerator(),
                _ => throw new ArgumentException($"{argReader.Generator} is an invalid generator."),
            };
        }
    }
}
