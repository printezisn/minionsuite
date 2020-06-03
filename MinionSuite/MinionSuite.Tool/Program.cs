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
            builder.AppendLine("  servicegen:\t\tgenerates a service layer based on a model class.");
            builder.AppendLine("  servicegen:test:\tgenerates tests for the service layer based on a model class.");
            builder.AppendLine("  pagemodel:\t\tgenerates a model to handle paging for queryables.");
            builder.AppendLine("  resultmodel:\t\tgenerates a result model that represents the result of a service operation.");
            builder.AppendLine("  mvccontroller:\tgenerates an MVC controller with CRUD operations on a model class.");

            Console.WriteLine(builder.ToString());
        }
    }
}
