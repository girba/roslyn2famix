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

            builder.BeginSolution();

            foreach (var project in solution.Projects)
            {
                builder.BeginProject(project.Name);

                var compilation = project.GetCompilationAsync().Result;

                walker.Visit(compilation.Assembly);

                builder.EndProject(project.Name);
            }

            builder.EndSolution();
        }

        public string ExportToString()
        {
            return builder.ToFamixString();
        }
    }
}
