using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                                             .WithCloseBraceToken(SyntaxFactory
                                                                      .MissingToken(SyntaxKind.CloseBraceToken));

            return RemoveMagicalNumber(block, root);
        }

        /// <summary>
        /// Recurrent function for removeing bagical number from last statement in given BlockSyntax.
        /// </summary>
        /// <param name="oldBlock">BlockSyntax in which a statement is defined</param>
        /// <param name="root">Root in which a statement was defind orignally</param>
        /// <returns>New BlockSyntax with removed magical number in a statement</returns>
        private BlockSyntax RemoveMagicalNumber(BlockSyntax oldBlock, SyntaxNode root) {
            StatementSyntax node = oldBlock.Statements.Last();

            // Find all magical numbers in a statement
            var numericalNode = node.DescendantNodes()
                                     .FirstOrDefault(n => n.IsKind(SyntaxKind.NumericLiteralExpression));

            if ( numericalNode != null ) {
                var expresion = (LocalDeclarationStatementSyntax)SyntaxFactory
                                                                 .ParseStatement($"var {FindFreeVariableName(root, oldBlock)} = {numericalNode.ToFullString()};")
                                                                 .WithLeadingTrivia(node.GetLeadingTrivia())
                                                                 .WithTrailingTrivia(SyntaxFactory
                                                                                         .ParseTrailingTrivia("\n"));
                
                return RemoveMagicalNumber(oldBlock.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia)
                                                   .AddStatements(expresion)
                                                   .AddStatements(node.ReplaceNode(numericalNode,
                                                                                   SyntaxFactory.IdentifierName(expresion.Declaration
                                                                                                                         .Variables[0]
                                                                                                                         .Identifier))),
                                           root);
            } else {
                return oldBlock;
            }
        }

        /// <summary>
        /// Finds first possible variable name.
        /// </summary>
        /// <param name="root">Root in which used names should be searched</param>
        /// <param name="block">BlockSyntax in which used names should be searched</param>
        /// <returns>First avaliable name which starts with "magicalNumber"</returns>
        private string FindFreeVariableName(SyntaxNode root, SyntaxNode block) {
            for ( int i = 1; true; ++i ) {
                if ( root.DescendantTokens()
                         .Concat(block.DescendantTokens())
                         .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
                         .All(identifier => identifier.Text != $"magicalNumber{i}") )
                    return $"magicalNumber{i}";
            }
        }

        /// <inheritdoc />
        public SyntaxTree Refactor(SyntaxNode nodeToRefactor) {
            // Retrive statement from given node
            while ( !(nodeToRefactor is StatementSyntax) )
                nodeToRefactor = nodeToRefactor.Parent;

            return nodeToRefactor.SyntaxTree.GetRoot()
                                 .ReplaceNode(nodeToRefactor,
                                              VisitStatement((StatementSyntax)nodeToRefactor,
                                                             nodeToRefactor.SyntaxTree.GetRoot()))
                                 .SyntaxTree;
        }
    }
}