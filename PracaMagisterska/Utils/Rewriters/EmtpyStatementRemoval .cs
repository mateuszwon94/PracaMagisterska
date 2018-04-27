using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PracaMagisterska.WPF.Utils.Rewriters {
    public class EmtpyStatementRemoval : CSharpSyntaxRewriter, IRefactor {
        /// <summary>
        /// Visit Empty Statement and removes it
        /// </summary>
        /// <param name="node">Visited statement</param>
        /// <returns>null, becouse statement should be removed</returns>
        public override SyntaxNode VisitEmptyStatement(EmptyStatementSyntax node) 
            => null;

        /// <inheritdoc />
        public SyntaxTree Refactor(SyntaxNode nodeToRefactor)
            => nodeToRefactor.SyntaxTree.GetRoot()
                             .ReplaceNode(nodeToRefactor, 
                                          VisitEmptyStatement((EmptyStatementSyntax)nodeToRefactor))
                             .SyntaxTree;
    }
}