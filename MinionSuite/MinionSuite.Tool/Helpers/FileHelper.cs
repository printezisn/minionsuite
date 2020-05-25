using System;
using System.IO;

namespace MinionSuite.Tool.Helpers
{
    /// <summary>
    /// Class with file helper methods
    /// </summary>
    public static class FileHelper
    {
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
