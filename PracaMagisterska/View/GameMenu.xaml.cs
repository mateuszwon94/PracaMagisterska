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
using PracaMagisterska.WPF.Testers;
using PracaMagisterska.WPF.Utils;

namespace PracaMagisterska.WPF.View {
    /// <summary>
    /// Interaction logic for GameMenu.xaml
    /// </summary>
    public partial class GameMenu : Page {
        /// <inheritdoc />
        /// <summary>
        /// Constructor. Initialize all components
        /// </summary>
        public GameMenu() {
            InitializeComponent();
            LessonsListView.ItemsSource = Lesson.AllLessons;
        }

        /// <summary>
        /// Event function. Called when user select specyfic lesson.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void LessonsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            LessonsListView.SelectedItem = null;

            if (e.AddedItems.Count > 0)
                NavigationService?.Navigate(new SourceCode((Lesson)e.AddedItems[0]));
        }
    }
}