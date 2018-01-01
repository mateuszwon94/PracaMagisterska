using System.Collections.Generic;

namespace PracaMagisterska.WPF.Utils {
    /// <summary>
    /// Helper class containg information about lessons
    /// </summary>
    public static class LessonsAbout {
        /// <summary>
        /// Dictionary of Lessons information
        /// </summary>
        public static readonly Dictionary<int, string> LessonInfo = new Dictionary<int, string> {
            [0] = "W Funkcji static void Main() wywołaj funkcję WriteLine z klasy Console znajdującej się w przestrzeni nazw System.",
            [1] = ""
        };

        /// <summary>
        /// Dictionary of lessons title
        /// </summary>
        public static readonly Dictionary<int, string> LessonTitle = new Dictionary<int, string> {
            [0] = "Napisz swój pierwszy program!",
            [1] = "Zmienne są najważniejsze!"
        };
    }
}