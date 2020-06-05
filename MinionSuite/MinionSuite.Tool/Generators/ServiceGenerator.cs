using System;
using System.Linq;
using System.Text;
using MinionSuite.Tool.Extensions;
using MinionSuite.Tool.Helpers;
using MinionSuite.Tool.Properties;

namespace MinionSuite.Tool.Generators
{
    /// <summary>
    /// Generates the service layer for a model
    /// </summary>
    public class ServiceGenerator : IGenerator
    {
        /// <summary>
        /// Generates the service layer for a model
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        public void Generate(ArgReader argReader)
        {
            if (argReader.GeneratePageModel)
            {
                new PageModelGenerator().Generate(argReader);
            }
            if (argReader.GenerateResultModel)
            {
                new ResultModelGenerator().Generate(argReader);
            }

            var metadata = new ModelMetadata(argReader.ModelPath);

            string interfaceContent = GetInterfaceContent(argReader, metadata);
            FileHelper.SaveToOutput(argReader.OutputFolder, $"I{metadata.Name}Service.cs", interfaceContent);

            string classContent = GetClassContent(argReader, metadata);
            FileHelper.SaveToOutput(argReader.OutputFolder, $"{metadata.Name}Service.cs", classContent);
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder
                .AppendLine("Usage: minionsuite servicegen [parameters]")
                .AppendLine()
                .AppendLine("Generates a service layer based on a model class.")
                .AppendLine()
                .AppendLine("Parameters:")
                .AppendLine("  -m|--model-path <path>:\t\t\tThe path to the model class.")
                .AppendLine("  -ns|--namespace <name>:\t\t\tThe namespace of the generated classes.")
                .AppendLine("  -o|--output <path>:\t\t\t\tThe path to the output folder (default: .).")
                .AppendLine("  -gpm|--generate-page-model:\t\t\tGenerate page model.")
                .AppendLine("  -grm|--generate-result-model:\t\t\tGenerate result model.")
                .AppendLine("  -db <class name>|--db-context <class name>:\tThe database context class.");

            Console.WriteLine(builder.ToString());
        }

        /// <summary>
        /// Returns the content of the interface
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        /// <param name="metadata">Metadata about the model</param>
        /// <returns>The content of the interface</returns>
        private string GetInterfaceContent(ArgReader argReader, ModelMetadata metadata)
        {
            var builder = new StringBuilder();

            builder
                .AppendNestedLine(0, "using System;")
                .AppendNestedLine(0, "using System.Collections.Generic;")
                .AppendNestedLine(0, "using System.Threading.Tasks;")
                .AppendNestedLine(0, $"using {metadata.Namespace};")
                .AppendLine()
                .AppendNestedLine(0, $"namespace {argReader.Namespace}")
                .AppendNestedLine(0, "{")
                .AppendNestedLine(1, $"public interface I{metadata.Name}Service")
                .AppendNestedLine(1, "{")
                .AppendNestedLine(2, $"Task<ResultModel<{metadata.Name}>> CreateAsync({metadata.Name} entity);")
                .AppendNestedLine(2, $"Task<bool> DeleteAsync({metadata.KeyProperty.TypeName} key);")
                .AppendNestedLine(2, $"Task<{metadata.Name}> GetAsync({metadata.KeyProperty.TypeName} key);")
                .AppendNestedLine(2, $"Task<List<{metadata.Name}>> GetAllAsync();")
                .AppendNestedLine(2, $"Task<PageModel<{metadata.Name}>> GetAllAsync(int page, int pageSize, string sortField, bool asc);");

            if (metadata.Properties.Values.Any(a => a is StringProperty))
            {
                builder
                    .AppendNestedLine(2, $"Task<List<{metadata.Name}>> SearchAsync(string term);")
                    .AppendNestedLine(2, $"Task<PageModel<{metadata.Name}>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc);");
            }

            builder
                .AppendNestedLine(2, $"Task<ResultModel<{metadata.Name}>> UpdateAsync({metadata.Name} entity);")
                .AppendNestedLine(1, "}")
                .AppendNestedLine(0, "}");

            return builder.ToString();
        }

