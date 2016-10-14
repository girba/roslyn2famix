namespace Famix
{
    using Exceptions;
    using Famix.Language;

    public class FamixTreeBuilder
    {
        private Class rootClass = null;

        public void CreateSolution()
        {
        }

        public void CreateProject(string name)
        {
        }

        public void CreateAssembly(string name)
        {
        }

        public void CreateNamespace(string name)
        {
        }

        public void CreateClass(string name)
        {
            this.rootClass = new Class(name);
        }

        public void EndClass(string name)
        {
            if(this.rootClass.Name != name)
            {
                throw new UnexpectedNodeNameException<Class>(this.rootClass, name);
            }
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
