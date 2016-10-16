namespace Famix.Language
{
    using Famix.Language.Contracts;
    using System.Collections.Generic;
    using System.Text;

    public class Class : IFamixNode, IFamixContainer<Class>, IFamixContainer<Method>
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

        public void Add(Class @class)
        {
            this.Classes.Add(@class);
        }

        public void Add(Method method)
        {
            this.Methods.Add(method);
        }

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