        /// <summary>
        /// Returns the content of the class implementation
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        /// <param name="metadata">Metadata about the model</param>
        /// <returns>The content of the class implementation</returns>
        private string GetClassContent(ArgReader argReader, ModelMetadata metadata)
        {
            var builder = new StringBuilder();

            string searchQuery = string.Join(" || ", metadata.Properties.Values
                .Where(w => w is StringProperty)
                .Select(s => $"w.{s.Name}.Contains(term)"));

            builder
                .AppendNestedLine(0, "using System;")
                .AppendNestedLine(0, "using System.Collections.Generic;")
                .AppendNestedLine(0, "using System.Linq;")
                .AppendNestedLine(0, "using System.Threading.Tasks;")
                .AppendNestedLine(0, "using Microsoft.EntityFrameworkCore;")
                .AppendNestedLine(0, $"using {metadata.Namespace};")
                .AppendLine()
                .AppendNestedLine(0, $"namespace {argReader.Namespace}")
                .AppendNestedLine(0, "{")
                .AppendNestedLine(1, $"public class {metadata.Name}Service : I{metadata.Name}Service")
                .AppendNestedLine(1, "{")
                .AppendNestedLine(2, $"private readonly {argReader.DbContext} _context;")
                .AppendLine()
                .AppendNestedLine(2, $"public {metadata.Name}Service({argReader.DbContext} context)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "_context = context;")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"public virtual async Task<ResultModel<{metadata.Name}>> CreateAsync({metadata.Name} model)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var newEntity = new {metadata.Name}();")
                .AppendLine();

            foreach (var property in metadata.Properties.Where(w => w.Key != metadata.KeyName && w.Key != "CreatedAt" && w.Key != "UpdatedAt"))
            {
                builder.AppendNestedLine(3, $"newEntity.{property.Key} = model.{property.Key};");
            }
            if (metadata.Properties.ContainsKey("CreatedAt"))
            {
                builder.AppendNestedLine(3, "newEntity.CreatedAt = DateTime.UtcNow;");
            }
            if (metadata.Properties.ContainsKey("UpdatedAt"))
            {
                builder.AppendNestedLine(3, "newEntity.UpdatedAt = DateTime.UtcNow;");
            }

            builder
                .AppendLine()
                .AppendNestedLine(3, $"_context.{metadata.PluralName}.Add(newEntity);")
                .AppendNestedLine(3, "await _context.SaveChangesAsync();")
                .AppendLine()
                .AppendNestedLine(3, $"return new ResultModel<{metadata.Name}>(newEntity);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"public virtual async Task<bool> DeleteAsync({metadata.KeyProperty.TypeName} key)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var entity = await GetAsync(key);")
                .AppendNestedLine(3, "if (entity == null)")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, "return false;")
                .AppendNestedLine(3, "}")
                .AppendLine()
                .AppendNestedLine(3, $"_context.{metadata.PluralName}.Remove(entity);")
                .AppendNestedLine(3, "await _context.SaveChangesAsync();")
                .AppendLine()
                .AppendNestedLine(3, "return true;")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"public virtual Task<{metadata.Name}> GetAsync({metadata.KeyProperty.TypeName} key)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"return _context.{metadata.PluralName}.FindAsync(key).AsTask();")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"public virtual Task<List<{metadata.Name}>> GetAllAsync()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"return _context.{metadata.PluralName}.ToListAsync();")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"public virtual Task<PageModel<{metadata.Name}>> GetAllAsync(int page, int pageSize, string sortField, bool asc)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var query = _context.{metadata.PluralName}.AsNoTracking();")
                .AppendNestedLine(3, "query = GetSortedQuery(query, sortField, asc);")
                .AppendLine()
                .AppendNestedLine(3, $"return PageModel<{metadata.Name}>.CreateAsync(query, page, pageSize, sortField, asc);")
                .AppendNestedLine(2, "}");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                builder
                    .AppendLine()
                    .AppendNestedLine(2, $"public virtual Task<List<{metadata.Name}>> SearchAsync(string term)")
                    .AppendNestedLine(2, "{")
                    .AppendNestedLine(3, $"return _context.{metadata.PluralName}.Where(w => {searchQuery}).ToListAsync();")
                    .AppendNestedLine(2, "}")
                    .AppendLine()
                    .AppendNestedLine(2, $"public virtual Task<PageModel<{metadata.Name}>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc)")
                    .AppendNestedLine(2, "{")
                    .AppendNestedLine(3, $"var query = _context.{metadata.PluralName}.AsNoTracking().Where(w => {searchQuery});")
                    .AppendNestedLine(3, $"query = GetSortedQuery(query, sortField, asc);")
                    .AppendLine()
                    .AppendNestedLine(3, $"return PageModel<{metadata.Name}>.CreateAsync(query, page, pageSize, sortField, asc);")
                    .AppendNestedLine(2, "}");
            }

