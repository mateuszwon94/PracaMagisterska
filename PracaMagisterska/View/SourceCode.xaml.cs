using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

            if ( !string.IsNullOrEmpty(info) ) {
                LessonInfoTextBlock.Text = info;
            } else {
                LessonInfoTextBlock.Text = "Póki co brak opisu! :(";
            }
        }

        public string LessonTitle { get; private set; } = string.Empty;

        private void CompileButton_OnClick(object sender, RoutedEventArgs e) {
            SyntaxTree code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);

            ConsoleHelper.Show(); {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Compilation starter!");
                (Assembly program, var diagnostics, bool isBuildSuccessful) = code.CompileAneBuild();

                WriteDiagnostic(diagnostics, DiagnosticSeverity.Error, ConsoleColor.Red,
                                "There was compilation errors");
                WriteDiagnostic(diagnostics, DiagnosticSeverity.Warning, ConsoleColor.Yellow);

                if ( isBuildSuccessful ) {
                    try {
                        Console.WriteLine("Execution starter!\n\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        program.RunMain();
                    } catch ( Exception ex ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There was execution errors!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"\t{ex}");
                    }
                }

                if ( !Settings.AutoCloseConsole ) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\n\tPlease, press ENTER to hide console.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadLine();
                }
            } ConsoleHelper.Hide();
        }

        private void WriteDiagnostic(ImmutableArray<Diagnostic> diagnostics,
                                     DiagnosticSeverity type,
                                     ConsoleColor color,
                                     string additionalMessage = null) {
            if ( diagnostics.Any(diagnostic => diagnostic.Severity == type) ) {
                if ( !string.IsNullOrEmpty(additionalMessage) ) {
                    Console.ForegroundColor = color;
                    Console.WriteLine(additionalMessage);
                }

                foreach ( Diagnostic failure in diagnostics.Where(fail => fail.Severity == type) ) {
                    Console.ForegroundColor = color;
                    Console.Write($"\t{failure.Id}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($" - {failure.ToString()}");
                }
            }
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e) {
            if ( NavigationService != null && NavigationService.CanGoBack )
                NavigationService?.GoBack();
        }
    }
}