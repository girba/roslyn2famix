namespace Famix
{
    using Exceptions;
    using Famix.Language;
    using System.Collections.Generic;

    public class FamixTreeBuilder
    {
        private readonly Stack<IFamixLanguageNode> currentNodeStack;
        private IFamixLanguageNode lastNode;

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

            var popedNode = currentNodeStack.Pop();
            lastNode = popedNode;
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
            this.EndNode<Project>(name);
        }

        public void BeginAssembly(string name)
        {
            var currentNode = currentNodeStack.Peek() as Project;
            if (currentNode == null)
            {
                throw new UnexpectedNodeTypeException<Project>(currentNodeStack.Peek());
            }

            var assembly = new Assembly(name);
            currentNode.Assemblies.Add(assembly);

            currentNodeStack.Push(assembly);
        }

        public void EndAssembly(string name)
        {
            this.EndNode<Assembly>(name);
        }

        public void BeginNamespace(string name)
        {
            var @namespace = new Namespace(name);

            if (currentNodeStack.Count != 0)
            {
                var currentAssembly = currentNodeStack.Peek() as Assembly;
                if (currentAssembly != null)
                {
                    currentAssembly.Namespaces.Add(@namespace);
                }
                else
                {
                    var currentNamepace = currentNodeStack.Peek() as Namespace;
                    if (currentNamepace != null)
                    {
                        currentNamepace.Namespaces.Add(@namespace);
                    }
                    else
                    {
                        throw new UnexpectedNodeTypeException<Assembly>(currentNodeStack.Peek());
                    }
                }
            }

            currentNodeStack.Push(@namespace);
        }

        public void EndNamespace(string name)
        {
            this.EndNode<Namespace>(name);
        }

        public void BeginClass(string name)
        {
            var @class = new Class(name);

            if (currentNodeStack.Count != 0)
            {
                var currentNamepace = currentNodeStack.Peek() as Namespace;
                if (currentNamepace != null)
                {
                    currentNamepace.Classes.Add(@class);
                }
                else
                {
                    var currentClass = currentNodeStack.Peek() as Class;
                    if (currentClass != null)
                    {
                        currentClass.Classes.Add(@class);
                    }
                    else
                    {
                        throw new UnexpectedNodeTypeException<Class>(currentNodeStack.Peek());
                    }
                }
            }

            currentNodeStack.Push(@class);
        }

        public void EndClass(string name)
        {
            this.EndNode<Class>(name);
        }

        public void BeginMethod(string name)
        {
            var currentNode = currentNodeStack.Peek() as Class;
            if (currentNode == null)
            {
                throw new UnexpectedNodeTypeException<Class>(currentNodeStack.Peek());
            }

            var method = new Method(name);
            currentNode.Methods.Add(method);

            currentNodeStack.Push(method);
        }

        public void EndMethod(string name)
        {
            this.EndNode<Method>(name);
        }

        public string ToFamixString()
        {
            return lastNode?.ToString();
        }

        private void EndNode<T>(string name) where T : class, IFamixLanguageNode
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

            if (currentNodeStack.Count == 0)
            {
                lastNode = popedNode;
            }
        }
    }
}
