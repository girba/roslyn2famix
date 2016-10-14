using System;
using System.Runtime.Serialization;

namespace Famix.Exceptions
{
    [Serializable]
    public class FamixTreeException : Exception
    {
        public FamixTreeException()
        {
        }

        public FamixTreeException(string message) : base(message)
        {
        }

        public FamixTreeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FamixTreeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}