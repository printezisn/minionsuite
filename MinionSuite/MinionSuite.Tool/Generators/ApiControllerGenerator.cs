using System;
using System.Linq;
using System.Text;
using MinionSuite.Tool.Extensions;
using MinionSuite.Tool.Helpers;

namespace MinionSuite.Tool.Generators
{
    /// <summary>
    /// Generates an API controller with CRUD operations for a model
    /// </summary>
    public class ApiControllerGenerator : IGenerator
    {
        /// <summary>
        /// Generates an API controller with CRUD operations for a model
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        public void Generate(ArgReader argReader)
        {
            var metadata = new ModelMetadata(argReader.ModelPath);
            var builder = new StringBuilder();
            var filledProperties = metadata.Properties
                .Where(w => w.Key != metadata.KeyName && w.Key != "CreatedAt" && w.Key != "UpdatedAt");

            builder
                .AppendNestedLine(0, "using System;")
                .AppendNestedLine(0, "using System.Threading.Tasks;")
                .AppendNestedLine(0, "using Microsoft.AspNetCore.Mvc;")
                .AppendNestedLine(0, $"using {metadata.Namespace};")
                .AppendLine()
                .AppendNestedLine(0, $"namespace {argReader.Namespace}")
                .AppendNestedLine(0, "{")
                .AppendNestedLine(1, "[Route(\"api/[controller]\")]")
                .AppendNestedLine(1, "[ApiController]")
                .AppendNestedLine(1, $"public class {metadata.PluralName}Controller : ControllerBase")
                .AppendNestedLine(1, "{")
                .AppendNestedLine(2, "private const int PAGE_SIZE = 20;")
                .AppendLine()
                .AppendNestedLine(2, $"private readonly I{metadata.Name}Service _service;")
                .AppendLine()
                .AppendNestedLine(2, $"public {metadata.PluralName}Controller(I{metadata.Name}Service service)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "_service = service;")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[HttpGet]")
                .AppendNestedLine(2, "public async Task<IActionResult> GetAll(string term, int page = 1, string sortField = \"\", bool asc = true)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entities = string.IsNullOrWhiteSpace(term)")
                .AppendNestedLine(4, "? await _service.GetAllAsync(page, PAGE_SIZE, sortField, asc)")
                .AppendNestedLine(4, ": await _service.SearchAsync(term, page, PAGE_SIZE, sortField, asc);")
                .AppendLine()
                .AppendNestedLine(3, "return Ok(entities);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[HttpGet(\"{id}\")]")
                .AppendNestedLine(2, $"public async Task<IActionResult> Get({metadata.KeyProperty.TypeName} id)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await _service.GetAsync(id);")
                .AppendNestedLine(3, "if (entity == null)")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, "return NotFound();")
                .AppendNestedLine(3, "}")
                .AppendLine()
                .AppendNestedLine(3, "return Ok(entity);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[HttpPost]")
                .AppendNestedLine(2, $"public async Task<IActionResult> Create({metadata.Name} entity)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var result = await _service.CreateAsync(entity);")
                .AppendLine()
                .AppendNestedLine(3, "return Ok(result);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[HttpPut(\"{id}\")]")
                .AppendNestedLine(2, $"public async Task<IActionResult> Update({metadata.Name} entity)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var result = await _service.UpdateAsync(entity);")
                .AppendNestedLine(3, "if (result == null)")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, "return NotFound();")
                .AppendNestedLine(3, "}")
                .AppendLine()
                .AppendNestedLine(3, "return Ok(result);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "[HttpDelete(\"{id}\")]")
                .AppendNestedLine(2, $"public async Task<IActionResult> Delete({metadata.KeyProperty.TypeName} id)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var result = await _service.DeleteAsync(id);")
                .AppendNestedLine(3, "if (result)")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, "return Ok();")
                .AppendNestedLine(3, "}")
                .AppendLine()
                .AppendNestedLine(3, "return NotFound();")
                .AppendNestedLine(2, "}")
                .AppendNestedLine(1, "}")
                .AppendNestedLine(0, "}");

            FileHelper.SaveToOutput(argReader.OutputFolder, $"{metadata.PluralName}Controller.cs", builder.ToString());
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder
                .AppendLine("Usage: minionsuite apicontroller [parameters]")
                .AppendLine()
                .AppendLine("Generates an API controller with CRUD operations on a model class.")
                .AppendLine()
                .AppendLine("Parameters:")
                .AppendLine("  -m|--model-path <path>:\tThe path to the model class.")
                .AppendLine("  -ns|--namespace <name>:\tThe namespace of the generated class.")
                .AppendLine("  -o|--output <path>:\t\tThe path to the output folder (default: .).");

            Console.WriteLine(builder.ToString());
        }
    }
}
