namespace Famix
{
    using Exceptions;
    using Famix.Language;
    using System.Collections.Generic;

    public class FamixTreeBuilder
    {
        private Class rootClass = null;
        private readonly Stack<IFamixLanguageNode> currentNodeStack;

        public FamixTreeBuilder()
        {
            this.currentNodeStack = new Stack<IFamixLanguageNode>();
        }

        public void BeginSolution()
        {
            if (this.currentNodeStack.Count != 0)
            {
                throw new FamixTreeException("Solution node already exist.");
            }

            var solution = new Solution();

            currentNodeStack.Push(solution);
        }

        public void EndSolution()
        {
            var currentNode = currentNodeStack.Peek() as Solution;
            if (currentNode == null)
            {
                throw new UnexpectedNodeTypeException<Solution>(currentNodeStack.Peek());
            }

            if (this.currentNodeStack.Count != 1)
            {
                throw new FamixTreeException("Solution must be the root node.");
            }

            currentNodeStack.Pop();
        }

        public void BeginProject(string name)
        {
            var currentNode = currentNodeStack.Peek() as Solution;
            if (currentNode == null)
            {
                throw new UnexpectedNodeTypeException<Solution>(currentNodeStack.Peek());
            }

            var project = new Project(name);
            currentNode.Projects.Add(project);

            currentNodeStack.Push(project);
        }

        public void EndProject(string name)
        {
            var currentNode = currentNodeStack.Peek() as Project;
            if (currentNode == null)
            {
                throw new UnexpectedNodeTypeException<Project>(currentNodeStack.Peek());
            }

            if (currentNode.Name != name)
            {
                throw new UnexpectedNodeNameException<Project>(currentNode, name);
            }

            currentNodeStack.Pop();
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
