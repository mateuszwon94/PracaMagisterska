using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PracaMagisterska.WPF.Testers {
    /// <summary>
    /// Abstract class representing lesson zero
    /// </summary>
    public class Lesson1 : Lesson {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Lesson1() : base(1, "Zmienne są najważniejsze", 0.25f)
            => HasStaticTest = true;

        /// <inheritdoc cref="Tester.Solution"/>
        protected override object Solution(params object[] parameters)
            => null;

        /// <inheritdoc cref="Tester.Solution"/>
        public override bool StaticTest(MethodDeclarationSyntax testMethod) {
            return true;
        }

        /// <inheritdoc cref="Lesson.DefaultCode"/>
        public override string DefaultCode { get; } = defaultProgramTemplate_.Replace("{ReturnType}", "void")
                                                                             .Replace("{Parameters}", string.Empty)
                                                                             .Replace("{Body}",
                                                                                      @"string imie = ""Wpisz swoje imie"";");

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

        /// <inheritdoc cref="Tester.ParametersType"/>
        protected override Type[] ParametersType { get; } = null;

        /// <inheritdoc cref="Lesson.RealTestCases"/>
        protected override List<object[]> RealTestCases { get; } = null;

        /// <inheritdoc cref="Lesson.SimpleTestCases"/>
        protected override List<object[]> SimpleTestCases { get; } = null;
    }
}