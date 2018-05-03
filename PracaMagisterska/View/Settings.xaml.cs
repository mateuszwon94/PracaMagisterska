using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.CodeAnalysis.CSharp;
using PracaMagisterska.WPF.Utils.Completion;

namespace PracaMagisterska.WPF.View {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page {
        public static readonly HashSet<SyntaxKind> SyntaxKindsForMagicalNumberSearch = new HashSet<SyntaxKind>();

        /// <inheritdoc />
        /// <summary>
        /// Static constructor. Sets all settings to default.
        /// </summary>
        static Settings() {
            AutoCloseConsole = false;

            foreach ( MagicalNumbersFinderHelper magicalNumberFinder in magicalNumberFinders_ ) 
                SyntaxKindsForMagicalNumberSearch.Add(magicalNumberFinder.SyntaxKind);
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor. Initialize all components
        /// </summary>
        public Settings() {
            InitializeComponent();

            MagicalNumberFinderListView.ItemsSource = magicalNumberFinders_;
            MagicalNumberFinderListView.SelectAll();
        }

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

        /// <summary>
        /// Event function. Called when selection in MagicalNumberFinder ListView has changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void MagicalNumberFinderListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            foreach ( var item in e.AddedItems.Cast<MagicalNumbersFinderHelper>() ) 
                SyntaxKindsForMagicalNumberSearch.Add(item.SyntaxKind);

            foreach ( var item in e.RemovedItems.Cast<MagicalNumbersFinderHelper>() ) 
                SyntaxKindsForMagicalNumberSearch.Remove(item.SyntaxKind);
        }

        /// <summary>
        /// Item source for MagicalNumberFinderListView.
        /// Contains all posible expresion in which magical number cann apear
        /// </summary>
        private static readonly ObservableCollection<MagicalNumbersFinderHelper> magicalNumberFinders_ =
            new ObservableCollection<MagicalNumbersFinderHelper>(MagicalNumbersFinderHelper.All);
    }
}