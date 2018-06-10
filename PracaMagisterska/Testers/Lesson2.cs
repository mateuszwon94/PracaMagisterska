using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static PracaMagisterska.WPF.Utils.ConsoleHelper;

namespace PracaMagisterska.WPF.Testers {
    public sealed class Lesson2 : Lesson {
        /// <inheritdoc />
        public Lesson2() : base(2, "Pierwsze warunki", 0.5f)
            => InitializeTest();

        /// <inheritdoc />
        public Lesson2(bool[] results) : base(2, "Pierwsze warunki", 0.5f, results)
            => InitializeTest();

        /// <inheritdoc />
        protected override void InitializeTest() {
            HasStaticTest  = true;
            HasSimpleTest  = true;
            HasRealTest    = true;
            HasRandomTests = true;
        }

        /// <inheritdoc />
        protected override object Solution(params object[] parameters) {
            int x = (int)parameters[0],
                y = (int)parameters[1];

            if ( x < 0 || y < 0 )
                return x % y;
            else if ( x % 2 == 0 && y % 2 == 0 )
                return x + y;
            else if ( x % 2 == 1 && y % 2 == 0 )
                return x - y;
            else if ( x % 2 == 0 && y % 2 == 1 )
                return x / y;
            else // if ( x % 2 == 1 && y % 2 == 1 )
                return x * y;
        }

        /// <inheritdoc />
        public override bool StaticTest(MethodDeclarationSyntax testMethod) {
            if ( !testMethod.DescendantNodes().Any(node => node.IsKind(SyntaxKind.IfStatement)) ) {
                WriteLineColor("TestMethod do not contains any if statement", ConsoleColor.Red);

                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override Type[] ParametersType { get; } = new[] {
            typeof(int),
            typeof(int)
        };

        /// <inheritdoc />
        protected override object[] GenerateParamaters()
            => new object[] {
                random_.Next(-1000, 1000),
                random_.Next(-1000, 1000)
            };

        /// <inheritdoc />
        protected override List<object[]> RealTestCases { get; } = new List<object[]> {
            new object[] {2, 4},
            new object[] {3, 4},
            new object[] {4, 3},
            new object[] {5, 5},
            new object[] {-2, -4},
        };

        /// <inheritdoc />
        protected override List<object[]> SimpleTestCases { get; } = new List<object[]> {
            new object[] {20, 40},
            new object[] {323, 46},
            new object[] {46, 34},
            new object[] {53, 57},
            new object[] {-20, -84},
            new object[] {-35, -64},
            new object[] {-64, -35},
            new object[] {-75, -57},
        };

        /// <inheritdoc />
        public override string Info { get; } =
@"Warunki to jedna z najczęściej spotykanych konstrukcji. Jak sama nazwa wskazuje sprawdzają one warunek i wykonują, bądź nie wykonują, odpowiednią część kodu.
Zapis wygląda następująco:
if (warunek) instrukcja;
lub:
if (warunek) instrukcja1;
else instrukcja2;
lub:
if (warunek1) instrukcja1;
else if (warunek2) instrukcja2;
else instrukcja3;
lub:
if (warunek1) instrukcja1;
else if (warunek2) instrukcja2;
Warunek musi zawsze być typu logicznego (bool).
Twoja zadanie jest bardzo proste. Dostajesz dwie liczby całkowitoliczbowe (int). Zaimplementuj poniższe warunki:
Jeśli x jest parzyste oraz y jest parzyste, to zwróc x + y.
Jeśli x jest nieparzyste oraz y jest parzyste, to zwróć x - y.
Jeśli x jest nieparzyste oraz y jest nieparzyste, to zwróc x * y.
Jeśli x jest parzyste oraz y jest nieparzyste, to zwróć x / y.
Jeżeli x lub y są mniejsze niż 0, to zwróć x % y.";

        /// <inheritdoc />
        public override string DefaultCode { get; } = defaultProgramTemplate_.Replace("{ReturnType}", "int")
                                                                             .Replace("{Parameters}", "int x, int y")
                                                                             .Replace("{Body}", @"return if ( x == 0 ) return x + y;");
    }
}