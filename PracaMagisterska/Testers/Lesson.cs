using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PracaMagisterska.WPF.View;

namespace PracaMagisterska.WPF.Testers {
    /// <summary>
    /// Abstract class representing lesson
    /// </summary>
    public abstract class Lesson : Tester {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="no">Lesson number</param>
        /// <param name="title">Lesson title</param>
        /// <param name="dificulty">Lesson dificulty</param>
        protected Lesson(int no, string title, float dificulty) {
            No        = no;
            Title     = title;
            Dificulty = dificulty;
        }

        /// <summary>
        /// Lesson Number
        /// </summary>
        public int No { get; }

        /// <summary>
        /// Lesson Number with dot at the end (displayed in <see cref="GameMenu"/>)
        /// </summary>
        public string NoString => No + ".";

        /// <summary>
        /// Info for user what have to be done in this lesson
        /// </summary>
        public abstract string Info { get; }

        /// <summary>
        /// Lesson title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Default code displayed in source code editor
        /// </summary>
        public abstract string DefaultCode { get; }

        /// <summary>
        /// Lesson dificulty
        /// </summary>
        public float Dificulty { get; }

        /// <summary>
        /// Dificulty displayed as five stars
        /// </summary>
        public string Stars {
            get {
                string value     = string.Empty;
                float  dificulty = Dificulty;

                for ( int stars = 0; stars < 5; ++stars ) {
                    // If dificulty was reduce to zero just fill with empty stars
                    if ( dificulty == 0f ) {
                        value += stars_[0f] + " ";
                        continue;
                    }

                    // Reduce dificulty and add stars
                    foreach ( float d in stars_.Keys.OrderByDescending(k => k) ) {
                        if ( dificulty - d >= 0 ) {
                            dificulty -= d;
                            value     += stars_[d] + " ";
                            break;
                        }
                    }
                }

                return value;
            }
        }

        /// <summary>
        /// Dictionary in which key is dificulty level and valu is properly filled star
        /// </summary>
        private static Dictionary<float, string> stars_ = new Dictionary<float, string>() {
            [0f]    = char.ConvertFromUtf32(0xE734),
            [0.25f] = char.ConvertFromUtf32(0xF0E5),
            [0.5f]  = char.ConvertFromUtf32(0xF0E7),
            [0.75f] = char.ConvertFromUtf32(0xF0E9),
            [1f]    = char.ConvertFromUtf32(0xE735),
        };

        /// <summary>
        /// Template of default program
        /// </summary>
        protected static string defaultProgramTemplate_ =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program {
    public static {ReturnType} TestMethod({Parameters}) {
        {Body}
    }
}";

        /// <summary>
        /// Template of default program with main function
        /// </summary>
        protected static string defaultProgramWithMain_ =
@"using System;

class Program {
    public static void Main(string[] args) {
        Console.WriteLine(""Hello World!"");
    }
}";

        /// <summary>
        /// Collection of all avaliable lessons
        /// </summary>
        public static ObservableCollection<Lesson> AllLessons = new ObservableCollection<Lesson> {
            new Lesson0(),
            new Lesson1(),
            new Lesson2(),
        };

        protected static readonly Random random_ = new Random();
    }
}