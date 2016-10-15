namespace Famix.Language
{
    using System.Text;

    public class Method : IFamixLanguageNode
    {
        public Method(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Method {this.Name})");
            famixBuilder.AppendLine($"(Method {this.Name} end)");

            return famixBuilder.ToString();
        }
    }
}
