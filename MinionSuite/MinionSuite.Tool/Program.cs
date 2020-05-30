using System;
using System.Text;
using MinionSuite.Tool.Generators;

namespace MinionSuite.Tool
{
    class Program
    {
        public static void Main(string[] args)
        {
            var argReader = new ArgReader(args);
            if (argReader.Generator == null)
            {
                ShowHelpMessage();
                return;
            }

            var generator = GeneratorFactory.GetGenerator(argReader);

            if (argReader.ShowHelp)
            {
                generator.ShowHelpMessage();
            }
            else
            {
                generator.Generate(argReader);
            }
        }

        private static void ShowHelpMessage()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Usage: dotnet minionsuite [generator] [parameters]");
            builder.AppendLine();
            builder.AppendLine("General purpose generators.");
            builder.AppendLine();
            builder.AppendLine("Generators:");
            builder.AppendLine("  servicegen:\tgenerates a service layer based on a model class.");
            builder.AppendLine("  servicegen:test:\tgenerates tests for the service layer based on a model class.");
            builder.AppendLine("  pagemodel:\tgenerates a model to handle paging for queryables.");
            builder.AppendLine("  resultmodel:\tgenerates a result model that represents the result of a service operation.");

            Console.WriteLine(builder.ToString());
        }
    }
}
