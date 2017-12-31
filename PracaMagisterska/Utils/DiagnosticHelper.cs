using Microsoft.CodeAnalysis;

namespace PracaMagisterska.WPF.Utils {
    public class DiagnosticHelper {
        public string Id { get; set; }
        public Type Severity { get; set; }
        public string Location { get; set; }
        public string Information { get; set; }

        public enum Type {
            ExecutionError = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }

    }
}