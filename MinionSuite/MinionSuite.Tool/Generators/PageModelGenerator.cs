using System;
using System.Text;
using MinionSuite.Tool.Extensions;
using MinionSuite.Tool.Helpers;

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

            builder
                .AppendNestedLine(0, "using System;")
                .AppendNestedLine(0, "using System.Collections.Generic;")
                .AppendNestedLine(0, "using System.Linq;")
                .AppendNestedLine(0, "using System.Threading.Tasks;")
                .AppendNestedLine(0, "using Microsoft.EntityFrameworkCore;")
                .AppendLine()
                .AppendNestedLine(0, $"namespace {argReader.Namespace}")
                .AppendNestedLine(0, "{")
                .AppendNestedLine(1, "public class PageModel<T>")
                .AppendNestedLine(1, "{")
                .AppendNestedLine(2, "public IEnumerable<T> Collection { get; private set; }")
                .AppendNestedLine(2, "public int TotalItems { get; private set; }")
                .AppendNestedLine(2, "public int TotalPages { get; private set; }")
                .AppendNestedLine(2, "public int Page { get; private set; }")
                .AppendNestedLine(2, "public int PageSize { get; private set; }")
                .AppendLine()
                .AppendNestedLine(2, "public bool IsFirstPage => Page == 1;")
                .AppendNestedLine(2, "public bool IsLastPage => Page == TotalPages;")
                .AppendLine()
                .AppendNestedLine(2, "private PageModel()")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "public static async Task<PageModel<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "var pageModel = new PageModel<T>();")
                .AppendLine()
                .AppendNestedLine(3, "pageModel.PageSize = pageSize;")
                .AppendNestedLine(3, "pageModel.TotalItems = query.Count();")
                .AppendNestedLine(3, "pageModel.TotalPages = (int)Math.Ceiling(pageModel.TotalItems / (double)pageModel.PageSize);")
                .AppendNestedLine(3, "pageModel.Page = Math.Max(1, page);")
                .AppendNestedLine(3, "pageModel.Page = Math.Min(pageModel.Page, pageModel.TotalPages);")
                .AppendNestedLine(3, " pageModel.Collection = await query.Skip((pageModel.Page - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToListAsync();")
                .AppendLine()
                .AppendNestedLine(3, "return pageModel;")
                .AppendNestedLine(2, "}")
                .AppendNestedLine(1, "}")
                .AppendNestedLine(0, "}");

            FileHelper.SaveToOutput(argReader.OutputFolder, "PageModel.cs", builder.ToString());
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder
                .AppendLine("Usage: minionsuite pagemodel [parameters]")
                .AppendLine()
                .AppendLine("Generates a model to handle paging for queryables.")
                .AppendLine()
                .AppendLine("Parameters:")
                .AppendLine("  -ns|--namespace <name>:\tThe namespace of the generated class.")
                .AppendLine("  -o|--output <path>:\tThe path to the output folder (default: .).");

            Console.WriteLine(builder.ToString());
        }
    }
}
