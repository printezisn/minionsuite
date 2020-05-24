using System;
using System.Linq;
using System.Text;

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
            Helper.SaveToOutput(argReader.OutputFolder, $"I{metadata.Name}Service.cs", interfaceContent);

            string classContent = GetClassContent(argReader, metadata);
            Helper.SaveToOutput(argReader.OutputFolder, $"{metadata.Name}Service.cs", classContent);
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Usage: minionsuite servicegen [parameters]");
            builder.AppendLine();
            builder.AppendLine("Generates a service layer based on a model class.");
            builder.AppendLine();
            builder.AppendLine("Parameters:");
            builder.AppendLine("  -m|--model-path <path>:\tThe path to the model class.");
            builder.AppendLine("  -ns|--namespace <name>:\tThe namespace of the generated classes.");
            builder.AppendLine("  -o|--output <path>:\tThe path to the output folder (default: .).");
            builder.AppendLine("  -gpm|--generate-page-model:\tGenerate page model.");
            builder.AppendLine("  -grm|--generate-result-model:\tGenerate result model.");
            builder.AppendLine("  -db|--db-context:\tThe database context class.");

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

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine($"using {metadata.Namespace};");
            builder.AppendLine();
            builder.AppendLine($"namespace {argReader.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine($"    public interface I{metadata.Name}Service");
            builder.AppendLine("    {");
            builder.AppendLine($"        Task<ResultModel<{metadata.Name}>> CreateAsync({metadata.Name} entity);");
            builder.AppendLine($"        Task DeleteAsync({metadata.Name} entity);");
            builder.AppendLine($"        Task<{metadata.Name}> GetAsync({metadata.KeyType} key);");
            builder.AppendLine($"        Task<List<{metadata.Name}>> GetAllAsync();");
            builder.AppendLine($"        Task<PageModel<{metadata.Name}>> GetAllAsync(int page, int pageSize, string sortField, bool asc);");
            if (metadata.Properties.Values.Any(a => Helper.IsStringType(a)))
            {
                builder.AppendLine($"        Task<List<{metadata.Name}>> SearchAsync(string term);");
                builder.AppendLine($"        Task<PageModel<{metadata.Name}>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc);");
            }
            builder.AppendLine($"        Task<ResultModel<{metadata.Name}>> UpdateAsync({metadata.Name} entity);");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }

        private string GetClassContent(ArgReader argReader, ModelMetadata metadata)
        {
            var builder = new StringBuilder();

            string searchQuery = string.Join(" || ", metadata.Properties
                .Where(w => Helper.IsStringType(w.Value))
                .Select(s => $"w.{s.Key}.Contains(term)"));

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("using Microsoft.EntityFrameworkCore;");
            builder.AppendLine($"using {metadata.Namespace};");
            builder.AppendLine();
            builder.AppendLine($"namespace {argReader.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine($"    public class {metadata.Name}Service : I{metadata.Name}Service");
            builder.AppendLine("    {");
            builder.AppendLine($"        private {argReader.DbContext} _context;");
            builder.AppendLine();
            builder.AppendLine($"        public {metadata.Name}Service({argReader.DbContext} context)");
            builder.AppendLine("        {");
            builder.AppendLine("            _context = context;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        public virtual async Task<ResultModel<{metadata.Name}>> CreateAsync({metadata.Name} entity)");
            builder.AppendLine("        {");
            if (metadata.Properties.ContainsKey("CreatedAt"))
            {
                builder.AppendLine("            entity.CreatedAt = DateTime.UtcNow;");
            }
            if (metadata.Properties.ContainsKey("UpdatedAt"))
            {
                builder.AppendLine("            entity.UpdatedAt = DateTime.UtcNow;");
            }
            builder.AppendLine($"            _context.{metadata.PluralName}.Add(entity);");
            builder.AppendLine("            await _context.SaveChangesAsync();");
            builder.AppendLine();
            builder.AppendLine($"            return new ResultModel<{metadata.Name}>(entity);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        public virtual Task DeleteAsync({metadata.Name} entity)");
            builder.AppendLine("        {");
            builder.AppendLine($"            _context.{metadata.PluralName}.Remove(entity);");
            builder.AppendLine("            return _context.SaveChangesAsync();");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        public virtual Task<{metadata.Name}> GetAsync({metadata.KeyType} key)");
            builder.AppendLine("        {");
            builder.AppendLine($"            return _context.{metadata.PluralName}.FindAsync(key).AsTask();");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        public virtual Task<List<{metadata.Name}>> GetAllAsync()");
            builder.AppendLine("        {");
            builder.AppendLine($"            return _context.{metadata.PluralName}.ToListAsync();");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        public virtual Task<PageModel<{metadata.Name}>> GetAllAsync(int page, int pageSize, string sortField, bool asc)");
            builder.AppendLine("        {");
            builder.AppendLine("            var query = GetSortedQuery(sortField, asc);");
            builder.AppendLine();
            builder.AppendLine($"            return PageModel<{metadata.Name}>.CreateAsync(query, page, pageSize);");
            builder.AppendLine("        }");
            if (!string.IsNullOrEmpty(searchQuery))
            {
                builder.AppendLine();
                builder.AppendLine($"        public virtual Task<List<{metadata.Name}>> SearchAsync(string term)");
                builder.AppendLine("        {");
                builder.AppendLine($"            return _context.{metadata.PluralName}.Where(w => {searchQuery}).ToListAsync();");
                builder.AppendLine("        }");
                builder.AppendLine();
                builder.AppendLine($"        public virtual Task<PageModel<{metadata.Name}>> SearchAsync(string term, int page, int pageSize, string sortField, bool asc)");
                builder.AppendLine("        {");
                builder.AppendLine($"            var query = GetSortedQuery(sortField, asc).Where(w => {searchQuery});");
                builder.AppendLine();
                builder.AppendLine($"            return PageModel<{metadata.Name}>.CreateAsync(query, page, pageSize);");
                builder.AppendLine("        }");
            }
            builder.AppendLine();
            builder.AppendLine($"        public virtual async Task<ResultModel<{metadata.Name}>> UpdateAsync({metadata.Name} entity)");
            builder.AppendLine("        {");
            builder.AppendLine($"            var existingEntity = await _context.{metadata.PluralName}.FindAsync(entity.{metadata.KeyName}).AsTask();");
            builder.AppendLine("            if (existingEntity == null)");
            builder.AppendLine("            {");
            builder.AppendLine($"                return new ResultModel<{metadata.Name}>(\"The entity was not found.\");");
            builder.AppendLine("            }");
            builder.AppendLine();
            foreach (var property in metadata.Properties.Where(w => w.Key != metadata.KeyName && w.Key != "CreatedAt" && w.Key != "UpdatedAt"))
            {
                builder.AppendLine($"            existingEntity.{property.Key} = entity.{property.Key};");
            }
            if (metadata.Properties.ContainsKey("UpdatedAt"))
            {
                builder.AppendLine("            entity.UpdatedAt = DateTime.UtcNow;");
            }
            builder.AppendLine();
            builder.AppendLine("            await _context.SaveChangesAsync();");
            builder.AppendLine();
            builder.AppendLine($"            return new ResultModel<{metadata.Name}>(existingEntity);");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine($"        protected virtual IQueryable<{metadata.Name}> GetSortedQuery(string sortField, bool asc)");
            builder.AppendLine("        {");
            builder.AppendLine($"            var query = _context.{metadata.PluralName}.OrderBy(o => o.{metadata.KeyName});");
            builder.AppendLine();
            builder.AppendLine("            switch (sortField)");
            builder.AppendLine("            {");
            foreach (var property in metadata.Properties.Where(w => w.Key != metadata.KeyName))
            {
                builder.AppendLine($"                case \"{property.Key}\":");
                builder.AppendLine($"                    query = asc");
                builder.AppendLine($"                        ? _context.{metadata.PluralName}");
                builder.AppendLine($"                              .OrderBy(o => o.{property.Key})");
                builder.AppendLine($"                              .ThenBy(o => o.{metadata.KeyName})");
                builder.AppendLine($"                        : _context.{metadata.PluralName}");
                builder.AppendLine($"                              .OrderByDescending(o => o.{property.Key})");
                builder.AppendLine($"                              .ThenBy(o => o.{metadata.KeyName});");
                builder.AppendLine("                    break;");
            }
            builder.AppendLine("            }");
            builder.AppendLine();
            builder.AppendLine("            return query;");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}
