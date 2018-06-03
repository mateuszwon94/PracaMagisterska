using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PracaMagisterska.WPF.Utils {
    public class StatementCounter : CSharpSyntaxWalker {
        /// <summary>
        /// Calculate number od statements in descendend nodes.
        /// </summary>
        /// <param name="node">Root node</param>
        /// <returns>number of statements</returns>
        public int Calculate(SyntaxNode node) {
            if ( node != null ) 
                Visit(node);

            return counter_;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a BreakStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitBreakStatement(BreakStatementSyntax node) {
            base.VisitBreakStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a ContinueStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitContinueStatement(ContinueStatementSyntax node) {
            base.VisitContinueStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a CheckedStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitCheckedStatement(CheckedStatementSyntax node) {
            base.VisitCheckedStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a DoStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitDoStatement(DoStatementSyntax node) {
            base.VisitDoStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a EmptyStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitEmptyStatement(EmptyStatementSyntax node) {
            base.VisitEmptyStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a ExpressionStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitExpressionStatement(ExpressionStatementSyntax node) {
            base.VisitExpressionStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a AccessorDeclarationSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node) {
            if ( node.Body == null ) 
                counter_++;

            base.VisitAccessorDeclaration(node);
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a FixedStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitFixedStatement(FixedStatementSyntax node) {
            base.VisitFixedStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a ForEachStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitForEachStatement(ForEachStatementSyntax node) {
            base.VisitForEachStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a ForStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitForStatement(ForStatementSyntax node) {
            base.VisitForStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a GlobalStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitGlobalStatement(GlobalStatementSyntax node) {
            base.VisitGlobalStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a GotoStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitGotoStatement(GotoStatementSyntax node) {
            base.VisitGotoStatement(node);
            counter_++;
        }

        /// <summary>
        /// Called when the visitor visits a IfStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitIfStatement(IfStatementSyntax node) {
            base.VisitIfStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a InitializerExpressionSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitInitializerExpression(InitializerExpressionSyntax node) {
            base.VisitInitializerExpression(node);
            counter_ += node.Expressions.Count;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a LabeledStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitLabeledStatement(LabeledStatementSyntax node) {
            base.VisitLabeledStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a LocalDeclarationStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) {
            base.VisitLocalDeclarationStatement(node);
            if ( !node.Modifiers.Any(SyntaxKind.ConstKeyword) )
                counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a LockStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitLockStatement(LockStatementSyntax node) {
            base.VisitLockStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a ReturnStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitReturnStatement(ReturnStatementSyntax node) {
            base.VisitReturnStatement(node);
            if ( node.Expression != null )
                counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a SwitchStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitSwitchStatement(SwitchStatementSyntax node) {
            base.VisitSwitchStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a ThrowStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitThrowStatement(ThrowStatementSyntax node) {
            base.VisitThrowStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a UnsafeStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitUnsafeStatement(UnsafeStatementSyntax node) {
            base.VisitUnsafeStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a UsingDirectiveSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitUsingDirective(UsingDirectiveSyntax node) {
            base.VisitUsingDirective(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a UsingStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitUsingStatement(UsingStatementSyntax node) {
            base.VisitUsingStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a ConstructorDeclarationSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node) {
            base.VisitConstructorDeclaration(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a WhileStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitWhileStatement(WhileStatementSyntax node) {
            base.VisitWhileStatement(node);
            counter_++;
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the visitor visits a YieldStatementSyntax node.
        /// </summary>
        /// <param name="node">Visited node</param>
        public override void VisitYieldStatement(YieldStatementSyntax node) {
            base.VisitYieldStatement(node);
            counter_++;
        }

        /// <summary>
        /// Counter of statements 
        /// </summary>
        private int counter_;
    }
}