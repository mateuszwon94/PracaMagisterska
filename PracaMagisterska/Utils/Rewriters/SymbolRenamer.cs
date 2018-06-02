using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using PracaMagisterska.WPF.Utils.Exceptions;

namespace PracaMagisterska.WPF.Utils.Rewriters {
    public static class SymbolRenamer {
        /// <summary>
        /// Rename all references to given SyntaxToken
        /// </summary>
        /// <param name="semanticModel">SemanticModel used to perform renaming</param>
        /// <param name="syntaxToken">SyntaxToken which will be renamed</param>
        /// <param name="newName">New name of given SyntaxToken</param>
        /// <returns></returns>
        public static SyntaxTree RenameSymbol(this SemanticModel semanticModel,
                                              SyntaxToken        syntaxToken,
                                              string             newName) {
            // Check if new identifier is valid
            if ( !SyntaxFacts.IsValidIdentifier(newName) )
                throw new InvalidIdentifierException();

            // Check if new identifier is not used
            if ( semanticModel.SyntaxTree
                              .GetRoot()
                              .DescendantTokens()
                              .Any(token => token.Text == newName) )
                throw new InvalidIdentifierException();

            // Get all spans in which identifier is used
            IEnumerable<TextSpan> renameSpans = semanticModel.GetSpansForRename(syntaxToken);

            // Actual renameing
            SourceText newSourceText = semanticModel.SyntaxTree
                                                    .GetText()
                                                    .WithChanges(renameSpans.Select(nameSpan => new TextChange(nameSpan,
                                                                                                               newName)));
                                                                                                               
            return semanticModel.SyntaxTree.WithChangedText(newSourceText);
        }

        /// <summary>
        /// Finds all references to given symbol and returns its TextSpans
        /// </summary>
        /// <param name="semanticModel">SemanticModel used to searching</param>
        /// <param name="syntaxToken">SyntaxToken with identifier</param>
        /// <returns></returns>
        private static IEnumerable<TextSpan> GetSpansForRename(this SemanticModel semanticModel,
                                                               SyntaxToken        syntaxToken) {
            // Get symbol from given SyntaxToken
            ISymbol symbol = semanticModel.GetSymbolInfo(syntaxToken.Parent).Symbol 
                             ?? semanticModel.GetDeclaredSymbol(syntaxToken.Parent);

            // If symbol was not found return empty sequence
            if ( symbol == null )
                return new TextSpan[0];

            // Get definition of symbol
            var definitions = symbol.Locations
                                    .Where(location => location.SourceTree == syntaxToken.SyntaxTree)
                                    .Select(location => location.SourceSpan);

            // Get usages of symbol
            var usages = semanticModel.SyntaxTree
                                      .GetRoot()
                                      .DescendantTokens()
                                      .Where(token => token.Text == symbol.Name)
                                      .Where(token => semanticModel.GetSymbolInfo(token.Parent).Symbol == symbol)
                                      .Select(token => token.Span);

            // It is enaught if symbol is not a NameType symbol
            if ( symbol.Kind != SymbolKind.NamedType )
                return definitions.Concat(usages);

            // If symbol is a NameType symbol, find all constructors and destructors
            var structors = from type in semanticModel.SyntaxTree
                                                      .GetRoot()
                                                      .DescendantNodes()
                                                      .OfType<TypeDeclarationSyntax>()
                            where type.Identifier.Text == symbol.Name
                            where semanticModel.GetDeclaredSymbol(type) == symbol
                            from method in type.Members
                            let constructor = method as ConstructorDeclarationSyntax
                            let destructor = method as DestructorDeclarationSyntax
                            where constructor != null || destructor != null
                            let identifier = constructor?.Identifier ?? destructor.Identifier
                            select identifier.Span;

            return definitions.Concat(usages).Concat(structors);
        }
    }
}