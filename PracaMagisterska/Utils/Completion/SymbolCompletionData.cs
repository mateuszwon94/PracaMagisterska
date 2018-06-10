using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CodeAnalysis;

namespace PracaMagisterska.WPF.Utils.Completion {
    public class SymbolCompletionData : ICompletionData {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="recomendation">Recomendation symbol</param>
        public SymbolCompletionData(ISymbol recomendation)
            => Recomendation = recomendation;

        /// <inheritdoc/>
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs) {
            if ( IsFunction ) {
                textArea.Document.Replace(completionSegment, Text + "()");
                textArea.Caret.Offset--;
            } else 
                textArea.Document.Replace(completionSegment, Text);
        }

        /// <summary>
        /// Recomendation
        /// </summary>
        public ISymbol Recomendation { get; }

        /// <summary>
        /// True if this recomendation is a function
        /// </summary>
        public bool IsFunction => Recomendation.Kind == SymbolKind.Method;

        /// <summary>
        /// Image for recomendation. Always null
        /// </summary>
        public ImageSource Image { get; } = null;

        /// <summary>
        /// Text of recomendation
        /// </summary>
        public string Text => Recomendation.Name;

        /// <summary>
        /// Full description of recomendation
        /// </summary>
        public object Description => Content;

        /// <summary>
        /// Contex of recomendation
        /// </summary>
        public object Content => Recomendation.ToDisplayString();

        /// <summary>
        /// Priority of recomendation
        /// </summary>
        public double Priority { get; } = 1;
    }
}