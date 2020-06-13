using System;
using System.IO;
using MinionSuite.Tests.Helpers;
using MinionSuite.Tool;
using MinionSuite.Tool.Generators;
using Xunit;

namespace MinionSuite.Tests.Generators
{
    public class MvcControllerTestGeneratorTests
    {
        [Fact]
        public void TestGenerator()
        {
            var args = new string[]
            {
                "mvccontroller:test", "-ns", "MinionSuite.Tests.Templates", "-m", "./Models/Post.cs", "-gwaf"
            };
            var argReader = new ArgReader(args);

            GeneratorFactory.GetGenerator(argReader).Generate(argReader);

            Assert.True(File.Exists("CustomWebApplicationFactory.cs"));
            Assert.True(File.Exists("PostsControllerTests.cs"));

            AssertHelper.AssertEqualFile(
                "Templates/MvcControllerTest/CustomWebApplicationFactory.txt", "CustomWebApplicationFactory.cs");
            AssertHelper.AssertEqualFile(
                "Templates/MvcControllerTest/PostsControllerTests.txt", "PostsControllerTests.cs");
        }
    }
}
