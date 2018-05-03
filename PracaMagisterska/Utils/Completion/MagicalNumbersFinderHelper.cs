using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.CodeAnalysis.CSharp;

namespace PracaMagisterska.WPF.Utils.Completion {
    public class MagicalNumbersFinderHelper {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="syntaxKind">SyntaxKind of expression</param>
        /// <param name="title">Title to be displayed in setting</param>
        /// <param name="example">Example of expresion</param>
        private MagicalNumbersFinderHelper(SyntaxKind syntaxKind, string title, string example) {
            SyntaxKind  = syntaxKind;
            Title       = title;
            Example     = example;
        }

        /// <summary>
        /// SyntaxKind of expression
        /// </summary>
        public SyntaxKind SyntaxKind  { get; }

        /// <summary>
        /// Title to be displayed in setting
        /// </summary>
        public string     Title       { get; }

        /// <summary>
        /// Example of expresion
        /// </summary>
        public string     Example     { get; }

        /// <summary>
        /// All expresion in which magical number can be found.
        /// </summary>
        public static IEnumerable<MagicalNumbersFinderHelper> All { get; } = new[] {
            new MagicalNumbersFinderHelper(SyntaxKind.AddExpression,                   "Dodawanie",                                      "a + 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.SubtractExpression,              "Odejmowanie",                                    "a - 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.MultiplyExpression,              "Mnożenie",                                       "a * 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.DivideExpression,                "Dzielenie",                                      "a / 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.ModuloExpression,                "Dzielenie modulo",                               "a % 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.LeftShiftExpression,             "Przesuniecie butowe w lewo",                     "a << 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.RightShiftExpression,            "Przesuniecie bitowe w prawo",                    "a >> 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.LogicalOrExpression,             "Logiczna alternatywa",                           "a || 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.LogicalAndExpression,            "Logiczna koniungcja",                            "a && 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.BitwiseOrExpression,             "Bitowa alternatywa",                             "a | 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.BitwiseAndExpression,            "Bitowa koniungcja",                              "a & 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.ExclusiveOrExpression,           "Bitowa alternatywa wykluczająca",                "a ^ 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.EqualsExpression,                "Równość",                                        "a == 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.NotEqualsExpression,             "Nierówność",                                     "a != 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.LessThanExpression,              "Mniejsze",                                       "a < 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.LessThanOrEqualExpression,       "Nie większe niż",                                "a <= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.GreaterThanExpression,           "Większe niż",                                    "a > 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.GreaterThanOrEqualExpression,    "Nie mniejsze niż",                               "a >= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.IsExpression,                    "Operator is",                                    "a is 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.CoalesceExpression,              "Koalescencja",                                   "a ?? 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.AddAssignmentExpression,         "Dodawanie z przypisaniem",                       "a += 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.SubtractAssignmentExpression,    "Odejmowanie z przypisaniem",                     "a -= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.MultiplyAssignmentExpression,    "Mnożenie z przypisaniem",                        "a *= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.DivideAssignmentExpression,      "Dzielenie z przypisaniem",                       "a /= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.ModuloAssignmentExpression,      "Dzielenie modulo z przypisaniem",                "a %= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.AndAssignmentExpression,         "Bitowa koniungcja z przypisaniem",               "a &= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.ExclusiveOrAssignmentExpression, "Bitowa alternatywa wykluczająca z przypisaniem", "a ^= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.OrAssignmentExpression,          "Bitowa alternatywa z przypisaniem",              "a |= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.LeftShiftAssignmentExpression,   "Przesunięcie bitowe w lewo z przypisaniem",      "a <<= 1"),
            new MagicalNumbersFinderHelper(SyntaxKind.RightShiftAssignmentExpression,  "Przesunięcie bitowe w prawo z przypisaniem",     "a >>= 1"),
        };
    }
}