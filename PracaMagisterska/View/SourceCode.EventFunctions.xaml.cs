using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using PracaMagisterska.WPF.Utils;
using PracaMagisterska.WPF.Utils.Exceptions;
using PracaMagisterska.WPF.Utils.Rewriters;
using static PracaMagisterska.WPF.Utils.Extension;

namespace PracaMagisterska.WPF.View {
    public partial class SourceCode {
        /// <summary>
        /// Event function. Called when Compile Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void RunButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, _) => assembly.RunMain(), OutputKind.ConsoleApplication);

        /// <summary>
        /// Event function. Called when BackButton is clicked
        /// Returns to select lesson page
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void BackButton_OnClick(object sender, RoutedEventArgs e) {
            if ( NavigationService != null && NavigationService.CanGoBack )
                NavigationService?.GoBack();
        }

        /// <summary>
        /// Event function. Called when selected in DiagnosticListView item has changed.
        /// Select line in source code depending on currently selected item. 
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void DiagnosticListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if ( e.AddedItems.Count > 0 &&
                 e.AddedItems[0] is DiagnosticHelper selectedItem &&
                 selectedItem.Location.Location == CodeLocation.LocationType.InSource ) {
                DocumentLine line = SourceCodeTextBox.Document
                                                     .GetLineByNumber(selectedItem.Location.Line);

                SourceCodeTextBox.Select(line.Offset, line.Length);

                FixButton.IsEnabled = selectedItem.Refactor != null;
            } else if ( e.RemovedItems.Count > 0 && DiagnosticListView.SelectedItem == null ) {
                SourceCodeTextBox.Select(0, 0);
                FixButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Event function. Called when user typed a text in SourceCodeTextBox.
        /// Show CompletionWindow if [.] have been typed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void SourceCodeTextBox_TextArea_TextEntered(object sender, TextCompositionEventArgs e) {
            // Display completation window
            if ( e.Text == "." ) InvokeCompletionWindow();
        }

        /// <summary>
        /// Event function. Called when user press a button when SourceCodeTextBox has focus.
        /// Show CompletionWindow if [CTRL] + [SPACE] have been preesed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void SourceCodeTextBox_OnKeyDown(object sender, KeyEventArgs e) {
            if ( e.Key == Key.Space && Keyboard.Modifiers == ModifierKeys.Control ) {
                // Display completation window
                InvokeCompletionWindow();

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event function. Called when user clicked Fix button. 
        /// Fix (if possible) selected diagnostic/
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void FixButton_OnClick(object sender, RoutedEventArgs e) {
            var diagnostic = DiagnosticListView.SelectedItem as DiagnosticHelper;
            var newCode    = diagnostic?.Refactor?.Refactor(diagnostic.SyntaxNode);

            if ( newCode != null ) {
                SourceCodeTextBox.Text = newCode.ToString();

                UpdateDiagnostic();
            }
        }

        /// <summary>
        /// Event funciont. Called when text have been changed in SourceCodeTextBox.
        /// Starts timer for future updation of diagnostic
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void SourceCodeTextBox_OnTextChanged(object sender, EventArgs e) {
            timer_.Start();

            Code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);
        }

        /// <summary>
        /// Event funciont. Called when ShowHideConsole Button have been preesed.
        /// Show/hide console window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void ShowHideConsoleButton_OnClick(object sender, RoutedEventArgs e) {
            if ( ConsoleHelper.IsVisible )
                ConsoleHelper.HideConsole();
            else if ( !ConsoleHelper.IsVisible )
                ConsoleHelper.ShowConsole(clear: false);
        }

        /// <summary>
        /// Event function. Called when AllTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void AllTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, tree) => {
                var result =
                    CurrentLesson.RunAllTests(tree, assembly, out double elapsedTime, out double referenceTime);

                if ( result ) {
                    ConsoleHelper.WriteLineColor("All test were succeful.", ConsoleColor.Green);
                    Console.WriteLine("Collecting information about provided solution.");
                    Console.WriteLine($"Execution time: {elapsedTime} ms (reference time: {referenceTime} ms)");
                    int statementCount = tree.GetRoot()
                                             .GetNumberOfStatements();
                    Console.WriteLine($"Number of statements in methods: {statementCount}");

                    CurrentLesson.EvaluateSolution(Code, elapsedTime, referenceTime);
                }

                CurrentResultTextBlock.Text = CurrentLesson.CurrentResultsStars;
            });

        /// <summary>
        /// Event function. Called when RandomTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void RandomTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, _) => CurrentLesson.RunRandomTests(assembly, out var _, out var _));

        /// <summary>
        /// Event function. Called when RealTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void RealTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, _) => CurrentLesson.RunRealTests(assembly, out var _, out var _));

        /// <summary>
        /// Event function. Called when SimpleTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void SimpleTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, _) => CurrentLesson.RunSampleTests(assembly, out var _, out var _));

        /// <summary>
        /// Event function. Called when StaticTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void StaticTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((_, tree) => CurrentLesson.RunStaticTests(tree));

        /// <summary>
        /// Event function. Called when selection in TextArea in SourceCodeTextBox has changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void SourceCodeTextBox_TextArea_SelectionChanged(object sender, EventArgs e) {
            if ( SourceCodeTextBox.SelectionLength == 0 ) // deselection
                syntaxTokenToRename_ = default;
            else {
                //selection
                // Get TextSpan of selected text
                var selectionSpan = new TextSpan(SourceCodeTextBox.SelectionStart,
                                                 SourceCodeTextBox.SelectionLength);

                Code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);

                // Get SyntaxToken which is selected
                syntaxTokenToRename_ = Code.GetRoot()
                                           .DescendantTokens()
                                           .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
                                           .FirstOrDefault(token => token.Span.Contains(selectionSpan));
            }

            if ( syntaxTokenToRename_ != default ) {
                // Enable renaming
                NewNameTextBox.Text      = syntaxTokenToRename_.ToString();
                NewNameTextBox.IsEnabled = true;
                RenameButton.IsEnabled   = true;
            } else {
                // Disable renaming
                NewNameTextBox.Text      = "Zaznacz identyfikator";
                NewNameTextBox.IsEnabled = false;
                RenameButton.IsEnabled   = false;
            }
        }

        /// <summary>
        /// Event function. Called when Rename Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private async void RenameButton_OnClick(object sender, RoutedEventArgs e) {
            try {
                // Get SyntaxTree with renamed symbol
                var newCode = Code.Compile(DefaultCompilationOptions)
                                  .GetSemanticModel(Code)
                                  .RenameSymbol(syntaxTokenToRename_, NewNameTextBox.Text.Trim());

                if ( newCode != null ) {
                    SourceCodeTextBox.Text = newCode.ToString();

                    UpdateDiagnostic();
                }
            } catch ( InvalidIdentifierException ) {
                // Display PopUp information about failed compilation
                await this.TryFindParent<MainWindow>()
                          .ShowMessageAsync("Zmiana nazwy nie powiodła się",
                                            "Podany identyfikator jest niepoprawny lub jest już używany");
            }
        }
    }
}