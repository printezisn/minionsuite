using System;
using System.IO;
using MinionSuite.Tests.Helpers;
using MinionSuite.Tool;
using MinionSuite.Tool.Generators;
using Xunit;

namespace MinionSuite.Tests.Generators
{
    public class ServiceGeneratorTests
    {
        [Fact]
        public void TestGenerator()
        {
            var args = new string[]
            {
                "servicegen", "-ns", "MinionSuite.Tests.Templates",
                "-m", "./Models/Post.cs", "-db", "ApplicationContext"
            };
            var argReader = new ArgReader(args);

            GeneratorFactory.GetGenerator(argReader).Generate(argReader);

            Assert.True(File.Exists("IPostService.cs"));
            Assert.True(File.Exists("PostService.cs"));

            AssertHelper.AssertEqualFile("Templates/Service/IPostService.cs", "IPostService.cs");
            AssertHelper.AssertEqualFile("Templates/Service/PostService.cs", "PostService.cs");
        }
    }
}
