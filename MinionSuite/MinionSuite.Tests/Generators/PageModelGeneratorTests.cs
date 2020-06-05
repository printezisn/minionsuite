using System;
using System.IO;
using MinionSuite.Tests.Helpers;
using MinionSuite.Tool;
using MinionSuite.Tool.Generators;
using Xunit;

namespace MinionSuite.Tests.Generators
{
    public class GeneratorTests
    {
        [Fact]
        public void TestGenerator()
        {
            var args = new string[] { "pagemodel", "-ns", "MinionSuite.Tests.Templates" };
            var argReader = new ArgReader(args);

            GeneratorFactory.GetGenerator(argReader).Generate(argReader);

            Assert.True(File.Exists("IPageModel.cs"));
            Assert.True(File.Exists("PageModel.cs"));

            AssertHelper.AssertEqualFile("Templates/PageModel/IPageModel.cs", "IPageModel.cs");
            AssertHelper.AssertEqualFile("Templates/PageModel/PageModel.cs", "PageModel.cs");
        }
    }
}
