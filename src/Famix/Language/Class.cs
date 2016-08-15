namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Class : IFamixLanguageNode
    {
        private readonly string name;

        public Class(string name)
        {
            this.name = name;

            this.Methods = new List<Method>();
        }

        public IList<Method> Methods { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Class {this.name})");

            foreach (var method in this.Methods)
            {
                famixBuilder.AppendLine(method.ToString());
            }

            famixBuilder.AppendLine($"(Class {this.name} end)");

            return famixBuilder.ToString();
        }
    }
}
