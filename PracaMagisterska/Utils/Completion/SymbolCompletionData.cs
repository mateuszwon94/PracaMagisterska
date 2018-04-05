using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CodeAnalysis;

namespace PracaMagisterska.WPF.Utils.Completion {
    public class SymbolCompletionData : ICompletionData {
        public SymbolCompletionData(ISymbol recomendation) 
            => Recomendation = recomendation;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs) {
            if ( IsFunction ) {
                textArea.Document.Replace(completionSegment, Text + "()");
                textArea.Caret.Offset--;
            } else {
                textArea.Document.Replace(completionSegment, Text);
            }
        }

        public ISymbol Recomendation { get; }

        public bool IsFunction => Recomendation.Kind == SymbolKind.Method;

        public ImageSource Image { get; } = null;

        public string Text => Recomendation.Name;

        public object Description => Recomendation.ToDisplayString();

        public object Content => Description;

        public double Priority { get; } = 1;
    }
}