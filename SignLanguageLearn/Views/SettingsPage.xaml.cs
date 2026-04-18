using System;
using System.Windows;
using System.Windows.Controls;
using SignLanguageLearn.Services;

namespace SignLanguageLearn.Views
{
    public partial class SettingsPage : Page
    {
        private bool _isReady = false;

        public SettingsPage()
        {
            InitializeComponent();
            LoadToggles();
            _isReady = true;
        }

        private void LoadToggles()
        {
            if (MainWindow.AppData == null) return;
            _isReady = false;

            if (MainWindow.AppData.AppSettings.CurrentLanguage == "UA")
                RbUa.IsChecked = true;
            else
                RbEn.IsChecked = true;

            ThemeCheckBox.IsChecked = (MainWindow.AppData.AppSettings.CurrentTheme == "Dark");
            _isReady = true;
        }

        private void TriggerUpdate()
        {
            if (!_isReady || MainWindow.AppData == null) return;

            // 1. Зберігаємо дані
            DataManager.SaveData(MainWindow.AppData);

            // 2. Оновлюємо глобальні ресурси (мову та кольори)
            string langFile = MainWindow.AppData.AppSettings.CurrentLanguage == "UA" ? "LangUA.xaml" : "LangEN.xaml";
            App.StringUpdate(langFile);
            App.ColorUpdate(MainWindow.AppData.AppSettings.CurrentTheme == "Dark");

            // 3. ФІКС: Оновлюємо саме вікно та перенаправляємо фрейм назад на налаштування
            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                // Перевантажуємо сторінку налаштувань, щоб вона не ставала порожньою
                mainWin.MainFrame.Navigate(new SettingsPage());
            }
        }

        private void DarkTheme_Checked(object sender, RoutedEventArgs e) { if (MainWindow.AppData != null) MainWindow.AppData.AppSettings.CurrentTheme = "Dark"; TriggerUpdate(); }
        private void DarkTheme_Unchecked(object sender, RoutedEventArgs e) { if (MainWindow.AppData != null) MainWindow.AppData.AppSettings.CurrentTheme = "Light"; TriggerUpdate(); }
        private void RbUa_Checked(object sender, RoutedEventArgs e) { if (MainWindow.AppData != null) MainWindow.AppData.AppSettings.CurrentLanguage = "UA"; TriggerUpdate(); }
        private void RbEn_Checked(object sender, RoutedEventArgs e) { if (MainWindow.AppData != null) MainWindow.AppData.AppSettings.CurrentLanguage = "EN"; TriggerUpdate(); }
    }
}