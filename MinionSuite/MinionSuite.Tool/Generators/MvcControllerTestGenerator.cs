using System;
using System.Linq;
using System.Text;
using MinionSuite.Tool.Extensions;
using MinionSuite.Tool.Helpers;
using MinionSuite.Tool.Properties;

namespace MinionSuite.Tool.Generators
{
    /// <summary>
    /// Generates tests for the MVC controller of a model
    /// </summary>
    public class MvcControllerTestGenerator : IGenerator
    {
        /// <summary>
        /// Generates tests for the MVC controller of a model
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        public void Generate(ArgReader argReader)
        {
            var metadata = new ModelMetadata(argReader.ModelPath);

            if (argReader.GenerateWebApplicationFactory)
            {
                var factoryContent = GetWebApplicationFactoryContent(argReader, metadata);
                FileHelper.SaveToOutput(argReader.OutputFolder, "CustomWebApplicationFactory.cs", factoryContent);
            }

            var testClassContent = GetTestContent(argReader, metadata);
            FileHelper.SaveToOutput(argReader.OutputFolder, $"{metadata.PluralName}ControllerTests.cs", testClassContent);
        }

        /// <summary>
        /// Returns the content of the test class
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        /// <param name="metadata">Metadata about the model</param>
        /// <returns>The content of the test class</returns>
        private string GetTestContent(ArgReader argReader, ModelMetadata metadata)
        {
            var builder = new StringBuilder();

            var stringProperties = metadata.Properties.Values.Where(w => w is StringProperty);
            var filledProperties = metadata.Properties.Where(w => w.Key != metadata.KeyName && w.Key != "CreatedAt" && w.Key != "UpdatedAt");

            builder
                .AppendNestedLine(0, "using System;")
                .AppendNestedLine(0, "using System.Collections.Generic;")
                .AppendNestedLine(0, "using System.Net;")
                .AppendNestedLine(0, "using System.Net.Http;")
                .AppendNestedLine(0, "using System.Text.RegularExpressions;")
                .AppendNestedLine(0, "using System.Threading;")
                .AppendNestedLine(0, "using System.Threading.Tasks;")
                .AppendNestedLine(0, "using System.Web;")
                .AppendNestedLine(0, "using Microsoft.AspNetCore.Mvc.Testing;")
                .AppendNestedLine(0, "using Microsoft.Extensions.DependencyInjection;")
                .AppendNestedLine(0, "using Newtonsoft.Json;")
                .AppendNestedLine(0, "using Xunit;")
                .AppendLine()
                .AppendNestedLine(0, $"namespace {argReader.Namespace}")
                .AppendNestedLine(0, "{")
                .AppendNestedLine(1, $"public class {metadata.PluralName}ControllerTests : IClassFixture<CustomWebApplicationFactory>")
                .AppendNestedLine(1, "{")
                .AppendNestedLine(2, "private readonly CustomWebApplicationFactory _factory;")
                .AppendNestedLine(2, "private readonly HttpClient _client;")
                .AppendNestedLine(2, "private readonly IServiceScope _scope;")
                .AppendNestedLine(2, $"private readonly I{metadata.Name}Service _service;")
                .AppendLine()
                .AppendNestedLine(2, $"public {metadata.PluralName}ControllerTests(CustomWebApplicationFactory factory)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "_factory = factory;")
                .AppendNestedLine(3, "_client = _factory.CreateClient(new WebApplicationFactoryClientOptions")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, "AllowAutoRedirect = false")
                .AppendNestedLine(3, "});")
                .AppendLine()
                .AppendNestedLine(3, "_scope = _factory.Services.CreateScope();")
                .AppendNestedLine(3, $"_service = _scope.ServiceProvider.GetRequiredService<I{metadata.Name}Service>();")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Index_Get_Successful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await BuildEntity(true);")
                .AppendLine()
                .AppendNestedLine(3, $"var response = await _client.GetAsync($\"/{metadata.PluralName}?asc=false\");")
                .AppendNestedLine(3, "var content = await response.Content.ReadAsStringAsync();")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.OK, response.StatusCode);")
                .AppendNestedLine(3, $"Assert.Contains($\"{metadata.PluralName}/Details/{{entity.{metadata.KeyName}}}\", content);")
                .AppendNestedLine(2, "}")
                .AppendLine();

