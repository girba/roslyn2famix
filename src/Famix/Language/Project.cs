namespace Famix.Language
{
    using Famix.Language.Contracts;
    using System.Collections.Generic;
    using System.Text;

    public class Project : IFamixNode, IFamixContainer<Assembly>
    {
        public Project(string name)
        {
            this.Name = name;

            this.Assemblies = new List<Assembly>();
        }

        public string Name { get; }

        public IList<Assembly> Assemblies { get; }

        public void Add(Assembly assembly)
        {
            this.Assemblies.Add(assembly);
        }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Project {this.Name})");

            foreach (var assembly in this.Assemblies)
            {
                famixBuilder.Append(assembly.ToString());
            }

            famixBuilder.AppendLine($"(Project {this.Name} end)");

            return famixBuilder.ToString();
        }
    }
}
