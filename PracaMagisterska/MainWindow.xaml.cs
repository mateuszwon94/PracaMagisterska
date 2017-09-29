using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using PracaMagisterska.WPF.Utils;

namespace PracaMagisterska.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        public MainWindow() { InitializeComponent(); }

        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e) {
            HamburgerMenuControl.Content = e.ClickedItem;
            HamburgerMenuControl.IsPaneOpen = false;
        }
    }
}