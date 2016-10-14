namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Class : IFamixLanguageNode
    {
        public Class(string name)
        {
            this.Name = name;

            this.Methods = new List<Method>();
        }

        public string Name { get; }

        public IList<Method> Methods { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Class {this.Name})");

            foreach (var method in this.Methods)
            {
                famixBuilder.AppendLine(method.ToString());
            }

            famixBuilder.AppendLine($"(Class {this.Name} end)");

            return famixBuilder.ToString();
        }
    }
}
