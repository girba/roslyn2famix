namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Namespace : IFamixLanguageNode
    {
        private readonly string name;

        public Namespace(string name)
        {
            this.name = name;

            this.Classes = new List<Class>();
        }

        public IList<Class> Classes { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Namespace {this.name})");

            foreach (var @class in this.Classes)
            {
                famixBuilder.AppendLine(@class.ToString());
            }

            famixBuilder.AppendLine($"(Namespace {this.name} end)");

            return famixBuilder.ToString();
        }
    }
}
