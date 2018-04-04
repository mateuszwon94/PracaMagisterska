using System;
using System.Dynamic;
using System.Linq;
using Microsoft.CodeAnalysis;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Diagnostic helper class used by DiagnosticListView in <see cref="SourceCode"/>
    /// </summary>
    public class DiagnosticHelper {
        public static DiagnosticHelper Create(Diagnostic diagnostic, Type severity = Type.None) {
            CodeLocation location;
            if ( diagnostic.Location.IsInSource ) {
                FileLinePositionSpan pos = diagnostic.Location.GetLineSpan();
                location    = new CodeLocation {
                    IsValid = true,
                    Line    = pos.StartLinePosition.Line + 1,
                    Column  = pos.StartLinePosition.Character + 1
                };
            } else {
                location    = new CodeLocation {
                    IsValid = false,
                    Line    = -1,
                    Column  = -1
                };
            }

            if ( severity == Type.None ) {
                if ( diagnostic.Severity == DiagnosticSeverity.Error )
                    severity = Type.Error;
                else if ( diagnostic.Severity == DiagnosticSeverity.Warning )
                    severity = Type.Warning;
                else if ( diagnostic.Severity == DiagnosticSeverity.Info )
                    severity = Type.Info;
            }

            return new DiagnosticHelper {
                Id          = diagnostic.Id,
                Severity    = severity,
                Location    = location,
                Information = diagnostic.GetMessage()
            };
        }

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
        public CodeLocation Location { get; set; } 

        /// <summary>
        /// Information itself
        /// </summary>
        public string Information { get; set; }

        /// <summary>
        /// Possible type of information
        /// </summary>
        public enum Type {
            None = 0,
            ExecutionError = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }

        public struct CodeLocation {
            public bool IsValid { get; set; }

            public int Line { get; set; }

            public int Column { get; set; }

            public override string ToString() => IsValid ? $"{Line}, {Column}" : "N/A";
        }
    }
}