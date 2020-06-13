using System;
using System.IO;
using MinionSuite.Tests.Helpers;
using MinionSuite.Tool;
using MinionSuite.Tool.Generators;
using Xunit;

namespace MinionSuite.Tests.Generators
{
    public class MvcControllerGeneratorTests
    {
        [Fact]
        public void TestGenerator()
        {
            var args = new string[]
            {
                "mvccontroller", "-ns", "MinionSuite.Tests.Templates", "-m", "./Models/Post.cs"
            };
            var argReader = new ArgReader(args);

            GeneratorFactory.GetGenerator(argReader).Generate(argReader);

            Assert.True(File.Exists("PostsController.cs"));

            AssertHelper.AssertEqualFile("Templates/MvcController/PostsController.cs", "PostsController.cs");
        }
    }
}
