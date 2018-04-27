using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PracaMagisterska.WPF.Utils;
using PracaMagisterska.WPF.Utils.Rewriters;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF {
    public static class CodeRules {
        /// <summary>
        /// Counts lines of code for all method
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which methods are searched</param>
        /// <returns>LOC in methods in given <see cref="SyntaxNode"/></returns>
        public static Dictionary<string, int> GetLinesOfCodeByMethod(this SyntaxNode root)
            => root.DescendantNodes()
                   .OfType<MethodDeclarationSyntax>()
                   .ToDictionary(method => method.Identifier.ValueText,
                                 method => method.Body.SyntaxTree.GetLineSpan(method.FullSpan).EndLinePosition.Line -
                                           method.Body.SyntaxTree.GetLineSpan(method.FullSpan).StartLinePosition.Line);

        /// <summary>
        /// Counts statements for all method
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which methods are searched</param>
        /// <returns>Number of sttements in methods in given <see cref="SyntaxNode"/></returns>
        public static Dictionary<string, int> GetNumberOfStatementsByMethod(this SyntaxNode root)
            => root.DescendantNodes()
                   .OfType<MethodDeclarationSyntax>()
                   .ToDictionary(method => method.Identifier.ValueText,
                                 method => method.Body.Statements.Count);

        /// <summary>
        /// This method concatinate all of custom diagnostics
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <param name="semanticModel"><see cref="semanticModel"/> which is used to flow analysis</param>
        /// <returns>All diagnostic found</returns>
        public static IEnumerable<DiagnosticHelper> GetAllCustomDiagnostic(this SyntaxNode root, SemanticModel semanticModel)
            => root.FindMagicalNumbersInExpresions()
                   .Concat(root.FindMethodsNotUsingAllParameters())
                   .Concat(root.FindPossibleConstVariables(semanticModel))
                   .Concat(root.FindEmptyStatement());

        /// <summary>
        /// This method finds all magical number used in code
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <returns>Diagnostics with magical number</returns>
        public static IEnumerable<DiagnosticHelper> FindMagicalNumbersInExpresions(this SyntaxNode root)
            => root.DescendantNodes()
                   .Where(node => Settings.SyntaxKindsForMagicalNumberSearch.Any(kind => kind == node.Kind()))
                   .Where(node => node.DescendantTokens().Any(s => s.Kind() == SyntaxKind.NumericLiteralToken))
                   .Select(node => DiagnosticHelper.Create(node, "Numerical literal found in expression", null));

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
                                                                   .SelectMany(statement => statement.DescendantTokens())
                                                                   .Select(s => s.ValueText)
                                                                   .Distinct()
                                                                   .Contains(parameter.Identifier.ValueText)))
                   .Select(method => DiagnosticHelper.Create(method, "Method not using all given parrameters", new NotUsedParameterRemoval()));

        /// <summary>
        /// This method finds all empty statements
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <returns>Diagnostics with empty statements</returns>
        public static IEnumerable<DiagnosticHelper> FindEmptyStatement(this SyntaxNode root)
            => root.DescendantNodes()
                   .OfType<EmptyStatementSyntax>()
                   .Select(statement => DiagnosticHelper.Create(statement, "Empty statement", new EmtpyStatementRemoval()));

        /// <summary>
        /// This method finds all posible const value
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <param name="semanticModel"><see cref="semanticModel"/> which is used to flow analysis</param>
        /// <returns>Diagnostics with possible const values</returns>
        public static IEnumerable<DiagnosticHelper> FindPossibleConstVariables(
            this SyntaxNode root, SemanticModel semanticModel)
            => root.DescendantNodes()
                   .OfType<LocalDeclarationStatementSyntax>()
                   .Where(declaration => {
                       // already const?
                       if ( declaration.Modifiers.Any(SyntaxKind.ConstKeyword) )
                           return false;

                       // Ensure that all variables in the local declaration have initializers that
                       // are assigned with constant values.
                       foreach ( VariableDeclaratorSyntax variable in declaration.Declaration.Variables ) {
                           EqualsValueClauseSyntax initializer = variable.Initializer;
                           if ( initializer == null )
                               return false;

                           Optional<object> constantValue = semanticModel.GetConstantValue(initializer.Value);
                           if ( !constantValue.HasValue )
                               return false;

                           TypeSyntax variableTypeName = declaration.Declaration.Type;
                           ITypeSymbol variableType = semanticModel.GetTypeInfo(variableTypeName).ConvertedType;

                           // Ensure that the initializer value can be converted to the type of the
                           // local declaration without a user-defined conversion.
                           Conversion conversion = semanticModel.ClassifyConversion(initializer.Value, variableType);
                           if ( !conversion.Exists || conversion.IsUserDefined )
                               return false;

                           // Special cases:
                           //   * If the constant value is a string, the type of the local declaration must be System.String.
                           //   * If the constant value is null, the type of the local declaration must be a reference type.
                           if ( constantValue.Value is string ) {
                               if ( variableType.SpecialType != SpecialType.System_String )
                                   return false;
                           } else if ( variableType.IsReferenceType && constantValue.Value != null )
                               return false;
                       }

                       return true;
                   }).Select(declaration => DiagnosticHelper.Create(declaration, "Variable can be cons", new AddConstModifier(semanticModel)));
    }
}