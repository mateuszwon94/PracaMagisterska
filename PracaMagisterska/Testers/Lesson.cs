using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
        /// <param name="resultStars">Number of result stars</param>
        protected Lesson(int no, string title, float dificulty, int resultStars = 3) : base(resultStars: resultStars) {
            No        = no;
            Title     = title;
            Dificulty = dificulty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="no">Lesson number</param>
        /// <param name="title">Lesson title</param>
        /// <param name="dificulty">Lesson dificulty</param>
        /// <param name="results">Current results</param>
        protected Lesson(int no, string title, float dificulty, bool[] results) : base(results: results) {
            No        = no;
            Title     = title;
            Dificulty = dificulty;
        }

        /// <inheritdoc cref="ITestable.EvaluateSolution" />
        public override async void EvaluateSolution(SyntaxTree userCode, double userTime, double referenceTime) {
            if ( CurrentResults.Length >= 2 && userTime <= referenceTime * 2 )
                CurrentResults[1] = true;

            if ( CurrentResults.Length >= 3 ) {
                SyntaxTree referenceTree;
                using ( StreamReader stream = File.OpenText($"./TestCodes/{nameof(Lesson)}{No}.cs") ) 
                    referenceTree = CSharpSyntaxTree.ParseText(await stream.ReadToEndAsync());

                if ( userCode.GetRoot().GetNumberOfStatements() <=
                     referenceTree.GetRoot().GetNumberOfStatements() * 1.5 )
                    CurrentResults[2] = true;
            }
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
        /// Initialize all present tests
        /// </summary>
        protected virtual void InitializeTest() { }

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
        public static readonly ObservableCollection<Lesson> AllLessons = new ObservableCollection<Lesson> { };

        /// <summary>
        /// Protected random number generator
        /// </summary>
        protected static readonly Random random_ = new Random();
    }
}