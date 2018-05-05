using System.Windows.Controls;
using PracaMagisterska.WPF.Testers;

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

            DescriptionTextBlock.Text = @"Cześć!
            
Po lewej stronie znajdują się poszczególne lekcje. Możesz wybrać dowolną, ale zdecydowanie zalecam wybieranie ich i rozwiązywanie pokolei. Zapewne dość często będzie sie zdażać tak, że jedna z lekcji będzie wymagała wiedzy z poprzednich. Tak to już jest z programowaniem, że bez podstaw nie da sie zrobić rzeczy bardziej zaawansowanych.
Jeżeli dla lekcji istnieją jakieś testy możesz je uruchomić odpowiednim przyciskiem. Jeśli wszystkie testy zakończą się pozytywnie Twoje rozwiązanie zostanie ocenione zarówno pod kontem ilości linii kodu jak i czasu wykonania.
Rodzaje testów z jakimi się spotkasz:
- Testy statyczne - sprawdzają czy użyłeś właściwych konstrukcji (np. czy w lekcji z warunkami użyłeś konstrukci z if-em).
- Proste testy - kilka prostych testów pokazujących jak Twoje rozwiązanie ma się zachowywać w konkretnych sytuacjach.
- Testy rzeczywiste - sporo testów, które zawieraja w sobie np. testowanie wartosci granicznych. Mają one dać pełen obraz tego, czy Twoje rozwiązanie jest poprawne.
- Testy losowe - mają poprostu za każdym razem losowo wygenerowane parametry (oczywiście takie, które nie znajdują się w testach prostych czy rzeczywistych). Mają na celu głównie zabezpieczenie się przed programami napisanymi tak, by przechodzić testy, a nie tak, żeby być kompleksowymi i poprawnymi rozwiązaniami.
Pamiętaj, że zawsze masz do dyspozycji przycisk ""Uruchom"", który będzie uruchamiał metodę: public static void Main(string args). Możesz w niej sprawdzić działanie swojej funkcji przed puszczeniem właściwych testów (o ile takie istnieją dla danej lekcji).";
        }

        /// <summary>
        /// Event function. Called when user select specyfic lesson.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments</param>
        private void LessonsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            LessonsListView.SelectedItem = null;

            if ( e.AddedItems.Count > 0 )
                NavigationService?.Navigate(new SourceCode((Lesson)e.AddedItems[0]));
        }
    }
}