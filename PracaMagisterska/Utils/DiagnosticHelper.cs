using Microsoft.CodeAnalysis;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Diagnostic helper class used by DiagnosticListView in <see cref="SourceCode"/>
    /// </summary>
    public class DiagnosticHelper {
        /// <summary>
        /// Id of information
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Type of information
        /// </summary>
        public Type Severity { get; set; }

        /// <summary>
        /// Location in code
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Information itself
        /// </summary>
        public string Information { get; set; }

        /// <summary>
        /// Possible type of information
        /// </summary>
        public enum Type {
            ExecutionError = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }
    }
}