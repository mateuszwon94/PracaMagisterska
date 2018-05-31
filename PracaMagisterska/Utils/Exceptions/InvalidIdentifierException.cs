using System;
using System.Runtime.Serialization;

namespace PracaMagisterska.WPF.Utils.Exceptions {
    public class InvalidIdentifierException : Exception {
        public InvalidIdentifierException() { }

        public InvalidIdentifierException(string message) : base(message) { }

        public InvalidIdentifierException(string message, Exception innerException) : base(message, innerException) { }

        protected InvalidIdentifierException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}