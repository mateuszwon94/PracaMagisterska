using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

namespace PracaMagisterska.WPF.Utils.Rewriters {
    public class AddConstModifier : CSharpSyntaxRewriter, IRefactor {
        public AddConstModifier(SemanticModel semanticModel) 
            => semanticModel_ = semanticModel;

        /// <summary>
        /// Visit LocalDeclaration Statement and add const to it
        /// </summary>
        /// <param name="node">Visited statement</param>
        /// <returns>null, becouse statement should be removed</returns>
        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) {
            if ( node.Declaration.Type.IsVar ) {
                TypeSyntax variableTypeName = node.Declaration.Type;

                // Special case: Ensure that 'var' isn't actually an alias to another type
                // (e.g. using var = System.String).
                if ( semanticModel_.GetAliasInfo(variableTypeName) == null ) {
                    // Retrieve the type inferred for var.
                    ITypeSymbol type = semanticModel_.GetTypeInfo(variableTypeName).ConvertedType;

                    // Special case: Ensure that 'var' isn't actually a type named 'var'.
                    if ( type.Name != "var" ) {
                        // Create a new TypeSyntax for the inferred type. Be careful
                        // to keep any leading and trailing trivia from the var keyword.
                        TypeSyntax typeName = SyntaxFactory.ParseTypeName(type.ToDisplayString())
                                                           .WithLeadingTrivia(variableTypeName.GetLeadingTrivia())
                                                           .WithTrailingTrivia(variableTypeName.GetTrailingTrivia());

                        // Add an annotation to simplify the type name.
                        TypeSyntax simplifiedTypeName = typeName.WithAdditionalAnnotations(Simplifier.Annotation);

                        // Replace the type in the variable declaration.
                        node = node.WithDeclaration(node.Declaration.WithType(simplifiedTypeName));
                    }
                }
            }

            SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();
            return node = node.WithLeadingTrivia(new SyntaxTrivia())
                              .AddModifiers(SyntaxFactory.Token(leadingTrivia,
                                                                SyntaxKind.ConstKeyword,
                                                                SyntaxTriviaList.Create(SyntaxFactory.Space)));


        }

        /// <inheritdoc />
        public SyntaxTree Refactor(SyntaxNode nodeToRefactor)
            => nodeToRefactor.SyntaxTree.GetRoot()
                             .ReplaceNode(nodeToRefactor,
                                          VisitLocalDeclarationStatement((LocalDeclarationStatementSyntax)nodeToRefactor))
                             .SyntaxTree;

        private readonly SemanticModel semanticModel_;
    }
}