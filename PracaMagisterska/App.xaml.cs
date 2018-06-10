using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PracaMagisterska.WPF.Testers;
using PracaMagisterska.WPF.Utils;
using static PracaMagisterska.WPF.Utils.Extension;

namespace PracaMagisterska.WPF {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        /// <summary>
        /// Event function. Called on program startup.
        /// Hiding console at program startup.
        /// Gets recomendation list for empty string to speed up future recomendation. (First take some time)
        /// Loads all current user progress.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void App_OnStartup(object sender, StartupEventArgs e) {
            ConsoleHelper.HideConsole();
            Task.Run(() => GetRecmoendations("", 0));

            try {
                // Load user progress
                var lessonsDict = new Dictionary<int, bool[]>();
                using ( var file = new BinaryReader(File.OpenRead("UserResults.res")) ) {
                    int count = file.ReadInt32();

                    for ( int i = 0; i < count; ++i ) {
                        int key = file.ReadInt32();
                        int internalCount = file.ReadInt32();

                        bool[] results = new bool[internalCount];
                        for ( int j = 0; j < internalCount; ++j ) 
                            results[j] = file.ReadBoolean();

                        lessonsDict[key] = results;
                    }
                }

                Lesson.AllLessons.Add(new Lesson0(lessonsDict[0]));
                Lesson.AllLessons.Add(new Lesson1(lessonsDict[1]));
                Lesson.AllLessons.Add(new Lesson2(lessonsDict[2]));
            } catch ( FileNotFoundException ) {
                // If there was no progress so far
                Lesson.AllLessons.Add(new Lesson0());
                Lesson.AllLessons.Add(new Lesson1());
                Lesson.AllLessons.Add(new Lesson2());
            }
        }

        /// <summary>
        /// Event function. Called on program exit.
        /// Saves all user current results.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void App_OnExit(object sender, ExitEventArgs e) {
            var lessonsDict = Lesson.AllLessons
                                    .ToDictionary(lesson => lesson.No, lesson => lesson.CurrentResults.ToList());

            using ( var file = new BinaryWriter(File.OpenWrite("UserResults.res")) ) {
                file.Write(lessonsDict.Keys.Count);
                foreach ( int key in lessonsDict.Keys ) {
                    file.Write(key);
                    file.Write(lessonsDict[key].Count);
                    foreach ( bool value in lessonsDict[key] )
                        file.Write(value);
                }
            }
        }
    }
}