            if (stringProperties.Any())
            {
                builder
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Index_Get_SearchSuccessful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var firstEntity = await BuildEntity(true);")
                .AppendNestedLine(3, "var secondEntity = await BuildEntity(true);")
                .AppendLine()
                .AppendNestedLine(3, $"var response = await _client.GetAsync($\"/{metadata.PluralName}?term={{HttpUtility.UrlEncode(firstEntity.{stringProperties.First().Name})}}\");")
                .AppendNestedLine(3, "var content = await response.Content.ReadAsStringAsync();")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.OK, response.StatusCode);")
                .AppendNestedLine(3, $"Assert.Contains($\"{metadata.PluralName}/Details/{{firstEntity.{metadata.KeyName}}}\", content);")
                .AppendNestedLine(3, $"Assert.DoesNotContain($\"{metadata.PluralName}/Details/{{secondEntity.{metadata.KeyName}}}\", content);")
                .AppendNestedLine(2, "}")
                .AppendLine();
            }

            builder
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Details_Get_NotFound()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var response = await _client.GetAsync(\"/{metadata.PluralName}/Details/0\");")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Details_Get_Successful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await BuildEntity(true);")
                .AppendNestedLine(3, $"var response = await _client.GetAsync($\"/{metadata.PluralName}/Details/{{entity.{metadata.KeyName}}}\");")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.OK, response.StatusCode);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Create_Get_Successful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var response = await _client.GetAsync(\"/{metadata.PluralName}/Create\");")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.OK, response.StatusCode);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Create_Post_Successful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var verificationToken = await GetVerificationTokenFromPage(\"/{metadata.PluralName}/Create\");")
                .AppendLine()
                .AppendNestedLine(3, "var entity = await BuildEntity(false);")
                .AppendNestedLine(3, "var dict = EntityToDictionary(entity);")
                .AppendNestedLine(3, "dict[\"__RequestVerificationToken\"] = verificationToken;")
                .AppendNestedLine(3, "var formContent = new FormUrlEncodedContent(dict);")
                .AppendLine()
                .AppendNestedLine(3, $"var response = await _client.PostAsync(\"/{metadata.PluralName}/Create\", formContent);")
                .AppendNestedLine(3, "var createdEntity = await FetchEntityFromDetailsUrl(response.Headers.Location.ToString());")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);")
                .AppendNestedLine(3, "Assert.NotNull(createdEntity);");

            foreach (var property in filledProperties)
            {
                builder.AppendNestedLine(3, $"Assert.Equal(entity.{property.Key}, createdEntity.{property.Key});");
            }

            builder
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Edit_Get_NotFound()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var response = await _client.GetAsync(\"/{metadata.PluralName}/Edit/0\");")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Edit_Get_Successful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await BuildEntity(true);")
                .AppendNestedLine(3, $"var response = await _client.GetAsync($\"/{metadata.PluralName}/Edit/{{entity.{metadata.KeyName}}}\");")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.OK, response.StatusCode);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Edit_Post_Successful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await BuildEntity(true);")
                .AppendNestedLine(3, $"var verificationToken = await GetVerificationTokenFromPage($\"/{metadata.PluralName}/Edit/{{entity.{metadata.KeyName}}}\");")
                .AppendLine()
                .AppendNestedLine(3, "var dict = EntityToDictionary(await BuildEntity(false));")
                .AppendNestedLine(3, $"dict[\"{metadata.KeyName}\"] = entity.{metadata.KeyName}.ToString();")
                .AppendNestedLine(3, "dict[\"__RequestVerificationToken\"] = verificationToken;")
                .AppendNestedLine(3, "var formContent = new FormUrlEncodedContent(dict);")
                .AppendLine()
                .AppendNestedLine(3, $"var response = await _client.PostAsync(\"/{metadata.PluralName}/Edit\", formContent);")
                .AppendNestedLine(3, $"var updatedEntity = await _service.GetAsync(entity.{metadata.KeyName});")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);")
                .AppendNestedLine(3, "Assert.NotNull(updatedEntity);");
            foreach (var property in filledProperties)
            {
                builder.AppendNestedLine(3, $"Assert.Equal(entity.{property.Key}, updatedEntity.{property.Key});");
            }
            builder
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Delete_Get_NotFound()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var response = await _client.GetAsync(\"/{metadata.PluralName}/Delete/0\");")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Delete_Get_Successful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await BuildEntity(true);")
                .AppendNestedLine(3, $"var response = await _client.GetAsync($\"/{metadata.PluralName}/Delete/{{entity.{metadata.KeyName}}}\");")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.OK, response.StatusCode);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[Fact]")
                .AppendNestedLine(2, "public async Task Delete_Post_Successful()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await BuildEntity(true);")
                .AppendNestedLine(3, $"var verificationToken = await GetVerificationTokenFromPage($\"/{metadata.PluralName}/Delete/{{entity.{metadata.KeyName}}}\");")
                .AppendLine()
                .AppendNestedLine(3, "var dict = new Dictionary<string, string>")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, $"[\"{metadata.KeyName}\"] = entity.{metadata.KeyName}.ToString(),")
                .AppendNestedLine(4, "[\"__RequestVerificationToken\"] = verificationToken")
                .AppendNestedLine(3, "};")
                .AppendNestedLine(3, "var formContent = new FormUrlEncodedContent(dict);")
                .AppendLine()
                .AppendNestedLine(3, $"var response = await _client.PostAsync(\"/{metadata.PluralName}/Delete\", formContent);")
                .AppendNestedLine(3, $"entity = await _service.GetAsync(entity.{metadata.KeyName});")
                .AppendLine()
                .AppendNestedLine(3, "Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);")
                .AppendNestedLine(3, "Assert.Null(entity);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"private async Task<{metadata.Name}> BuildEntity(bool save)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var entity = new {metadata.Name}()")
                .AppendNestedLine(3, "{");

            foreach (var property in filledProperties)
            {
                builder.AppendNestedLine(4, $"{property.Key} = {property.Value.DefaultValue()},");
            }

            builder
                .AppendNestedLine(3, "};")
                .AppendLine()
                .AppendNestedLine(3, "if (save)")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, "var result = await _service.CreateAsync(entity);")
                .AppendNestedLine(4, "return result.Result;")
                .AppendNestedLine(3, "}")
                .AppendLine()
                .AppendNestedLine(3, "return entity;")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"private Dictionary<string, string> EntityToDictionary({metadata.Name} entity)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var jsonContent = JsonConvert.SerializeObject(entity);")
                .AppendNestedLine(3, "return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "private async Task<string> GetVerificationTokenFromPage(string pageUrl)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var response = await _client.GetAsync(pageUrl);")
                .AppendNestedLine(3, "var content = await response.Content.ReadAsStringAsync();")
                .AppendNestedLine(3, "var verificationTokenMatch = Regex.Match(content, \"name=\\\"__RequestVerificationToken\\\"(.*?)value=\\\"(.+?)\\\"\");")
                .AppendLine()
                .AppendNestedLine(3, "Assert.True(verificationTokenMatch.Success);")
                .AppendLine()
                .AppendNestedLine(3, "return verificationTokenMatch.Groups[2].Value;")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"private async Task<{metadata.Name}> FetchEntityFromDetailsUrl(string url)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var match = Regex.Match(url, \"/{metadata.PluralName}/Details/(.+)\");")
                .AppendNestedLine(3, "if (!match.Success)")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, "return null;")
                .AppendNestedLine(3, "}")
                .AppendLine()
                .AppendNestedLine(3, "var key = int.Parse(match.Groups[1].Value);")
                .AppendNestedLine(3, "return await _service.GetAsync(key);")
                .AppendNestedLine(2, "}")
                .AppendNestedLine(1, "}")
                .AppendNestedLine(0, "}");

            return builder.ToString();
        }

        /// <summary>
        /// Returns the content of the web application factory
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        /// <param name="metadata">Metadata about the model</param>
        /// <returns>The content of the web application factory</returns>
        private string GetWebApplicationFactoryContent(ArgReader argReader, ModelMetadata metadata)
        {
            var builder = new StringBuilder();

            builder
                .AppendNestedLine(0, "using System;")
                .AppendNestedLine(0, "using Microsoft.AspNetCore.Hosting;")
                .AppendNestedLine(0, "using Microsoft.AspNetCore.Mvc.Testing;")
                .AppendLine()
                .AppendNestedLine(0, $"namespace {argReader.Namespace}")
                .AppendNestedLine(0, "{")
                .AppendNestedLine(1, "public class CustomWebApplicationFactory : WebApplicationFactory<Startup>")
                .AppendNestedLine(1, "{")
                .AppendNestedLine(2, "protected override void ConfigureWebHost(IWebHostBuilder builder)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "builder.UseEnvironment(\"Test\");")
                .AppendNestedLine(2, "}")
                .AppendNestedLine(1, "}")
                .AppendNestedLine(0, "}");

            return builder.ToString();
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder
                .AppendLine("Usage: minionsuite mvccontroller:test [parameters]")
                .AppendLine()
                .AppendLine("Generates tests for the MVC controller of a model class.")
                .AppendLine()
                .AppendLine("Parameters:")
                .AppendLine("  -m|--model-path <path>:\t\t\tThe path to the model class.")
                .AppendLine("  -ns|--namespace <name>:\t\t\tThe namespace of the generated class.")
                .AppendLine("  -o|--output <path>:\t\t\t\tThe path to the output folder (default: .).")
                .AppendLine("  -gwaf|--generate-web-application-factory:\tGenerate web application factory.");

            Console.WriteLine(builder.ToString());
        }
    }
}
