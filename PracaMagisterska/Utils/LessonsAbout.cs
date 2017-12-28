using System.Collections.Generic;

namespace PracaMagisterska.WPF.Utils {
    public static class LessonsAbout {
        public static readonly Dictionary<int, string> LessonInfo = new Dictionary<int, string> {
            [0] = "W Funkcji static void Main() wywołaj funkcję WriteLine z klasy Console znajdującej się w przestrzeni nazw System.",
            [1] = ""
        };

        public static readonly Dictionary<int, string> LessonTitle = new Dictionary<int, string> {
            [0] = "Napisz swój pierwszy program!",
            [1] = "Zmienne są najważniejsze!"
        };
    }
}