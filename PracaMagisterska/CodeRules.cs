using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PracaMagisterska.WPF.Utils;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF {
    public static class CodeRules {
        /// <summary>
        /// This method concatinate all of custom diagnostics
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <returns>All diagnostic found</returns>
        public static IEnumerable<DiagnosticHelper> GetAllCustomDiagnostic(this SyntaxNode root)
            => root.FindMagicalNumbersInExpresions()
                   .Concat(root.FindMethodsNotUsingAllParameters());

        /// <summary>
        /// This method finds all magical number used in code
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <returns>Diagnostics with magical number</returns>
        public static IEnumerable<DiagnosticHelper> FindMagicalNumbersInExpresions(this SyntaxNode root)
            => root.DescendantNodes()
                   .Where(node => Settings.SyntaxKindsForMagicalNumberSearch.Any(kind => kind == node.Kind()))
                   .Where(node => node.DescendantTokens().Any(s => s.Kind() == SyntaxKind.NumericLiteralToken))
                   .Select(node => DiagnosticHelper.Create(node, "Numerical literal found in expression"));

        /// <summary>
        /// This method finds all method which is not using all of its parameters
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <returns>Diagnostics with methods which is not using all of its parameters</returns>
        public static IEnumerable<DiagnosticHelper> FindMethodsNotUsingAllParameters(this SyntaxNode root)
            => root.DescendantNodes()
                   .OfType<MethodDeclarationSyntax>()
                   .Where(method => !method.ParameterList.Parameters
                                           .All(parameter => method.Body.Statements
                                                                   .SelectMany(statement=> statement.DescendantTokens())
                                                                   .Select(s => s.ValueText)
                                                                   .Distinct()
                                                                   .Contains(parameter.Identifier.ValueText)))
                   .Select(method => DiagnosticHelper.Create(method, "Method not using all given parrameters"));
    }
}