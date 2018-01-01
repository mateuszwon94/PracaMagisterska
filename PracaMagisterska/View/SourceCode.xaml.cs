using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
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
        public SourceCode(string title, string info = null) {
            InitializeComponent();

            LessonTitle = title;
            TitleTextBox.Text = LessonTitle;

            DiagnosticListView.DataContext = lastDiagnostics_;
            DiagnosticListView.ItemsSource = lastDiagnostics_;

            if ( !string.IsNullOrEmpty(info) ) {
                LessonInfoTextBlock.Text = info;
            } else {
                LessonInfoTextBlock.Text = "Póki co brak opisu! :(";
            }
        }

        public string LessonTitle { get; private set; } = string.Empty;

        private async void CompileButton_OnClick(object sender, RoutedEventArgs e) {
            DiagnosticListView.DataContext = null;
            SyntaxTree code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);
            
            lastDiagnostics_.Clear();
            
            CompileingIndicator.IsActive = true;
            CompileButton.IsEnabled = false;
            (Assembly program, var diagnostics, bool isBuildSuccessful) = await code.CompileAneBuild();
            CompileingIndicator.IsActive = false;
            CompileButton.IsEnabled = true;

            WriteDiagnostic(diagnostics.Where(diag => diag.Severity == DiagnosticSeverity.Error),
                            DiagnosticHelper.Type.Error);
            WriteDiagnostic(diagnostics.Where(diag => diag.Severity == DiagnosticSeverity.Warning),
                            DiagnosticHelper.Type.Warning);
            WriteDiagnostic(diagnostics.Where(diag => diag.Severity == DiagnosticSeverity.Info),
                            DiagnosticHelper.Type.Info);

            if ( isBuildSuccessful ) {
                ConsoleHelper.Show(); {
                    try {
                        await program.RunMain();
                    } catch ( Exception ex ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There was execution errors!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"\t{ex}");
                    }

                    if ( !Settings.AutoCloseConsole ) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\n\tMain Window would not be responding until the Console would be active.");
                        Console.Write("\n\tPlease, press ENTER to hide console.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadLine();
                    }
                } ConsoleHelper.Hide();
            } else {
                await this.TryFindParent<MainWindow>()
                          .ShowMessageAsync("Kompilacja zakończona", "Kompilacja niepowiodła się");
            }
        }

        private void WriteDiagnostic(IEnumerable<Diagnostic> diagnostics,
                                     DiagnosticHelper.Type severity) {
            foreach ( Diagnostic diagnostic in diagnostics ) {
                string[] infos = diagnostic.ToString().Split(':');
                lastDiagnostics_.Add(new DiagnosticHelper {
                    Id = diagnostic.Id,
                    Severity = severity,
                    Location = infos.First().Trim().Trim('(', ')'),
                    Information = infos.Last().Trim()
                });
            }
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e) {
            if ( NavigationService != null && NavigationService.CanGoBack )
                NavigationService?.GoBack();
        }

        private readonly ObservableCollection<DiagnosticHelper> lastDiagnostics_ 
            = new ObservableCollection<DiagnosticHelper>();
    }
}