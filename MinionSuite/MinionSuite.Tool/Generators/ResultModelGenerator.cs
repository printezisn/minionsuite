using System;
using System.Text;
using MinionSuite.Tool.Extensions;
using MinionSuite.Tool.Helpers;

namespace MinionSuite.Tool.Generators
{
    /// <summary>
    /// Generates a result model
    /// </summary>
    public class ResultModelGenerator : IGenerator
    {
        /// <summary>
        /// Generates a result model
        /// </summary>
        /// <param name="argReader">Information fetched from the command line arguments</param>
        public void Generate(ArgReader argReader)
        {
            var builder = new StringBuilder();

            builder
                .AppendNestedLine(0, "using System;")
                .AppendNestedLine(0, "using System.Collections.Generic;")
                .AppendNestedLine(0, "using System.Linq;")
                .AppendLine()
                .AppendNestedLine(0, $"namespace {argReader.Namespace}")
                .AppendNestedLine(0, "{")
                .AppendNestedLine(1, "public class ResultModel<T>")
                .AppendNestedLine(1, "{")
                .AppendNestedLine(2, "public T Result { get; private set; }")
                .AppendNestedLine(2, "public IEnumerable<string> Errors { get; private set; }")
                .AppendLine()
                .AppendNestedLine(2, "public bool IsSuccess => !Errors.Any();")
                .AppendLine()
                .AppendNestedLine(2, "public ResultModel(T result, IEnumerable<string> errors)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(3, "Result = result;")
                .AppendNestedLine(3, "Errors = errors;")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "public ResultModel(T result)")
                .AppendNestedLine(3, ": this(result, new List<string>())")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "public ResultModel(IEnumerable<string> errors)")
                .AppendNestedLine(3, ": this(default, errors)")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(2, "}")
                .AppendLine()
                .AppendNestedLine(2, "public ResultModel(string error)")
                .AppendNestedLine(3, ": this(default, new List<string>() { error })")
                .AppendNestedLine(2, "{")
                .AppendNestedLine(2, "}")
                .AppendNestedLine(1, "}")
                .AppendNestedLine(0, "}");

            FileHelper.SaveToOutput(argReader.OutputFolder, "ResultModel.cs", builder.ToString());
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder
                .AppendLine("Usage: minionsuite resultmodel [parameters]")
                .AppendLine()
                .AppendLine("Generates a result model that represents the result of a service operation.")
                .AppendLine()
                .AppendLine("Parameters:")
                .AppendLine("  -ns|--namespace <name>:\tThe namespace of the generated class.")
                .AppendLine("  -o|--output <path>:\tThe path to the output folder (default: .).");

            Console.WriteLine(builder.ToString());
        }
    }
}
