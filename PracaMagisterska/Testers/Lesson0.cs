namespace PracaMagisterska.WPF.Testers {
    /// <summary>
    /// Abstract class representing lesson zero
    /// </summary>
    public class Lesson0 : Lesson {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Lesson0() : base(0, "Witaj Świecie", 0f) { }
        
        /// <inheritdoc cref="Lesson.DefaultCode"/>
        public override string DefaultCode { get; } = defaultProgramWithMain_;

        /// <inheritdoc cref="Lesson.Info"/>
        public override string Info { get; } =
@"Oto twój pierwszy program. Przejdźmy linijka po linijce i wytłumacze Ci co się w nim dzieje.
W pierwszej linijce przy pomocy słowa kluczowego using importowana jest podstawowa biblioteka. W niej zanjdują się wszystkie najprostsze i najpotrzebniejsze typy typy.
Ponieważ C# jest czysto obiektowym językiem, to w linijce trzeciej zdefiniowana jest klasa (czy to dokładnie jest dowiesz się później). Ma ona w sobie metode Main. Tak nazywa się metodę, którą system operacyjny uruchamia jako pierwszą. W jej ciele z obiektu Console wywoływana jest metoda WriteLine, która wypisuje na ekran tekst z cudzysłowia.
Spróbuj skompilować i uruchomić ten program. Co zobaczyłeś w konsoli? Teraz spróbuj zmienić tekst i uruchomić program jeszcze raz.";
    }
}