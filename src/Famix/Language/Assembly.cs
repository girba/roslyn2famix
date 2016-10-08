namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Assembly : IFamixLanguageNode
    {
        private readonly string name;

        public Assembly(string name)
        {
            this.name = name;

            this.Namespaces = new List<Namespace>();
        }

        public IList<Namespace> Namespaces { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Assembly {this.name})");

            foreach (var @namespace in this.Namespaces)
            {
                famixBuilder.AppendLine(@namespace.ToString());
            }

            famixBuilder.AppendLine($"(Assembly {this.name} end)");

            return famixBuilder.ToString();
        }
    }
}
