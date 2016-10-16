namespace Famix
{
    using Exceptions;
    using Famix.Language;
    using Famix.Language.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class FamixTreeBuilder
    {
        private readonly Stack<IFamixNode> currentNodeStack;
        private IFamixNode rootNode;

        public FamixTreeBuilder()
        {
            this.currentNodeStack = new Stack<IFamixNode>();
        }

        private IFamixNode CurrentNode => this.currentNodeStack.Pop();

        public void BeginSolution()
        {
            if (this.currentNodeStack.Any())
            {
                throw new FamixTreeException("Solution must be the topmost node.");
            }

            var solution = new Solution();

            this.AddChildNode(solution);
        }

        public void EndSolution()
        {
            this.EndNode<Solution>(string.Empty);
        }

        public void BeginProject(string name)
        {
            var project = new Project(name);

            this.AddChildNode(project);
        }

        public void EndProject(string name)
        {
            this.EndNode<Project>(name);
        }

        public void BeginAssembly(string name)
        {
            var assembly = new Assembly(name);

            this.AddChildNode(assembly);
        }

        public void EndAssembly(string name)
        {
            this.EndNode<Assembly>(name);
        }

        public void BeginNamespace(string name)
        {
            var @namespace = new Namespace(name);

            this.AddChildNode(@namespace);
        }

        public void EndNamespace(string name)
        {
            this.EndNode<Namespace>(name);
        }

        public void BeginClass(string name)
        {
            var @class = new Class(name);

            this.AddChildNode(@class);
        }

        public void EndClass(string name)
        {
            this.EndNode<Class>(name);
        }

        public void BeginMethod(string name)
        {
            var method = new Method(name);

            this.AddChildNode(method);
        }

        public void EndMethod(string name)
        {
            this.EndNode<Method>(name);
        }

        public string ToFamixString()
        {
            return rootNode?.ToString();
        }

        private void AddChildNode<T>(T node) where T : class, IFamixNode
        {
            if (currentNodeStack.Any())
            {
                var currentNode = currentNodeStack.Peek() as IFamixContainer<T>;

                if (currentNode == null)
                {
                    throw new UnexpectedNodeTypeException<T>(currentNode);
                }

                currentNode.Add(node);
            }

            if (!this.currentNodeStack.Any())
            {
                rootNode = node;
            }

            currentNodeStack.Push(node);
        }

        private void EndNode<T>(string name) where T : class, IFamixNode
        {
            var currentNode = currentNodeStack.Peek() as T;
            if (currentNode == null)
            {
                throw new UnexpectedNodeTypeException<T>(currentNodeStack.Peek());
            }

            if (currentNode.Name != name)
            {
                throw new UnexpectedNodeNameException<T>(currentNode, name);
            }

            var popedNode = currentNodeStack.Pop();
        }
    }
}
