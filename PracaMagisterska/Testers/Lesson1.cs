using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static PracaMagisterska.WPF.Utils.ConsoleHelper;

namespace PracaMagisterska.WPF.Testers {
    /// <summary>
    /// Abstract class representing lesson zero
    /// </summary>
    public sealed class Lesson1 : Lesson {
        /// <inheritdoc />
        public Lesson1() : base(1, "Zmienne są najważniejsze", 0.25f, 1)
            => InitializeTest();

        /// <inheritdoc />
        public Lesson1(bool[] results ) : base(1, "Zmienne są najważniejsze", 0.25f, results) 
            => InitializeTest();

        /// <inheritdoc />
        protected override void InitializeTest() 
            => HasStaticTest = true;

        /// <inheritdoc cref="Tester.Solution"/>
        public override bool StaticTest(MethodDeclarationSyntax testMethod) {
            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.AddExpression) ||
                                                           node.IsKind(SyntaxKind.AddAssignmentExpression)) ) {
                WriteLineColor("TestMethod do not contains any add expresion", ConsoleColor.Red);

                return false;
            }

            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.SubtractExpression) ||
                                                           node.IsKind(SyntaxKind.SubtractAssignmentExpression)) ) {
                WriteLineColor("TestMethod do not contains any subtract expresion", ConsoleColor.Red);

                return false;
            }

            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.DivideExpression) ||
                                                           node.IsKind(SyntaxKind.DivideAssignmentExpression)) ) {
                WriteLineColor("TestMethod do not contains any divide expresion", ConsoleColor.Red);

                return false;
            }

            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.MultiplyExpression) ||
                                                           node.IsKind(SyntaxKind.MultiplyAssignmentExpression)) ) {
                WriteLineColor("TestMethod do not contains any multiply expresion", ConsoleColor.Red);

                return false;
            }

            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.ModuloExpression) ||
                                                           node.IsKind(SyntaxKind.ModuloAssignmentExpression)) ) {
                WriteLineColor("TestMethod do not contains any modulo expresion", ConsoleColor.Red);

                return false;
            }

            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.OrAssignmentExpression) ||
                                                           node.IsKind(SyntaxKind.BitwiseOrExpression)) ) {
                WriteLineColor("TestMethod do not contains any OR expresion", ConsoleColor.Red);

                return false;
            }

            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.AndAssignmentExpression) ||
                                                           node.IsKind(SyntaxKind.BitwiseAndExpression)) ) {
                WriteLineColor("TestMethod do not contains any AND expresion", ConsoleColor.Red);

                return false;
            }

            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.ExclusiveOrExpression) ||
                                                           node.IsKind(SyntaxKind.ExclusiveOrAssignmentExpression)) ) {
                WriteLineColor("TestMethod do not contains any AND expresion", ConsoleColor.Red);

                return false;
            }

            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.LeftShiftAssignmentExpression) ||
                                                           node.IsKind(SyntaxKind.LeftShiftExpression) ||
                                                           node.IsKind(SyntaxKind.RightShiftAssignmentExpression) ||
                                                           node.IsKind(SyntaxKind.RightShiftAssignmentExpression)) ) {
                WriteLineColor("TestMethod do not contains any shift expresion", ConsoleColor.Red);

                return false;
            }

            return true;
        }

        /// <inheritdoc cref="Lesson.DefaultCode"/>
        public override string DefaultCode { get; } = defaultProgramTemplate_.Replace("{ReturnType}", "void")
                                                                             .Replace("{Parameters}", string.Empty)
                                                                             .Replace("{Body}", @"string imie = ""Wpisz swoje imie"";");

        /// <inheritdoc cref="Lesson.Info"/>
        public override string Info { get; } =
@"Zmienne lokalne używane są do przechowywania wartości w obręie danej funkcji. Przed użyciem należy je najpierw zadeklarować, np:
int wiek;  // Deklaracja
string name = ""Mateusz"";  // Deklaracja z definicją
wiek = 24; // Definicja
obecnyRok = 2018;
rokUrodzenia = obecnyRok - wiek;

Dostępne są takie typy proste jak:
- Typy całkowitoliczbowe:
    > int, uint, long, ulong, short, ushort, byte, sbyte
- Typy zmiennoprzecinkowe:
    > float, double, decimal
- Typ znakowy i łańcuchowy:
    > char, string
- Typ logiczny:
    > bool
Na typach liczbowych można wykonywać takie operacje jak:
- Operacje algebraiczne:
    > dodawanie (+)
    > odejmowanie (-)
    > mnożenie (*)
    > dzielenie (/)
    > dzielenie modulo (%)
- Operacje bitowe:
    > i, AND (&)
    > lub, OR (|)
    > albo, XOR (^)
    > przesunięcie w prawo (>>)
    > przesunięcie w lewo (<<)
Wszystkie typy liczbowe można ze sobą porównywać:
    > równe (==)
    > różne (!=)
    > mniejsze niż (<)
    > większe niż (>)
    > nie mniejsze niż/większe bądź równe (>=)
    > nie większe niż/mniejsze bądź równe (<=)
Warto pamiętać w jakiej kolejności są przeprowadzane operacje. 
Pamiętaj, że zapis: i = i + 1, można uprościć do i += 1.
Zadeklaruj zmienne różnych z podanych typów. Sprawdź jakie są ich zakresy wartości. Wypróbój wszystkie wspomniane operacje algebraiczne i bitowe.";
    }
}