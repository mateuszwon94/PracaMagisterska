using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.SharpDevelop.Editor;
using MahApps.Metro.Controls;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using PracaMagisterska.WPF.Exceptions;
using PracaMagisterska.WPF.Utils;
using PracaMagisterska.WPF.Utils.Completion;

namespace PracaMagisterska.WPF.View {
    /// <summary>
    /// Interaction logic for SourceCode.xaml
    /// </summary>
    public partial class SourceCode : Page {
        /// <summary>
        /// Constructor. Initialize all components and sets basic fields.
        /// </summary>
        /// <param name="title">Title of the page</param>
        /// <param name="lessonNo">Lesson number</param>
        /// <param name="info">Info about the lesson</param>
        public SourceCode(string title, int lessonNo, string info = null) {
            InitializeComponent();

            lessonNo_         = lessonNo;
            LessonTitle       = title;
            TitleTextBox.Text = LessonTitle;
                        
            DiagnosticListView.ItemsSource = lastDiagnostics_;

            LessonInfoTextBlock.Text = !string.IsNullOrEmpty(info) ? info : "Póki co brak opisu! :(";

            // Create marker service
            textMarkerService_ = new TextMarkerService(SourceCodeTextBox.Document);
            SourceCodeTextBox.TextArea.TextView.BackgroundRenderers.Add(textMarkerService_);
            SourceCodeTextBox.TextArea.TextView.LineTransformers.Add(textMarkerService_);

            // Add service to document
            ((IServiceContainer)SourceCodeTextBox.Document
                                                 .ServiceProvider
                                                 .GetService(typeof(IServiceContainer)))
                                                 ?.AddService(typeof(ITextMarkerService), textMarkerService_);

            SourceCodeTextBox.TextArea.TextEntered += SourceCodeTextBox_TextArea_TextEntered;
            
            SourceCodeTextBox.Focus();

            // Handle real-time diagnostic updation
            dispacherTimer_.Tick += (sender, args) => {
                if ( timer_.Elapsed >= TimeSpan.FromSeconds(1) ) {
                    UpdateDiagnostic();

                    timer_.Reset();
                }
            };

            dispacherTimer_.Interval = TimeSpan.FromSeconds(2);
            dispacherTimer_.Start();
        }

        /// <summary>
        /// Lesson title display ind header
        /// </summary>
        public string LessonTitle { get; private set; } = string.Empty;

        /// <summary>
        /// Event function. Called when Compile Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private async void CompileButton_OnClick(object sender, RoutedEventArgs e) {
            // Remove all markers from code
            textMarkerService_.RemoveAll(marker => true);

            // Get SyntaxTree from code
            SyntaxTree code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);
            
            // Compile and build code
            CompileingIndicator.IsActive = true;
            CompileButton.IsEnabled = false;
            (Assembly program, var diagnostics, bool isBuildSuccessful) = await code.Compile().Build();
            CompileingIndicator.IsActive = false;
            CompileButton.IsEnabled = true;

            // Write new diagnostic information
            UpdateDiagnostic(diagnostics);

