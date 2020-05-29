using System;
using System.IO;
using Xunit;

namespace MinionSuite.Tests.Helpers
{
    /// <summary>
    /// Contains helper assert methods
    /// </summary>
    public static class AssertHelper
    {
        /// <summary>
        /// Asserts that two files are equal
        /// </summary>
        /// <param name="expectedFile">The expected file</param>
        /// <param name="actualFile">The actual file</param>
        public static void AssertEqualFile(string expectedFile, string actualFile)
        {
            Assert.Equal(File.ReadAllText(expectedFile), File.ReadAllText(actualFile));
        }
    }
}
