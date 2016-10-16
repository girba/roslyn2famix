namespace Famix.Language
{
    using Famix.Language.Contracts;
    using System.Collections.Generic;
    using System.Text;

    public class Namespace : IFamixNode, IFamixContainer<Class>, IFamixContainer<Namespace>
    {
        public Namespace(string name)
        {
            this.Name = name;

            this.Classes = new List<Class>();
            this.Namespaces = new List<Namespace>();
        }

        public string Name { get; }

        public IList<Class> Classes { get; }

        public IList<Namespace> Namespaces { get; }

        public void Add(Class @class)
        {
            this.Classes.Add(@class);
        }

        public void Add(Namespace @namespace)
        {
            this.Namespaces.Add(@namespace);
        }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            var isEmptyNamespace = string.IsNullOrEmpty(this.Name);
            if (!isEmptyNamespace)
            {
                famixBuilder.AppendLine($"(Namespace {this.Name})");
            }
            else
            {
                famixBuilder.AppendLine($"(Namespace)");
            }

            foreach (var @class in this.Classes)
            {
                famixBuilder.Append(@class.ToString());
            }

            foreach (var @namespace in this.Namespaces)
            {
                famixBuilder.Append(@namespace.ToString());
            }

            if (!isEmptyNamespace)
            {
                famixBuilder.AppendLine($"(Namespace {this.Name} end)");
            }
            else
            {
                famixBuilder.AppendLine($"(Namespace end)");
            }

            return famixBuilder.ToString();
        }
    }
}
