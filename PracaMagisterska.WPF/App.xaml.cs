using System.Windows;
using PracaMagisterska.WPF.Utils;

namespace PracaMagisterska.WPF {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        static App() => new ConsoleHelper(ConsoleHelper.ConsoleState.Hide);
    }
}
