namespace Famix
{
    using Famix.Language;

    public class FamixTreeBuilder
    {
        private Class rootClass = null;

        public void BeginSolution()
        {
        }

        public void EndSolution()
        {
        }

        public void BeginProject(string name)
        {
        }

        public void EndProject(string name)
        {
        }

        public void BeginAssembly(string name)
        {
        }

        public void EndAssembly(string name)
        {
        }

        public void BeginNamespace(string name)
        {
        }

        public void EndNamespace(string name)
        {
        }

        public void BeginClass(string name)
        {
            this.rootClass = new Class(name);
        }

        public void EndClass(string name)
        {
            if (this.rootClass.Name != name)
            {
                throw new UnexpectedNodeNameException<Class>(this.rootClass, name);
            }
        }

        public void BeginMethod(string name)
        {
            var method = new Method(name);

            this.rootClass?.Methods.Add(method);
        }

        public void EndMethod(string name)
        {
        }

        public string ToFamixString()
        {
            return rootClass?.ToString();
        }
    }
}
