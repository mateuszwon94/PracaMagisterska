using System;
using System.Collections.Generic;
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

namespace PracaMagisterska.WPF.View {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page {
        /// <inheritdoc />
        /// <summary>
        /// Static constructor. Sets all settings.
        /// </summary>
        static Settings() {
            AutoCloseConsole = false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor. Initialize all components
        /// </summary>
        public Settings() => InitializeComponent();

        /// <summary>
        /// If <value>true</value> then console window will automaticly close after execution of program, else pressing enter will be needed.
        /// </summary>
        public static bool AutoCloseConsole { get; private set; }

        /// <summary>
        /// Event function. Called when AutoHideConsole CheckBox is checked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void AutoCloseConsoleCheckBox_Checked(object sender, RoutedEventArgs e)
            => AutoCloseConsole = true;

        /// <summary>
        /// Event function. Called when AutoHideConsole CheckBox is unchecked
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void AutoCloseConsoleCheckBox_Unchecked(object sender, RoutedEventArgs e)
            => AutoCloseConsole = false;
    }
}