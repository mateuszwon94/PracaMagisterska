using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PracaMagisterska.WPF.Utils.Rewriters {
    public class NotUsedParameterRemoval : CSharpSyntaxRewriter, IRefactor {
        /// <summary>
        /// Visit MethodDeclaration Statement and removes all not used parameters
        /// </summary>
        /// <param name="node">Visited statement</param>
        /// <returns>null, becouse statement should be removed</returns>
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
            ParameterListSyntax newParameterList = node.ParameterList.WithParameters(new SeparatedSyntaxList<ParameterSyntax>());
            foreach ( ParameterSyntax parameter in node.ParameterList.Parameters ) {
                if ( node.Body.Statements
                         .SelectMany(statement => statement.DescendantTokens())
                         .Select(s => s.ValueText)
                         .Distinct()
                         .Contains(parameter.Identifier.ValueText) )
                    newParameterList = newParameterList.AddParameters(parameter);
            }

            return node.WithParameterList(newParameterList);
        }

        /// <inheritdoc />
        public SyntaxTree Refactor(SyntaxNode nodeToRefactor)
            => nodeToRefactor.SyntaxTree.GetRoot()
                             .ReplaceNode(nodeToRefactor,
                                          VisitMethodDeclaration((MethodDeclarationSyntax)nodeToRefactor))
                             .SyntaxTree;
    }
}