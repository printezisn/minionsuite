namespace MinionSuite.Tool
{
    /// <summary>
    /// Extracts certain information from the command line arguments
    /// </summary>
    public class ArgReader
    {
        /// <summary>
        /// The name of the generator to use
        /// </summary>
        public string Generator { get; private set; }

        /// <summary>
        /// The path to the model class
        /// </summary>
        public string ModelPath { get; private set; }

        /// <summary>
        /// The namespace of the generated file
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// The output folder
        /// </summary>
        public string OutputFolder { get; private set; } = ".";

        /// <summary>
        /// Indicates if a page model must be generated
        /// </summary>
        public bool GeneratePageModel { get; private set; }

        /// <summary>
        /// Indicates if a result model must be generated
        /// </summary>
        public bool GenerateResultModel { get; private set; }

        /// <summary>
        /// The database context class
        /// </summary>
        public string DbContext { get; private set; } = "ApplicationDbContext";

        /// <summary>
        /// Indicates if the help message must be shown
        /// </summary>
        public bool ShowHelp { get; private set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="args">The command line arguments to take as input</param>
        public ArgReader(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-m" || args[i] == "--model-path")
                {
                    if (i + 1 < args.Length)
                    {
                        ModelPath = args[i + 1];
                        i++;
                    }
                }
                else if (args[i] == "-ns" || args[i] == "--namespace")
                {
                    if (i + 1 < args.Length)
                    {
                        Namespace = args[i + 1];
                        i++;
                    }
                }
                else if (args[i] == "-o" || args[i] == "--output")
                {
                    if (i + 1 < args.Length)
                    {
                        OutputFolder = args[i + 1];
                        i++;
                    }
                }
                else if (args[i] == "-gpm" || args[i] == "--generate-page-model")
                {
                    GeneratePageModel = true;
                }
                else if (args[i] == "-grm" || args[i] == "--generate-result-model")
                {
                    GenerateResultModel = true;
                }
                else if (args[i] == "-db" || args[i] == "--db-context")
                {
                    if (i + 1 < args.Length)
                    {
                        DbContext = args[i + 1];
                        i++;
                    }
                }
                else if (args[i] == "-h" || args[i] == "--help")
                {
                    ShowHelp = true;
                }
                else if (Generator == null)
                {
                    Generator = args[i];
                }
            }
        }
    }
}