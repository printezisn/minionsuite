using System;
using System.IO;
using MinionSuite.Tests.Helpers;
using MinionSuite.Tool;
using MinionSuite.Tool.Generators;
using Xunit;

namespace MinionSuite.Tests.Generators
{
    public class ApiControllerGeneratorTests
    {
        [Fact]
        public void TestGenerator()
        {
            var args = new string[]
            {
                "apicontroller", "-ns", "MinionSuite.Tests.Templates.Controllers.Api",
                "-m", "./Models/Post.cs", "-o", "Api"
            };
            var argReader = new ArgReader(args);

            GeneratorFactory.GetGenerator(argReader).Generate(argReader);

            Assert.True(File.Exists("Api/PostsController.cs"));

            AssertHelper.AssertEqualFile("Templates/ApiController/PostsController.cs", "Api/PostsController.cs");
        }
    }
}
