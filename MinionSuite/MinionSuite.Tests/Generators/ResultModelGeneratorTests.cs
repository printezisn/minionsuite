using System;
using System.IO;
using MinionSuite.Tests.Helpers;
using MinionSuite.Tool;
using MinionSuite.Tool.Generators;
using Xunit;

namespace MinionSuite.Tests.Generators
{
    public class ResultModelGeneratorTests
    {
        [Fact]
        public void TestGenerator()
        {
            var args = new string[] { "resultmodel", "-ns", "MinionSuite.Tests.Templates" };
            var argReader = new ArgReader(args);

            GeneratorFactory.GetGenerator(argReader).Generate(argReader);

            Assert.True(File.Exists("ResultModel.cs"));

            AssertHelper.AssertEqualFile("Templates/ResultModel/ResultModel.cs", "ResultModel.cs");
        }
    }
}
