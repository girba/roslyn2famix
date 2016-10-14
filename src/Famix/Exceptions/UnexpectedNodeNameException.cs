using System;
using System.Runtime.Serialization;
using Famix.Language;
using Famix.Exceptions;

namespace Famix
{
    [Serializable]
    public class UnexpectedNodeNameException<T> : FamixTreeException where T : IFamixLanguageNode
    {
        public UnexpectedNodeNameException()
        {
        }

        public UnexpectedNodeNameException(string message) : base(message)
        {
        }

        public UnexpectedNodeNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnexpectedNodeNameException(T node, string name) : base(CreateMessage(node, name))
        {
            this.Node = node;
            this.Name = name;
        }

        private static string CreateMessage(T node, string name)
        {
            return $"Unexpected name of a {typeof(T).Name}. Expected \"{node.Name}\" but was \"{name}\"";
        }

        protected UnexpectedNodeNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string Name { get; }

        public T Node { get; }
    }
}