namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Assembly : IFamixLanguageNode
    {
        public Assembly(string name)
        {
            this.Name = name;

            this.Namespaces = new List<Namespace>();
        }

        public string Name { get; }

        public IList<Namespace> Namespaces { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Assembly {this.Name})");

            foreach (var @namespace in this.Namespaces)
            {
                famixBuilder.AppendLine(@namespace.ToString());
            }

            famixBuilder.AppendLine($"(Assembly {this.Name} end)");

            return famixBuilder.ToString();
        }
    }
}
