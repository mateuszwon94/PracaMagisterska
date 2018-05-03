using System.Windows;
using PracaMagisterska.WPF.Utils;
using static PracaMagisterska.WPF.Utils.Extension;

namespace PracaMagisterska.WPF {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        /// <inheritdoc />
        /// <summary>
        /// Static constructor.
        /// Hiding console at program startup.
        /// Gets recomendation list for empty string to speed up future recomendation. (First take some time)
        /// </summary>
        static App() {
            ConsoleHelper.Hide();
            GetRecmoendations("", 0);
        }
    }
}
