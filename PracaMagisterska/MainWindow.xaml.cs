using MahApps.Metro.Controls;

namespace PracaMagisterska.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        /// <inheritdoc />
        /// <summary>
        /// Constructor. Initialize all components
        /// </summary>
        public MainWindow() => InitializeComponent();

        /// <summary>
        /// Event function. Called when HamburgerMenuControl is clicked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e) {
            HamburgerMenuControl.Content    = e.ClickedItem;
            HamburgerMenuControl.IsPaneOpen = false;
        }
    }
}