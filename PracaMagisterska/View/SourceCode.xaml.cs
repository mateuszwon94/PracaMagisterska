using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
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
using PracaMagisterska.WPF.Utils.Exceptions;
using static System.Console;
using static PracaMagisterska.WPF.Utils.ConsoleHelper;
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
            CurrentResultTextBlock.Text    = CurrentLesson.CurrentResultsStars;

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
            SourceCodeTextBox.TextArea.SelectionChanged += SourceCodeTextBox_TextArea_SelectionChanged;

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
                Code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);
                Code.Compile(out var diagnostics);

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
        /// Displays completiion options (Intellisense).
        /// </summary>
        /// <param name="filterFunction">Function use to filetr displayed options</param>
        private async void InvokeCompletionWindow(Func<ISymbol, bool> filterFunction = null) {
            if ( filterFunction == null ) filterFunction = r => true;
            var recomendedSymbols = await GetRecmoendations(SourceCodeTextBox.Text,
                                                            SourceCodeTextBox.CaretOffset);

            if ( recomendedSymbols.Any(filterFunction) ) {
                // Prepare completion window
                var completionWindow = new CompletionWindow(SourceCodeTextBox.TextArea) {
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
        /// Run program compiled from source sode editor.
        /// </summary>
        /// <param name="runAction">How program should be executed</param>
        /// <param name="outputKind">Output type of compilation</param>
        private async void RunProgram(Action<Assembly, SyntaxTree> runAction,
                                      OutputKind                   outputKind = OutputKind.DynamicallyLinkedLibrary) {
            // Remove all markers from code
            textMarkerService_.RemoveAll(marker => true);

            // Get SyntaxTree from code
            Code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);

            try {
                CompileingIndicator.IsActive = true;
                DisaableAllButtons();

                // Compile and build code
                var assembly = await Code.Compile(out var diagnostics, 
                                                  compilationOptions: new CSharpCompilationOptions(outputKind))
                                         .Build();

                CompileingIndicator.IsActive = false;
                EnbleAllButtons();

                // Write new diagnostic information
                UpdateDiagnostic(diagnostics);

                ShowConsole();

                try {
                    // Run Main method
                    runAction(assembly, Code);
                } catch ( Exception ex ) {
                    // Write to console any execution errors
                    WriteLineColor("There was execution errors!", ConsoleColor.Red);
                    WriteLine($"\t{ex}");
                }

                WriteLine("Execution ended.");

                if ( Settings.AutoCloseConsole )
                    HideConsole();
            } catch ( BuildFailedException ex ) {
                CompileingIndicator.IsActive = false;
                EnbleAllButtons();

                // Write new diagnostic information
                UpdateDiagnostic(ex.Diagnostics);

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
        /// Represent current code in SourceCodeTextBox
        /// </summary>
        public SyntaxTree Code;

        /// <summary>
        /// Object represent current lesson
        /// </summary>
        public Lesson CurrentLesson { get; }

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

        /// <summary>
        /// Node which will be use to renaming
        /// </summary>
        private SyntaxToken syntaxTokenToRename_;
    }
}