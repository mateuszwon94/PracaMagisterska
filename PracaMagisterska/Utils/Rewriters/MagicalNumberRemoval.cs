using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;
using static PracaMagisterska.WPF.Utils.CompilationHelper;

namespace PracaMagisterska.WPF.Utils.Rewriters {
    public class MagicalNumberRemoval : CSharpSyntaxRewriter, IRefactor {
        /// <summary>
        /// Visit Expression Statement and removes from it magical numbers
        /// </summary>
        /// <param name="node">Visited statement</param>
        /// <returns>New block of code with replaced magical number in expresion</returns>
        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
            List<SyntaxNode> numericalNodes = node.DescendantNodes()
                                                  .Where(n => n.Kind() == SyntaxKind.NumericLiteralExpression)
                                                  .ToList();

            if ( numericalNodes.Count > 0 ) {
                BlockSyntax statementList = SyntaxFactory.Block()
                                                         .WithOpenBraceToken(SyntaxFactory.MissingToken(SyntaxKind.OpenBraceToken))
                                                         .WithCloseBraceToken(SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken));
                
                foreach ( SyntaxNode numericalNode in numericalNodes ) {
                    var expresion = (LocalDeclarationStatementSyntax)SyntaxFactory.ParseStatement($"var {FindFreeVariableName(node.SyntaxTree.GetRoot())} = {numericalNode.ToFullString()};")
                                                                                  .WithTriviaFrom(node)
                                                                                  .WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia("\n"));

                    statementList = statementList.AddStatements(expresion);

                    node = node.ReplaceNode(numericalNode, SyntaxFactory.IdentifierName(expresion.Declaration.Variables[0].Identifier));
                }

                return statementList.AddStatements(node.WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia("\n")));
            } else {
                return node;
            }
        }

        private string FindFreeVariableName(SyntaxNode root) {
            for ( int i = 1; true ; ++i ) {
                if ( root.DescendantTokens()
                         .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
                         .All(identifier => identifier.Text != $"magicalNumber{i}") )
                    return $"magicalNumber{i}";
            }
        }

        /// <inheritdoc />
        public SyntaxTree Refactor(SyntaxNode nodeToRefactor)
            => nodeToRefactor.SyntaxTree.GetRoot()
                             .ReplaceNode(nodeToRefactor.Parent,
                                          VisitExpressionStatement(SyntaxFactory.ExpressionStatement((ExpressionSyntax)nodeToRefactor)))
                             .SyntaxTree;
    }
}