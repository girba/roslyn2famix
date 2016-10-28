namespace Roslyn2Famix
{
    using Roslyn2FamixImporter;
    using System.IO;
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new CommandLineOptions();
            var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

            if (isValid && options.InputSolutionFile != null)
            {
                using (var outputFile = new StreamWriter(options.OutputFile))
                {
                    var importer = new Importer();

                    importer.Run(options.InputSolutionFile);
                    
                    var parsedFamix = importer.ExportToString();

                    outputFile.Write(parsedFamix);
                }
            }
        }
    }
}