            if ( isBuildSuccessful ) {
                using (ConsoleHelper ch = new ConsoleHelper()) {
                    try {
                        // Run Main method
                        await program.RunMain();
                    } catch ( Exception ex ) {
                        // Write to console any execution errors
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There was execution errors!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"\t{ex}");
                    }

                    if ( !Settings.AutoCloseConsole ) {
                        // Wait for user to press ENTER to hide a console
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\n\tMain Window would not be responding until the Console would be active.");
                        Console.Write("\n\tPlease, press ENTER to hide console.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadLine();
                    }
                }
                SourceCodeTextBox.Focus();
            } else {
                // Display PopUp information about failed compilation
                await this.TryFindParent<MainWindow>()
                          .ShowMessageAsync("Kompilacja zakończona", "Kompilacja niepowiodła się");
            }
        }

        /// <summary>
        /// Update diagnostic in DiagnosticListView
        /// </summary>
        /// <param name="diagnostics">Diagnostic to write</param>
        private void UpdateDiagnostic(IEnumerable<DiagnosticHelper> diagnostics) {
            // Clear all previous diagnostic info
            lastDiagnostics_.Clear();

            // Write new diagnostic information
            WriteDiagnostic(diagnostics.OrderBy(diag => diag.Priotiy));

            timer_.Reset();
        }

        /// <summary>
        /// Update diagnostic in DiagnosticListView
        /// </summary>
        private void UpdateDiagnostic() {
            // Clear all previous diagnostic info
            lastDiagnostics_.Clear();

            // Remove all markers from code
            textMarkerService_.RemoveAll(marker => true);

            if ( !string.IsNullOrEmpty(SourceCodeTextBox.Text.Trim()) ) {
                // Get SyntaxTree from code
                CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text).Compile(out var diagnostics);

                // Write new diagnostic information
                UpdateDiagnostic(diagnostics);
            }
        }

        /// <summary>
        /// Function writes diagnostic information to DiagnosticListView
        /// </summary>
        /// <param name="diagnostics">Diagnostic to be written</param>
        /// <param name="severity">Severity of diagnostic</param>
        private void WriteDiagnostic(IEnumerable<DiagnosticHelper> diagnostics) {
            foreach ( DiagnosticHelper diagnostic in diagnostics ) {
                lastDiagnostics_.Add(diagnostic);

                // Create and display markers in document
                if ( diagnostic.Location.Location == CodeLocation.LocationType.InSource ) {
                    DocumentLine line = SourceCodeTextBox.Document.GetLineByNumber(diagnostic.Location.Line);

                    ITextMarker marker = textMarkerService_.Create(line.Offset, line.Length);
                    marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                    if (diagnostic.Severity == DiagnosticHelper.SeverityType.Error)
                        marker.MarkerColor = Colors.Red;
                    else if ( diagnostic.Severity == DiagnosticHelper.SeverityType.Warning )
                        marker.MarkerColor = Colors.Yellow;
                    else if ( diagnostic.Severity == DiagnosticHelper.SeverityType.Info )
                        marker.MarkerColor = Colors.Blue;
                }
            }
        }

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
            if ( e.AddedItems.Count > 0 ) {
                DiagnosticHelper selectedItem = (DiagnosticHelper)e.AddedItems[0];

                if ( selectedItem.Location.Location == CodeLocation.LocationType.InSource ) {
                    DocumentLine line = SourceCodeTextBox.Document
                                                         .GetLineByNumber(selectedItem.Location.Line);

                    SourceCodeTextBox.Select(line.Offset, line.Length);
                }
            } else if ( e.RemovedItems.Count > 0 && DiagnosticListView.SelectedItem == null ) {
                SourceCodeTextBox.Select(0, 0);
            }
        }

        /// <summary>
        /// Displays completiion options (Intellisense).
        /// </summary>
        /// <param name="filterFunction">Function use to filetr displayed options</param>
        private async void InvokeCompletionWindow(Func<ISymbol, bool> filterFunction = null) {
            if ( filterFunction == null ) filterFunction = r => true;
            var recomendedSymbols = await CompilationHelper.GetRecmoendations(SourceCodeTextBox.Text,
                                                                              SourceCodeTextBox.CaretOffset);

            if ( recomendedSymbols.Any(filterFunction) ) {
                // Prepare completion window
                CompletionWindow completionWindow = new CompletionWindow(SourceCodeTextBox.TextArea) {
                    FontFamily                    = SourceCodeTextBox.FontFamily,
                };

                // Add completion options
                foreach ( ISymbol recomendation in recomendedSymbols.Where(filterFunction) )
                    completionWindow.CompletionList
                                    .CompletionData
                                    .Add(new SymbolCompletionData(recomendation));

                completionWindow.Show();
                completionWindow.Closed += (sender, args) => completionWindow = null;
            }
        }

        /// <summary>
        /// Event function. Called when user typed a text in SourceCodeTextBox.
        /// Show CompletionWindow if [.] have been typed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void SourceCodeTextBox_TextArea_TextEntered(object sender, TextCompositionEventArgs e) {
            if ( e.Text == "." ) {
                // Display completation window
                InvokeCompletionWindow();
            }
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
            var diagnostic = (DiagnosticHelper)DiagnosticListView.SelectedItem;
            var newCode    = diagnostic?.Refactor?.Refactor(diagnostic.SyntaxNode);

            if ( newCode != null ) {
                SourceCodeTextBox.Text = newCode.ToString();

                UpdateDiagnostic();
            }
        }

        /// <summary>
        /// Event funciont. Called when texe have been changed in SourceCodeTextBox.
        /// Starts timer for future updation of diagnostic
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void SourceCodeTextBox_OnTextChanged(object sender, EventArgs e)
            => timer_.Start();

        /// <summary>
        /// Lesson number
        /// </summary>
        private readonly int lessonNo_;

        /// <summary>
        /// Collectione used by DiagnosticListView to display diagnostic info
        /// </summary>
        private readonly ObservableCollection<DiagnosticHelper> lastDiagnostics_ = new ObservableCollection<DiagnosticHelper>();

        /// <summary>
        /// Text marker service use to display errors and warnings in code
        /// </summary>
        private readonly TextMarkerService textMarkerService_;

        /// <summary>
        /// Timer used to update diagnostic
        /// </summary>
        private static readonly Stopwatch timer_ = new Stopwatch();

        /// <summary>
        /// Timer used to update diagnostic
        /// </summary>
        private static readonly DispatcherTimer dispacherTimer_ = new DispatcherTimer();
    }
}