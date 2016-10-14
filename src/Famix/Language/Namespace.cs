namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Namespace : IFamixLanguageNode
    {
        public Namespace(string name)
        {
            this.Name = name;

            this.Classes = new List<Class>();
        }

        public string Name { get; }

        public IList<Class> Classes { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Namespace {this.Name})");

            foreach (var @class in this.Classes)
            {
                famixBuilder.AppendLine(@class.ToString());
            }

            famixBuilder.AppendLine($"(Namespace {this.Name} end)");

            return famixBuilder.ToString();
        }
    }
}
