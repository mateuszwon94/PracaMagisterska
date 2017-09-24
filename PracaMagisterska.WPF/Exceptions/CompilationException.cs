using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis;

namespace PracaMagisterska.WPF.Exceptions {
    public class CompilationException : Exception {
        public CompilationException(IEnumerable<Diagnostic> failures) => Failures = failures;

        public CompilationException(string message, IEnumerable<Diagnostic> failures) : base(message) => Failures = failures;

        public CompilationException(string message, Exception innerException, IEnumerable<Diagnostic> failures) : base(message, innerException) => Failures = failures;

        protected CompilationException(SerializationInfo info, StreamingContext context, IEnumerable<Diagnostic> failures) : base(info, context) => Failures = failures;

        public IEnumerable<Diagnostic> Failures { get; }
    }
}