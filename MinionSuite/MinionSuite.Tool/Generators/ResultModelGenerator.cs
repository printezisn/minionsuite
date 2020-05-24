using System;
using System.Text;

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

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine();
            builder.AppendLine($"namespace {argReader.Namespace}");
            builder.AppendLine("{");
            builder.AppendLine("    public class ResultModel<T>");
            builder.AppendLine("    {");
            builder.AppendLine("        public T Result { get; private set; }");
            builder.AppendLine("        public IEnumerable<string> Errors { get; private set; }");
            builder.AppendLine();
            builder.AppendLine("        public bool IsSuccess { get => !Errors.Any(); }");
            builder.AppendLine();
            builder.AppendLine("        public ResultModel(T result, IEnumerable<string> errors)");
            builder.AppendLine("        {");
            builder.AppendLine("            Result = result;");
            builder.AppendLine("            Errors = errors;");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public ResultModel(T result)");
            builder.AppendLine("            : this(result, new List<string>())");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public ResultModel(IEnumerable<string> errors)");
            builder.AppendLine("            : this(default, errors)");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine();
            builder.AppendLine("        public ResultModel(string error)");
            builder.AppendLine("            : this(default, new List<string>() { error })");
            builder.AppendLine("        {");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");

            Helper.SaveToOutput(argReader.OutputFolder, "ResultModel.cs", builder.ToString());
        }

        /// <summary>
        /// Displays a help message
        /// </summary>
        public void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Usage: minionsuite resultmodel [parameters]");
            builder.AppendLine();
            builder.AppendLine("Generates a result model that represents the result of a service operation.");
            builder.AppendLine();
            builder.AppendLine("Parameters:");
            builder.AppendLine("  -ns|--namespace <name>:\tThe namespace of the generated class.");
            builder.AppendLine("  -o|--output <path>:\tThe path to the output folder (default: .).");

            Console.WriteLine(builder.ToString());
        }
    }
}
