using System;
using System.Text;
using System.Linq;
using MinionSuite.Tool.Extensions;
using MinionSuite.Tool.Helpers;
using MinionSuite.Tool.Properties;

namespace MinionSuite.Tool.Generators
{
    /// <summary>
    /// Generates tests for the service layer of a model
    /// </summary>
    public class ServiceTestGenerator : IGenerator
    {
        /// <summary>
        /// Generates tests for the service layer of a model
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        public void Generate(ArgReader argReader)
        {
            var metadata = new ModelMetadata(argReader.ModelPath);
            var builder = new StringBuilder();

            var filledProperties = metadata.Properties
                .Where(w => w.Key != metadata.KeyName && w.Key != "CreatedAt" && w.Key != "UpdatedAt");
            var firstFilledProperty = filledProperties.First();
            var filledStringProperties = filledProperties.Where(w => w.Value is StringProperty);

            builder
                .AppendNestedLine(0, "using System;")
                .AppendNestedLine(0, "using System.Linq;")
                .AppendNestedLine(0, "using System.Threading.Tasks;")
                .AppendNestedLine(0, "using Microsoft.EntityFrameworkCore;")
                .AppendNestedLine(0, $"using {metadata.Namespace};")
                .AppendNestedLine(0, "using Xunit;")
                .AppendLine()
                .AppendNestedLine(0, $"namespace {argReader.Namespace}")
                .AppendNestedLine(0, "{")
                .AppendNestedLine(1, $"public class {metadata.Name}ServiceTests : IDisposable")
                .AppendNestedLine(1, "{")
                .AppendNestedLine(2, $"private {argReader.DbContext} _context;")
                .AppendNestedLine(2, $"private {metadata.Name}Service _service;")
                .AppendLine()
                .AppendNestedLine(2, $"public {metadata.Name}ServiceTests()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var options = new DbContextOptionsBuilder<{argReader.DbContext}>()")
                .AppendNestedLine(4, $".UseInMemoryDatabase(\"{metadata.Name}Service\")")
                .AppendNestedLine(4, ".Options;")
                .AppendNestedLine(3, $"_context = new {argReader.DbContext}(options);")
                .AppendNestedLine(3, $"_service = new {metadata.Name}Service(_context);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "public void Dispose()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "_context.Database.EnsureDeleted();")
                .AppendNestedLine(3, "_context.Dispose();")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task TestCreateAndReturnEntities()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await CreateEntity();")
                .AppendNestedLine(3, "var entities = await _service.GetAllAsync();")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Single(entities);");
            foreach (var property in filledProperties)
            {
                builder
                    .AppendNestedLine(3, $"Assert.Equal(entity.{property.Key}, entities[0].{property.Key});");
            }
            builder
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task TestUpdateEntity()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await CreateEntity();")
                .AppendLine()
                .AppendNestedLine(3, $"entity.{firstFilledProperty.Key} = {firstFilledProperty.Value.DefaultValue(2)};")
                .AppendNestedLine(3, "await _service.UpdateAsync(entity);")
                .AppendLine()
                .AppendNestedLine(3, $"entity = await _service.GetAsync(entity.{metadata.KeyName});")
                .AppendLine()
                .AppendNestedLine(3, "Assert.NotNull(entity);")
                .AppendNestedLine(3, $"Assert.Equal({firstFilledProperty.Value.DefaultValue(2)}, entity.{firstFilledProperty.Key});")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task TestDeleteEntity()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await CreateEntity();")
                .AppendNestedLine(3, "await _service.DeleteAsync(entity);")
                .AppendLine()
                .AppendNestedLine(3, $"entity = await _service.GetAsync(entity.{metadata.KeyName});")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Null(entity);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task TestPagingAndSorting()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "await CreateEntity();")
                .AppendNestedLine(3, "var entity = await CreateEntity();")
                .AppendNestedLine(3, $"entity.{firstFilledProperty.Key} = {firstFilledProperty.Value.DefaultValue(2)};")
                .AppendNestedLine(3, "await _service.UpdateAsync(entity);")
                .AppendLine()
                .AppendNestedLine(3, $"var page = await _service.GetAllAsync(1, 1, \"{firstFilledProperty.Key}\", false);")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(2, page.TotalItems);")
                .AppendNestedLine(3, "Assert.Equal(1, page.Page);")
                .AppendNestedLine(3, "Assert.Equal(1, page.PageSize);")
                .AppendNestedLine(3, $"Assert.Equal(\"{firstFilledProperty.Key}\", page.SortField);")
                .AppendNestedLine(3, "Assert.False(page.IsAscending);")
                .AppendNestedLine(3, "Assert.Equal(entity.Id, page.First().Id);")
                .AppendNestedLine(2, "}");
            if (filledStringProperties.Any())
            {
                builder
                    .AppendLine()
                    .AppendNestedLine(2, "[Fact]")
                    .AppendNestedLine(2, "public async Task TestSearch()")
                    .AppendNestedLine(2, "{")
                    .AppendNestedLine(3, "await CreateEntity();")
                    .AppendNestedLine(3, "var secondEntity = await CreateEntity();");
                foreach (var property in filledStringProperties)
                {
                    builder
                        .AppendNestedLine(3, $"secondEntity.{property.Key} = {property.Value.DefaultValue(2)};");
                }
                builder
                    .AppendNestedLine(3, "await _service.UpdateAsync(secondEntity);")
                    .AppendLine()
                    .AppendNestedLine(3, $"var entities = await _service.SearchAsync({filledStringProperties.First().Value.DefaultValue(2)});")
                    .AppendLine()
                    .AppendNestedLine(3, "Assert.Single(entities);")
                    .AppendNestedLine(3, "Assert.Equal(secondEntity.Id, entities.First().Id);")
                    .AppendNestedLine(2, "}")
                    .AppendLine()
                    .AppendNestedLine(2, "[Fact]")
                    .AppendNestedLine(2, "public async Task TestSearchWithPagingAndSorting()")
                    .AppendNestedLine(2, "{")
                    .AppendNestedLine(3, "await CreateEntity();")
                    .AppendNestedLine(3, "var secondEntity = await CreateEntity();")
                    .AppendNestedLine(3, "var thirdEntity = await CreateEntity();");
                foreach (var property in filledStringProperties)
                {
                    builder
                        .AppendNestedLine(3, $"thirdEntity.{property.Key} = {property.Value.DefaultValue(2)};");
                }
                builder
                    .AppendNestedLine(3, "await _service.UpdateAsync(thirdEntity);")
                    .AppendLine()
                    .AppendNestedLine(3, $"var page = await _service.SearchAsync({filledStringProperties.First().Value.DefaultValue(1)}, 1, 1, "
                        + $"\"{metadata.KeyName}\", false);")
                    .AppendLine()
                    .AppendNestedLine(3, "Assert.Equal(2, page.TotalItems);")
                    .AppendNestedLine(3, "Assert.Equal(1, page.Page);")
                    .AppendNestedLine(3, "Assert.Equal(1, page.PageSize);")
                    .AppendNestedLine(3, $"Assert.Equal(\"{metadata.KeyName}\", page.SortField);")
                    .AppendNestedLine(3, "Assert.False(page.IsAscending);")
                    .AppendNestedLine(3, "Assert.Equal(secondEntity.Id, page.First().Id);")
                    .AppendNestedLine(2, "}");
            }
            builder
                .AppendLine()
                .AppendNestedLine(2, $"private async Task<{metadata.Name}> CreateEntity()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var entity = new {metadata.Name}()")
                .AppendNestedLine(3, "{");
            foreach (var property in filledProperties)
            {
                builder
                    .AppendNestedLine(4, $"{property.Key} = {property.Value.DefaultValue(1)},");
            }
            builder
                .AppendNestedLine(3, "};")
                .AppendLine()
                .AppendNestedLine(3, "var result = await _service.CreateAsync(entity);")
                .AppendNestedLine(3, "return result.Result;")
                .AppendNestedLine(2, "}")
                .AppendNestedLine(1, "}")
                .AppendNestedLine(0, "}");

            FileHelper.SaveToOutput(argReader.OutputFolder, $"{metadata.Name}ServiceTests.cs", builder.ToString());
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder
                .AppendLine("Usage: minionsuite servicegen:test [parameters]")
                .AppendLine()
                .AppendLine("Generates tests for the service layer based on a model class.")
                .AppendLine()
                .AppendLine("Parameters:")
                .AppendLine("  -m|--model-path <path>:\t\t\tThe path to the model class.")
                .AppendLine("  -ns|--namespace <name>:\t\t\tThe namespace of the generated classes.")
                .AppendLine("  -o|--output <path>:\t\t\t\tThe path to the output folder (default: .).")
                .AppendLine("  -db <class name>|--db-context <class name>:\tThe database context class.");

            Console.WriteLine(builder.ToString());
        }

    }
}
