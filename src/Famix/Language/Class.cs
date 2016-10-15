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
            this.Classes = new List<Class>();
        }

        public string Name { get; }

        public IList<Method> Methods { get; }

        public IList<Class> Classes { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Class {this.Name})");

            foreach (var method in this.Methods)
            {
                famixBuilder.Append(method.ToString());
            }

            foreach (var @class in this.Classes)
            {
                famixBuilder.Append(@class.ToString());
            }

            famixBuilder.AppendLine($"(Class {this.Name} end)");

            return famixBuilder.ToString();
        }
    }
}
