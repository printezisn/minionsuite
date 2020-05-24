using System;

namespace MinionSuite.Tool.Generators
{
    /// <summary>
    /// Generates items
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Generates items
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        void Generate(ArgReader argReader);

        /// <summary>
        /// Displays a help message
        /// </summary>
        void ShowHelpMessage();
    }
}
