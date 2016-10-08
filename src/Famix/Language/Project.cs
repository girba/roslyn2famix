namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Project : IFamixLanguageNode
    {
        private readonly string name;

        public Project(string name)
        {
            this.name = name;

            this.Assemblies = new List<Assembly>();
        }

        public IList<Assembly> Assemblies { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Project {this.name})");

            foreach (var assembly in this.Assemblies)
            {
                famixBuilder.AppendLine(assembly.ToString());
            }

            famixBuilder.AppendLine($"(Project {this.name} end)");

            return famixBuilder.ToString();
        }
    }
}
