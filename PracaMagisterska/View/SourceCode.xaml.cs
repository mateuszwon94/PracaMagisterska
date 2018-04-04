using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
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
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Editor;
using MahApps.Metro.Controls;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MahApps.Metro.Controls.Dialogs;
using PracaMagisterska.WPF.Exceptions;
using PracaMagisterska.WPF.Utils;
using static PracaMagisterska.WPF.Utils.CompilationHelper;

namespace PracaMagisterska.WPF.View {
    /// <summary>
    /// Interaction logic for SourceCode.xaml
    /// </summary>
    public partial class SourceCode : Page {
        /// <summary>
        /// Constructor. Initialize all components and sets basic fields.
        /// </summary>
        /// <param name="title">Title of the page</param>
        /// <param name="info">Info about the lesson</param>
        public SourceCode(string title, string info = null) {
            InitializeComponent();

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
            
            // Clear all previous diagnostic info
            lastDiagnostics_.Clear();
            
            // Compile and build code
            CompileingIndicator.IsActive = true;
            CompileButton.IsEnabled = false;
            (Assembly program, var diagnostics, bool isBuildSuccessful) = await code.CompileAneBuild();
            CompileingIndicator.IsActive = false;
            CompileButton.IsEnabled = true;

            // Write new diagnostic information
            WriteDiagnostic(diagnostics.Where(diag => diag.Severity == DiagnosticSeverity.Error));
            WriteDiagnostic(diagnostics.Where(diag => diag.Severity == DiagnosticSeverity.Warning));
            WriteDiagnostic(diagnostics.Where(diag => diag.Severity == DiagnosticSeverity.Info));

            if ( isBuildSuccessful ) {
                using (ConsoleHelper vh = new ConsoleHelper()) {
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
        /// Function writes diagnostic information to DiagnosticListView
        /// </summary>
        /// <param name="diagnostics">Diagnostic to be written</param>
        /// <param name="severity">Severity of diagnostic</param>
        private void WriteDiagnostic(IEnumerable<Diagnostic> diagnostics) {
            foreach ( Diagnostic diagnostic in diagnostics ) {
                lastDiagnostics_.Add(DiagnosticHelper.Create(diagnostic));

                // Create and display markers in document
                if ( diagnostic.Location.IsInSource ) {
                    DocumentLine line = SourceCodeTextBox.Document
                                                         .GetLineByNumber(diagnostic.Location
                                                                                    .GetLineSpan()
                                                                                    .StartLinePosition
                                                                                    .Line +1);

                    ITextMarker marker = textMarkerService_.Create(line.Offset, line.Length);
                    marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                    if (diagnostic.Severity == DiagnosticSeverity.Error)
                        marker.MarkerColor = Colors.Red;
                    else if ( diagnostic.Severity == DiagnosticSeverity.Warning )
                        marker.MarkerColor = Colors.Yellow;
                    else if ( diagnostic.Severity == DiagnosticSeverity.Info )
                        marker.MarkerColor = Colors.Blue;
                }
            }
        }

        /// <summary>
        /// Event function. Called when BackButton is clicked
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
            }
        }

        /// <summary>
        /// Collectione used by DiagnosticListView to display diagnostic info
        /// </summary>
        private readonly ObservableCollection<DiagnosticHelper> lastDiagnostics_ = new ObservableCollection<DiagnosticHelper>();

        /// <summary>
        /// Text marker service use to display errors and warnings in code
        /// </summary>
        private readonly TextMarkerService textMarkerService_;

    }
}