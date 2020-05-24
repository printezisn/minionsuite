using System;
using System.Text;

namespace MinionSuite.Tool.Generators
{
    /// <summary>
    /// Generates a page model
    /// </summary>
    public class PageModelGenerator : IGenerator
    {
        /// <summary>
        /// Generates a page model
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        public void Generate(ArgReader argReader)
        {
            var builder = new StringBuilder();

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("using Microsoft.EntityFrameworkCore;");
            builder.AppendLine();
            builder.AppendLine($"namespace {argReader.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine("    public class PageModel<T>");
            builder.AppendLine("    {");
            builder.AppendLine("        public IEnumerable<T> Collection { get; private set; }");
            builder.AppendLine("        public int TotalItems { get; private set; }");
            builder.AppendLine("        public int TotalPages { get; private set; }");
            builder.AppendLine("        public int Page { get; private set; }");
            builder.AppendLine("        public int PageSize { get; private set; }");
            builder.AppendLine();
            builder.AppendLine("        public bool IsFirstPage");
            builder.AppendLine("        {");
            builder.AppendLine("            get => Page == 1;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public bool IsLastPage");
            builder.AppendLine("        {");
            builder.AppendLine("            get => Page == TotalPages;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        private PageModel()");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public static async Task<PageModel<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)");
            builder.AppendLine("        {");
            builder.AppendLine("            var pageModel = new PageModel<T>();");
            builder.AppendLine();
            builder.AppendLine("            pageModel.PageSize = pageSize;");
            builder.AppendLine("            pageModel.TotalItems = query.Count();");
            builder.AppendLine("            pageModel.TotalPages = (int)Math.Ceiling(pageModel.TotalItems / (double)pageModel.PageSize);");
            builder.AppendLine("            pageModel.Page = Math.Max(1, page);");
            builder.AppendLine("            pageModel.Page = Math.Min(pageModel.Page, pageModel.TotalPages);");
            builder.AppendLine("            pageModel.Collection = await query.Skip((pageModel.Page - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToListAsync();");
            builder.AppendLine();
            builder.AppendLine("            return pageModel;");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            Helper.SaveToOutput(argReader.OutputFolder, "PageModel.cs", builder.ToString());
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Usage: minionsuite pagemodel [parameters]");
            builder.AppendLine();
            builder.AppendLine("Generates a model to handle paging for queryables.");
            builder.AppendLine();
            builder.AppendLine("Parameters:");
            builder.AppendLine("  -ns|--namespace <name>:\tThe namespace of the generated class.");
            builder.AppendLine("  -o|--output <path>:\tThe path to the output folder (default: .).");

            Console.WriteLine(builder.ToString());
        }
    }
}
