using System;
using System.IO;
using MinionSuite.Tests.Helpers;
using MinionSuite.Tool;
using MinionSuite.Tool.Generators;
using Xunit;

namespace MinionSuite.Tests.Generators
{
    public class ServiceTestGeneratorTests
    {
        [Fact]
        public void TestServiceGenerator()
        {
            var args = new string[]
            {
                "servicegen:test", "-ns", "MinionSuite.Tests.Templates",
                "-m", "./Models/Post.cs", "-db", "ApplicationContext"
            };
            var argReader = new ArgReader(args);

            GeneratorFactory.GetGenerator(argReader).Generate(argReader);

            Assert.True(File.Exists("PostServiceTests.cs"));

            AssertHelper.AssertEqualFile("Templates/ServiceTest/PostServiceTests.cs", "PostServiceTests.cs");
        }
    }
}
