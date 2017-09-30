using System;
using System.Collections.Generic;
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

namespace PracaMagisterska.WPF.View {
    /// <summary>
    /// Interaction logic for SourceCode.xaml
    /// </summary>
    public partial class SourceCode : Page {
        public SourceCode(string title) {
            InitializeComponent();
            LessonTitle = title;
            TitleTextBox.Text = LessonTitle;
        }

        public string LessonTitle { get; private set; } = string.Empty;

        private void CompileButton_OnClick(object sender, RoutedEventArgs e) {
            SyntaxTree code = CSharpSyntaxTree.ParseText(SourceCodeTextBox.Text);

            using ( var ch = new ConsoleHelper() ) {
                try {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Compilation starter!");
                    Assembly program = CompilationHelper.Compile(code);

                    try {
                        Console.WriteLine("Execution starter!");
                        Console.ForegroundColor = ConsoleColor.White;
                        CompilationHelper.RunMain(program);
                    } catch ( Exception ex ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("There was execution errors!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                } catch ( CompilationException ex ) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("There was compilation errors!");
                    foreach ( Diagnostic failure in ex.Failures ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"\t{failure.Id}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" - {failure.ToString()}");
                    }
                }
            }
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e) {
            if ( NavigationService != null && NavigationService.CanGoBack )
                NavigationService?.GoBack();
        }
    }
}