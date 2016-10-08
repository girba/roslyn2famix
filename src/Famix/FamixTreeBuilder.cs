using Famix.Language;

namespace Famix
{
    public class FamixTreeBuilder
    {
        private Class rootClass = null;

        public void CreateNamespace(string name)
        {
        }

        public void CreateClass(string name)
        {
            this.rootClass = new Class(name);
        }

        public void CreateMethod(string name)
        {
            var method = new Method(name);

            this.rootClass?.Methods.Add(method);
        }

        public string ToFamixString()
        {
            return rootClass?.ToString();
        }
    }
}
