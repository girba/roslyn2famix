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

            this.Namespaces = new List<Namespace>();
        }

        public IList<Namespace> Namespaces { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Project {this.name})");

            foreach (var @namespace in this.Namespaces)
            {
                famixBuilder.AppendLine(@namespace.ToString());
            }

            famixBuilder.AppendLine($"(Project {this.name} end)");

            return famixBuilder.ToString();
        }
    }
}
