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

                string variableName = "magicalNumber";
                for ( int i = 0; i < numericalNodes.Count; ++i ) {
                    LocalDeclarationStatementSyntax expresion = (LocalDeclarationStatementSyntax)SyntaxFactory.ParseStatement("var " + variableName + (i + 1) + " = " + numericalNodes[i].ToFullString() + ";")
                                                                                                 .WithTriviaFrom(node)
                                                                                                 .WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia("\n"));

                    statementList = statementList.AddStatements(expresion);

                    node = node.ReplaceNode(numericalNodes[i], SyntaxFactory.IdentifierName(expresion.Declaration.Variables[0].Identifier));
                }

                return statementList.AddStatements(node.WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia("\n")));
            } else {
                return node;
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