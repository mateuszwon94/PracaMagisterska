using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PracaMagisterska.WPF.Utils;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF {
    public static class CodeRules {
        public static IEnumerable<DiagnosticHelper> FindMagicalNumbersInExpresions(this SyntaxNode root)
            => root.DescendantNodes()
                   .Where(node => Settings.SyntaxKindsForMagicalNumberSearch.Any(kind => kind == node.Kind()))
                   .Where(node => node.DescendantTokens().Any(s => s.Kind() == SyntaxKind.NumericLiteralToken))
                   .Select(node => DiagnosticHelper.Create(node, "Numerical literal found in expression"));
    }
}