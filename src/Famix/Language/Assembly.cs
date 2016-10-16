namespace Famix.Language
{
    using Famix.Language.Contracts;
    using System.Collections.Generic;
    using System.Text;

    public class Assembly : IFamixNode, IFamixContainer<Namespace>
    {
        public Assembly(string name)
        {
            this.Name = name;

            this.Namespaces = new List<Namespace>();
        }

        public string Name { get; }

        public IList<Namespace> Namespaces { get; }

        public void Add(Namespace assembly)
        {
            this.Namespaces.Add(assembly);
        }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Assembly {this.Name})");

            foreach (var @namespace in this.Namespaces)
            {
                famixBuilder.Append(@namespace.ToString());
            }

            famixBuilder.AppendLine($"(Assembly {this.Name} end)");

            return famixBuilder.ToString();
        }
    }
}
