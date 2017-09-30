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
        static Settings() {
            AutoCloseConsole = false;
        }

        public Settings() => InitializeComponent();

        public static bool AutoCloseConsole { get; private set; }

        private void AutoCloseConsoleCheckBox_Checked(object sender, RoutedEventArgs e)
            => AutoCloseConsole = true;

        private void AutoCloseConsoleCheckBox_Unchecked(object sender, RoutedEventArgs e)
            => AutoCloseConsole = false;
    }
}