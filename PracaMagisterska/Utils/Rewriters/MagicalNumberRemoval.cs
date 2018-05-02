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
    public class MagicalNumberRemoval : IRefactor {
        /// <summary>
        /// Visit Expression Statement and removes from it magical numbers
        /// </summary>
        /// <param name="node">Visited statement</param>
        /// <returns>New block of code with replaced magical number in expresion</returns>
        public SyntaxNode VisitStatement(StatementSyntax node, SyntaxNode root) {
            BlockSyntax block = SyntaxFactory.Block(node)
                                             .WithOpenBraceToken(SyntaxFactory.MissingToken(SyntaxKind.OpenBraceToken))
                                             .WithCloseBraceToken(SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken));
            
            return RemoveMagicalNumber(block, root);
        }
        
        private BlockSyntax RemoveMagicalNumber(BlockSyntax oldBlock, SyntaxNode root) {
            StatementSyntax node = oldBlock.Statements.Last();

            List<SyntaxNode> numericalNodes = node.DescendantNodes()
                                                  .Where(n => n.Kind() == SyntaxKind.NumericLiteralExpression)
                                                  .ToList();

            if ( numericalNodes.Count > 0 ) {
                var expresion = (LocalDeclarationStatementSyntax)SyntaxFactory.ParseStatement($"var {FindFreeVariableName(root, oldBlock)} = {numericalNodes[0].ToFullString()};")
                                                                              .WithLeadingTrivia(node.GetLeadingTrivia())
                                                                              .WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia("\n"));

                return RemoveMagicalNumber(oldBlock.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia)
                                                   .AddStatements(expresion)
                                                   .AddStatements(node.ReplaceNode(numericalNodes[0],
                                                                                   SyntaxFactory.IdentifierName(expresion.Declaration
                                                                                                                         .Variables[0]
                                                                                                                         .Identifier))),
                                                   root);
            } else {
                return oldBlock;
            }
        }

        private string FindFreeVariableName(SyntaxNode root, SyntaxNode block) {
            for ( int i = 1; true ; ++i ) {
                if ( root.DescendantTokens()
                         .Concat(block.DescendantTokens())
                         .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
                         .All(identifier => identifier.Text != $"magicalNumber{i}") )
                    return $"magicalNumber{i}";
            }
        }

        /// <inheritdoc />
        public SyntaxTree Refactor(SyntaxNode nodeToRefactor) {
            while ( !(nodeToRefactor is StatementSyntax) ) 
                nodeToRefactor = nodeToRefactor.Parent;

            return nodeToRefactor.SyntaxTree.GetRoot()
                          .ReplaceNode(nodeToRefactor, VisitStatement((StatementSyntax)nodeToRefactor,
                                                                      nodeToRefactor.SyntaxTree.GetRoot()))
                          .SyntaxTree;
        }
    }
}