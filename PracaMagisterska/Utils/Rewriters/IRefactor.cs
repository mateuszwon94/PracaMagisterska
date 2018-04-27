using Microsoft.CodeAnalysis;

namespace PracaMagisterska.WPF.Utils.Rewriters {
    /// <summary>
    /// Interface used by refactoring classes.
    /// </summary>
    public interface IRefactor {
        /// <summary>
        /// Method, which is performing refactorin on given node.
        /// </summary>
        /// <param name="nodeToRefactor">Node to recator</param>
        /// <returns>SyntaxTree with refactored node.</returns>
        SyntaxTree Refactor(SyntaxNode nodeToRefactor);
    }
}