using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Логіка взаємодії для головної сторінки (HomePage.xaml).
    /// Слугує основним навігаційним вузлом застосунку, що забезпечує швидкий перехід 
    /// до ключових розділів: уроків, словника, досягнень та тестування.
    /// </summary>
    public partial class HomePage : Page
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="HomePage"/>.
        /// </summary>
        public HomePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обробник події натискання на кнопку "Уроки". 
        /// Переспрямовує користувача на сторінку з навчальними матеріалами.
        /// </summary>
        private void BtnLessons_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new LessonsPage());

        /// <summary>
        /// Обробник події натискання на кнопку "Словник". 
        /// Відкриває перелік доступних жестів з описом.
        /// </summary>
        private void BtnDictionary_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new DictionaryPage());

        /// <summary>
        /// Обробник події натискання на кнопку "Досягнення". 
        /// Відображає прогрес навчання та отримані нагороди користувача.
        /// </summary>
        private void BtnAchievements_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new AchievementsPage());

        /// <summary>
        /// Обробник події натискання на кнопку "Тести". 
        /// Переводить користувача до модуля перевірки знань.
        /// </summary>
        private void BtnTests_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new TestPage());
    }
}