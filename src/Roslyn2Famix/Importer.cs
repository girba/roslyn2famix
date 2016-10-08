namespace Roslyn2Famix
{
    using Famix;
    using Microsoft.CodeAnalysis.MSBuild;

    public class Importer
    {
        private readonly FamixTreeBuilder builder;
        private readonly SymbolWalker walker;

        public Importer()
        {
            this.builder = new FamixTreeBuilder();
            this.walker = new SymbolWalker(builder);
        }

        public void Run(string solutionPath)
        {
            var workspace = MSBuildWorkspace.Create();

            var solution = workspace.OpenSolutionAsync(solutionPath).Result;

            builder.CreateSolution();

            foreach (var project in solution.Projects)
            {
                builder.CreateProject(project.Name);

                var compilation = project.GetCompilationAsync().Result;

                walker.Visit(compilation.Assembly);
            }
        }

        public string ExportToString()
        {
            return builder.ToFamixString();
        }
    }
}
