using System;
using System.Runtime.Serialization;
using Famix.Language;
using Famix.Exceptions;

namespace Famix
{
    [Serializable]
    public class UnexpectedNodeTypeException<T> : FamixTreeException where T : IFamixLanguageNode
    {
        public UnexpectedNodeTypeException()
        {
        }

        public UnexpectedNodeTypeException(string message) : base(message)
        {
        }

        public UnexpectedNodeTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnexpectedNodeTypeException(IFamixLanguageNode node) : base(CreateMessage(node))
        {
            this.Node = node;
        }

        private static string CreateMessage(IFamixLanguageNode node)
        {
            return $"Unexpected node type. Expected \"{typeof(T).Name}\" but was \"{node.GetType().Name}\"";
        }

        protected UnexpectedNodeTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Type ExpectedNodeType => typeof(T);

        public IFamixLanguageNode Node { get; }
    }
}