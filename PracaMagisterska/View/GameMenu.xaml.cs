using System;
using System.Linq;
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
using PracaMagisterska.WPF.Utils;
using static PracaMagisterska.WPF.Utils.LessonsAbout;

namespace PracaMagisterska.WPF.View {
    /// <summary>
    /// Interaction logic for GameMenu.xaml
    /// </summary>
    public partial class GameMenu : Page {
        /// <inheritdoc />
        /// <summary>
        /// Constructor. Initialize all components
        /// </summary>
        public GameMenu() => InitializeComponent();

        /// <summary>
        /// Event function. Called when any of LessonButtons is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void LessonButton_OnClick(object sender, RoutedEventArgs e) {
            if ( sender is Button b ) {
                // get lesson number from button name
                int lessonNo = int.Parse(b.Name
                                          .Replace("Lesson", string.Empty)
                                          .Replace("Button", string.Empty));

                NavigationService?.Navigate(new SourceCode($"Lekcja {lessonNo}. {LessonTitle[lessonNo]}", LessonInfo[lessonNo]));
            }
        }
    }
}