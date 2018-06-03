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
        /// Counts statements 
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which statements are enumerated</param>
        /// <returns>Number of sttements in given <see cref="SyntaxNode"/></returns>
        public static int GetNumberOfStatements(this SyntaxNode root) 
            => new StatementCounter().Calculate(root);

        /// <summary>
        /// This method concatinate all of custom diagnostics
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <param name="semanticModel"><see cref="semanticModel"/> which is used to flow analysis</param>
        /// <returns>All diagnostic found</returns>
        public static IEnumerable<DiagnosticHelper> GetAllCustomDiagnostic(this SyntaxNode root,
                                                                           SemanticModel   semanticModel)
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
                   .Where(node => Settings.SyntaxKindsForMagicalNumberSearch.Any(node.IsKind))
                   .Where(node => node.DescendantNodes().Any(s => s.IsKind(SyntaxKind.NumericLiteralExpression)))
                   .Select(node => DiagnosticHelper.Create(node, "Numerical literal in expression",
                                                           new MagicalNumberRemoval()));

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
                   .Select(method => DiagnosticHelper.Create(method, "Method not using all parrameters",
                                                             new NotUsedParameterRemoval()));

        /// <summary>
        /// This method finds all empty statements
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <returns>Diagnostics with empty statements</returns>
        public static IEnumerable<DiagnosticHelper> FindEmptyStatement(this SyntaxNode root)
            => root.DescendantNodes()
                   .OfType<EmptyStatementSyntax>()
                   .Select(statement => DiagnosticHelper.Create(statement, "Empty statement",
                                                                new EmtpyStatementRemoval()));

        /// <summary>
        /// This method finds all posible const value
        /// </summary>
        /// <param name="root"><see cref="SyntaxNode"/> in which diagnostics are searched</param>
        /// <param name="semanticModel"><see cref="semanticModel"/> which is used to flow analysis</param>
        /// <returns>Diagnostics with possible const values</returns>
        public static IEnumerable<DiagnosticHelper> FindPossibleConstVariables(this SyntaxNode root,
                                                                               SemanticModel   semanticModel)
            => root.DescendantNodes()
                   .OfType<LocalDeclarationStatementSyntax>()
                   .Where(declaration => !declaration.Modifiers.Any(SyntaxKind.ConstKeyword))
                   .Where(declaration => {
                       // Retrieve detailed information about the declared type of the local declaration
                       TypeSyntax variableTypeName = declaration.Declaration.Type;
                       ITypeSymbol variableType     = semanticModel.GetTypeInfo(variableTypeName).ConvertedType;

                       // Perform data flow analysis on the local declaration.
                       DataFlowAnalysis dataFlowAnalysis = semanticModel.AnalyzeDataFlow(declaration);

                       // Retrieve the local symbol for each variable in the local declaration
                       foreach ( VariableDeclaratorSyntax variable in declaration.Declaration.Variables ) {
                           // Ensure that variable has initializers that are assigned with constant values.
                           EqualsValueClauseSyntax initializer = variable.Initializer;
                           if ( initializer == null )
                               return false;

                           var constantValue = semanticModel.GetConstantValue(initializer.Value);
                           if ( !constantValue.HasValue )
                               return false;

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

                           // Ensure that variable is not written outside of the data flow analysis region.
                           ISymbol variableSymbol = semanticModel.GetDeclaredSymbol(variable);
                           if ( dataFlowAnalysis.WrittenOutside.Contains(variableSymbol) )
                               return false;
                       }

                       // Finally, it can be const
                       return true;
                   })
                   .Select(declaration => DiagnosticHelper.Create(declaration, "Variable can be const",
                                                                  new AddConstModifier(semanticModel)));
    }
}