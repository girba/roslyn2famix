namespace Roslyn2Famix
{
    using CommandLine;

    public class CommandLineOptions
    {
        [Option('o', "output", Required = false,
          HelpText = "The name of the output Famix file.")]
        public string OutputFile { get; set; }

        [ValueOption(0)]
        public string InputSolutionFile { get; set; }
    }
}
