using System;
using System.Dynamic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PracaMagisterska.WPF.Utils.Rewriters;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Diagnostic helper class used by DiagnosticListView in <see cref="SourceCode"/>
    /// </summary>
    public class DiagnosticHelper {
        /// <summary>
        /// Create DiagnosticHelper instance based on Microsoft.CodeAnalysis.<see cref="Diagnostic"/> class.
        /// </summary>
        /// <param name="diagnostic">Diagnostic info, which is use to create helper object</param>
        /// <returns>Helper object based on <see cref="diagnostic"/></returns>
        public static DiagnosticHelper Create(Diagnostic diagnostic) {
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
                Severity    = severity,
                Location    = new CodeLocation(diagnostic.Location),
                Information = diagnostic.GetMessage(),
                SyntaxNode  = diagnostic.Location
                                        .SourceTree
                                        ?.GetRoot()
                                        .FindNode(diagnostic.Location.SourceSpan),
            };
        }

        public static DiagnosticHelper Create(SyntaxNode syntaxNode, string information, IRefactor refactor) {
            // Create helper object
            return new DiagnosticHelper {
                Severity    = SeverityType.Info,
                Location    = new CodeLocation(syntaxNode.GetLocation()),
                Information = information,
                Refactor    = refactor,
                SyntaxNode  = syntaxNode
            };
        }

        /// <summary>
        /// SeverityType of information
        /// </summary>
        public SeverityType Severity { get; private set; }

        /// <summary>
        /// Location in code
        /// </summary>
        public CodeLocation Location { get; private set; }

        /// <summary>
        /// Priority of diagnostic
        /// </summary>
        public int Priotiy => (int)Severity;

        /// <summary>
        /// Information itself
        /// </summary>
        public string Information { get; private set; }

        /// <summary>
        /// Object which represents way of refactoring this type of diagnostic
        /// </summary>
        public IRefactor Refactor { get; private set; } = null;

        /// <summary>
        /// SyntaxNode on which this diagnostic appears
        /// </summary>
        public SyntaxNode SyntaxNode { get; private set; } = null;

        /// <summary>
        /// Possible type of information
        /// </summary>
        public enum SeverityType {
            None = 0,
            ExecutionError = 1,
            Error = 2,
            Warning = 3,
            Info = 4,
        }
    }
}