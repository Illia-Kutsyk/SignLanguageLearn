using System;
using System.Windows;
using System.Linq;
using System.Windows.Media;
using SignLanguageLearn.Services;

namespace SignLanguageLearn
{
    public partial class MainWindow : Window
    {
        public static RootData AppData { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // 1. Завантажуємо дані з JSON
            AppData = DataManager.LoadData();

            // 2. Застосовуємо тему та мову при старті
            UpdateVisuals();
        }

        // ЦЕЙ МЕТОД ТЕПЕР ПУБЛІЧНИЙ І МІСТИТЬ ВСЕ: І МОВУ, І ТЕМУ
        public void UpdateVisuals()
        {
            if (AppData == null) return;

            try
            {
                // --- 1. ОНОВЛЕННЯ МОВИ ---
                // Використовуємо твій LanguageManager або пряму заміну файлів
                string langFile = AppData.AppSettings.CurrentLanguage == "UA" ? "LangUA.xaml" : "LangEN.xaml";

                // Оскільки файли в корені (як на твоїх скрінах), шлях просто "/файл"
                var langUri = new Uri($"/{langFile}", UriKind.Relative);
                var newLangDict = new ResourceDictionary { Source = langUri };

                // Видаляємо стару мову і додаємо нову
                var oldLang = Application.Current.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Lang"));

                if (oldLang != null) Application.Current.Resources.MergedDictionaries.Remove(oldLang);
                Application.Current.Resources.MergedDictionaries.Add(newLangDict);

                // --- 2. ОНОВЛЕННЯ ТЕМИ (Твій робочий метод кольорів) ---
                SetTheme(AppData.AppSettings.CurrentTheme == "Dark");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка оновлення візуалу: " + ex.Message);
            }
        }

        // ТОЙ САМИЙ МЕТОД, ЯКИЙ ШУКАВ SETTINGSPAGE
        public void SetTheme(bool isDark)
        {
            if (isDark)
            {
                Application.Current.Resources["PrimaryBackground"] = new SolidColorBrush(Color.FromRgb(30, 27, 41));
                Application.Current.Resources["CardBackground"] = new SolidColorBrush(Color.FromRgb(45, 41, 59));
                Application.Current.Resources["MainText"] = new SolidColorBrush(Colors.White);
            }
            else
            {
                Application.Current.Resources["PrimaryBackground"] = new SolidColorBrush(Color.FromRgb(205, 233, 254));
                Application.Current.Resources["CardBackground"] = new SolidColorBrush(Colors.White);
                Application.Current.Resources["MainText"] = new SolidColorBrush(Color.FromRgb(45, 52, 54));
            }
        }

        // --- НАВІГАЦІЯ ---
        private void BtnHome_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new Uri("Views/HomePage.xaml", UriKind.Relative));
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new Uri("Views/ProfilePage.xaml", UriKind.Relative));
        private void BtnSettings_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new Uri("Views/SettingsPage.xaml", UriKind.Relative));

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            DataManager.SaveData(AppData);
            Application.Current.Shutdown();
        }
    }
}