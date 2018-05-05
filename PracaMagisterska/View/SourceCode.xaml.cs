using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.SharpDevelop.Editor;
using MahApps.Metro.Controls;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MahApps.Metro.Controls.Dialogs;
using PracaMagisterska.WPF.Testers;
using PracaMagisterska.WPF.Utils;
using PracaMagisterska.WPF.Utils.Completion;
using static PracaMagisterska.WPF.Utils.Extension;

namespace PracaMagisterska.WPF.View {
    /// <summary>
    /// Interaction logic for SourceCode.xaml
    /// </summary>
    public partial class SourceCode : Page {
        /// <summary>
        /// Constructor. Initialize all components and sets basic fields.
        /// </summary>
        /// <param name="currentLesson">Specyfic <see cref="Lesson"/> object for current lesson</param>
        public SourceCode(Lesson currentLesson) {
            InitializeComponent();

            // Initialize with defaults
            CurrentLesson                  = currentLesson;
            TitleTextBox.Text              = CurrentLesson.Title;
            LessonInfoTextBlock.Text       = CurrentLesson.Info;
            SourceCodeTextBox.Text         = CurrentLesson.DefaultCode;
            DiagnosticListView.ItemsSource = lastDiagnostics_;

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

            UpdateDiagnostic();
            EnbleAllButtons();
        }

        /// <summary>
        /// Object represent current lesson
        /// </summary>
        public Lesson CurrentLesson { get; }

        /// <summary>
        /// Event function. Called when Compile Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void RunButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, _) => assembly.RunMain(), OutputKind.ConsoleApplication);

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
                    if ( diagnostic.Severity == DiagnosticHelper.SeverityType.Error )
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
            var recomendedSymbols = await GetRecmoendations(SourceCodeTextBox.Text,
                                                            SourceCodeTextBox.CaretOffset);

            if ( recomendedSymbols.Any(filterFunction) ) {
                // Prepare completion window
                CompletionWindow completionWindow = new CompletionWindow(SourceCodeTextBox.TextArea) {
                    FontFamily = SourceCodeTextBox.FontFamily,
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
            var diagnostic = (DiagnosticHelper)DiagnosticListView.SelectedItem;
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
        private void SourceCodeTextBox_OnTextChanged(object sender, EventArgs e)
            => timer_.Start();

        /// <summary>
        /// Event funciont. Called when ShowHideConsole Button have been preesed.
        /// Show/hide console window.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void ShowHideConsoleButton_OnClick(object sender, RoutedEventArgs e) {
            if ( ConsoleHelper.IsVisible )
                ConsoleHelper.Hide();
            else if ( !ConsoleHelper.IsVisible )
                ConsoleHelper.Show(clear: false);
        }

        /// <summary>
        /// Event function. Called when AllTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void AllTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, tree) => {
                var result = CurrentLesson.RunAllTests(tree.GetRoot(), assembly, out double elapsedTime);

                if ( result ) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("All test were succeful.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Collecting information about provided solution.");
                    Console.WriteLine($"Execution time: {elapsedTime} ms.");
                    int linesOfCode = tree.GetRoot()
                                          .GetLinesOfCodeByMethod()
                                          .Aggregate(0, (currentResult, pair) => currentResult + pair.Value);
                    Console.WriteLine($"Line of code in methods: {linesOfCode}");
                    int statementCount = tree.GetRoot()
                                             .GetNumberOfStatementsByMethod()
                                             .Aggregate(0, (currentResult, pair) => currentResult + pair.Value);
                    Console.WriteLine($"Number of statements in methods: {statementCount}");
                }
            });

        /// <summary>
        /// Event function. Called when RandomTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void RandomTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, _) => CurrentLesson.RunRandomTests(assembly, out var _));

        /// <summary>
        /// Event function. Called when RealTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void RealTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, _) => CurrentLesson.RunRealTests(assembly, out var _));


        /// <summary>
        /// Event function. Called when SimpleTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void SimpleTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((assembly, _) => CurrentLesson.RunSampleTests(assembly, out var _));

        /// <summary>
        /// Event function. Called when StaticTests Button is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void StaticTestsButton_OnClick(object sender, RoutedEventArgs e)
            => RunProgram((_, tree) => CurrentLesson.RunStaticTests(tree.GetRoot()));

        /// <summary>
        /// Run program compiled from source sode editor.
        /// </summary>
        /// <param name="runAction">How program should be executed</param>
        /// <param name="outputKind">Output type of compilation</param>
        private async void RunProgram(Action<Assembly, SyntaxTree> runAction,
                                      OutputKind                   outputKind = OutputKind.DynamicallyLinkedLibrary) {
            // Remove all markers from code
            textMarkerService_.RemoveAll(marker => true);

            // Get SyntaxTree from code
            SyntaxTree code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);

            CompileingIndicator.IsActive = true;
            DisaableAllButtons();

            // Compile and build code
            var (assembly, diagnostics, isBuildSuccesful) = await code.Compile(compilationOptions: new CSharpCompilationOptions(outputKind))
                                                                      .Build();

            CompileingIndicator.IsActive = false;
            EnbleAllButtons();

            // Write new diagnostic information
            UpdateDiagnostic(diagnostics);

            if ( isBuildSuccesful ) {
                ConsoleHelper.Show();

                try {
                    // Run Main method
                    runAction(assembly, code);
                } catch ( Exception ex ) {
                    // Write to console any execution errors
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("There was execution errors!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\t{ex}");
                }

                Console.WriteLine("Execution ended.");

                if ( Settings.AutoCloseConsole )
                    ConsoleHelper.Hide();
            } else {
                // Display PopUp information about failed compilation
                await this.TryFindParent<MainWindow>()
                          .ShowMessageAsync("Kompilacja zakończona", "Kompilacja niepowiodła się");
            }
        }

        /// <summary>
        /// Disables all buttons related to running or testing.
        /// </summary>
        private void DisaableAllButtons() {
            RunButton.IsEnabled         = false;
            StaticTestsButton.IsEnabled = false;
            SimpleTestsButton.IsEnabled = false;
            RealTestsButton.IsEnabled   = false;
            RandomTestsButton.IsEnabled = false;
            AllTestsButton.IsEnabled    = false;
        }

        /// <summary>
        /// Enables all buttons related to running or testing.
        /// If specyfic tests type do not exist for current lesson related button won't be enabled.
        /// </summary>
        private void EnbleAllButtons() {
            RunButton.IsEnabled         = true;
            StaticTestsButton.IsEnabled = CurrentLesson.HasStaticTest;
            SimpleTestsButton.IsEnabled = CurrentLesson.HasSimpleTest;
            RealTestsButton.IsEnabled   = CurrentLesson.HasRealTest;
            RandomTestsButton.IsEnabled = CurrentLesson.HasRandomTests;
            AllTestsButton.IsEnabled    = CurrentLesson.HasStaticTest ||
                                          CurrentLesson.HasSimpleTest ||
                                          CurrentLesson.HasRealTest ||
                                          CurrentLesson.HasRandomTests;
        }

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
        private readonly Stopwatch timer_ = new Stopwatch();

        /// <summary>
        /// Timer used to update diagnostic
        /// </summary>
        private readonly DispatcherTimer dispacherTimer_ = new DispatcherTimer();
    }
}