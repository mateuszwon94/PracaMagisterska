using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PracaMagisterska.WPF.Utils;

namespace PracaMagisterska.WPF {
    public static class CodeRules {
        public static IEnumerable<DiagnosticHelper> FindMagicalNumbersInExpresions(this SyntaxNode root) {
            return root.DescendantNodes()
                       .Where(node => kindsForMagicalNumbers_.Any(kind => kind == node.Kind()))
                       .Where(node => node.DescendantTokens().Any(s => s.Kind() == SyntaxKind.NumericLiteralToken))
                       .Select(node => DiagnosticHelper.Create(node, "Numerical literal found in expression"));
        }

        private static readonly List<SyntaxKind> kindsForMagicalNumbers_ = new List<SyntaxKind> {
            SyntaxKind.AddAssignmentExpression,      // +=
            SyntaxKind.SubtractAssignmentExpression, // -=
            SyntaxKind.MultiplyAssignmentExpression, // *=
            SyntaxKind.DivideAssignmentExpression,   // /=
            SyntaxKind.ModuloAssignmentExpression,   // %=
            SyntaxKind.AddExpression,                // +
            SyntaxKind.SubtractExpression,           // -
            SyntaxKind.MultiplyExpression,           // *
            SyntaxKind.DivideExpression,             // /
            SyntaxKind.ModuloExpression,             // %
        };
    }
}