            builder
                .AppendLine()
                .AppendNestedLine(2, $"public virtual async Task<ResultModel<{metadata.Name}>> UpdateAsync({metadata.Name} model)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, $"var existingEntity = await GetAsync(model.Id);")
                .AppendNestedLine(3, "if (existingEntity == null)")
                .AppendNestedLine(3, "{")
                .AppendNestedLine(4, "return null;")
                .AppendNestedLine(3, "}")
                .AppendLine();

            foreach (var property in metadata.Properties.Where(w => w.Key != metadata.KeyName && w.Key != "CreatedAt" && w.Key != "UpdatedAt"))
            {
                builder.AppendNestedLine(3, $"existingEntity.{property.Key} = model.{property.Key};");
            }
            if (metadata.Properties.ContainsKey("UpdatedAt"))
            {
                builder.AppendNestedLine(3, "existingEntity.UpdatedAt = DateTime.UtcNow;");
            }

            builder
                .AppendLine()
                .AppendNestedLine(3, "await _context.SaveChangesAsync();")
                .AppendLine()
                .AppendNestedLine(3, $"return new ResultModel<{metadata.Name}>(existingEntity);")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, $"protected virtual IQueryable<{metadata.Name}> GetSortedQuery(IQueryable<{metadata.Name}> query, string sortField, bool asc)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "switch (sortField)")
                .AppendNestedLine(3, "{");

            foreach (var property in metadata.Properties.Where(w => w.Key != metadata.KeyName))
            {
                builder
                    .AppendNestedLine(4, $"case \"{property.Key}\":")
                    .AppendNestedLine(5, $"return asc")
                    .AppendNestedLine(6, "? query")
                    .AppendNestedLine(7, $".OrderBy(o => o.{property.Key})")
                    .AppendNestedLine(7, $".ThenBy(o => o.{metadata.KeyName})")
                    .AppendNestedLine(6, ": query")
                    .AppendNestedLine(7, $".OrderByDescending(o => o.{property.Key})")
                    .AppendNestedLine(7, $".ThenBy(o => o.{metadata.KeyName});");
            }

            builder
                .AppendNestedLine(4, "default:")
                .AppendNestedLine(5, $"return asc")
                .AppendNestedLine(6, $"? query.OrderBy(o => o.{metadata.KeyName})")
                .AppendNestedLine(6, $": query.OrderByDescending(o => o.{metadata.KeyName});")
                .AppendNestedLine(3, "}")
                .AppendNestedLine(2, "}")
                .AppendNestedLine(1, "}")
                .AppendNestedLine(0, "}");

            return builder.ToString();
        }
    }
}
