using System;
using System.Dynamic;
using System.Linq;
using Microsoft.CodeAnalysis;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Diagnostic helper class used by DiagnosticListView in <see cref="SourceCode"/>
    /// </summary>
    public struct DiagnosticHelper {
        /// <summary>
        /// Create DiagnosticHelper instance based on Microsoft.CodeAnalysis.<see cref="Diagnostic"/> class.
        /// </summary>
        /// <param name="diagnostic">Diagnostic info, which is use to create helper object</param>
        /// <returns>Helper object based on <see cref="diagnostic"/></returns>
        public static DiagnosticHelper Create(Diagnostic diagnostic) {
            // Default location
            CodeLocation location = new CodeLocation {
                Location          = CodeLocation.LocationType.None,
                Line              = -1,
                Column            = -1
            };

            // Specific location based on lacation of diagnostic
            if ( diagnostic.Location.IsInSource ) {
                FileLinePositionSpan pos = diagnostic.Location.GetLineSpan();
                location.Location        = CodeLocation.LocationType.InSource;
                location.Line            = pos.StartLinePosition.Line + 1;
                location.Column          = pos.StartLinePosition.Character + 1;
            } else if ( diagnostic.Location.IsInMetadata ) {
                location.Location = CodeLocation.LocationType.InMetadata;
            }

            // Set proper severity type
            SeverityType severity = SeverityType.None;
            if ( diagnostic.Severity == DiagnosticSeverity.Error )
                severity = SeverityType.Error;
            else if ( diagnostic.Severity == DiagnosticSeverity.Warning )
                severity = SeverityType.Warning;
            else if ( diagnostic.Severity == DiagnosticSeverity.Info )
                severity = SeverityType.Info;

            // Create helper object
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
        /// SeverityType of information
        /// </summary>
        public SeverityType Severity { get; set; }

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
        public enum SeverityType {
            None = 0,
            ExecutionError = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }
    }
}