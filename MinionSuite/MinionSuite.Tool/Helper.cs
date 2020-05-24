using System;
using System.IO;

namespace MinionSuite.Tool
{
    /// <summary>
    /// Class with helper methods
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Indicates if a type represents a string
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the type represents a string, otherwise false</returns>
        public static bool IsStringType(string type) =>
            type.ToLower() == "string" || type.ToLower() == "system.string";

        /// <summary>
        /// Saves content to an output file
        /// </summary>
        /// <param name="outputFolder">The output folder</param>
        /// <param name="filename">The output filename</param>
        /// <param name="content">The content to save</param>
        public static void SaveToOutput(string outputFolder, string filename, string content)
        {
            File.WriteAllText(Path.Combine(outputFolder, filename), content);
        }
    }
}
