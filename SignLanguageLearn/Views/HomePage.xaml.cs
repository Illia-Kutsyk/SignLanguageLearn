using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SignLanguageLearn.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void BtnLessons_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new LessonsPage());

        private void BtnDictionary_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new DictionaryPage());

        private void BtnAchievements_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new AchievementsPage());

        // ВИПРАВЛЕНО: Прибрали 's', тепер назва збігається з класом у TestPage.xaml.cs
        private void BtnTests_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new TestPage());
    }
}