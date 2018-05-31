using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PracaMagisterska.WPF.Utils.Exceptions {
    public class BuildFailedException : Exception {
        public BuildFailedException(IEnumerable<DiagnosticHelper> diagnostics)
            => Diagnostics = diagnostics;

        public BuildFailedException(string message, IEnumerable<DiagnosticHelper> diagnostics) : base(message)
            => Diagnostics = diagnostics;

        public BuildFailedException(string message, Exception innerException, IEnumerable<DiagnosticHelper> diagnostics) : base(message, innerException)
            => Diagnostics = diagnostics;

        protected BuildFailedException(SerializationInfo             info,
                                       StreamingContext              context,
                                       IEnumerable<DiagnosticHelper> diagnostics) : base(info, context)
            => Diagnostics = diagnostics;

        public IEnumerable<DiagnosticHelper> Diagnostics { get; }
    }
}