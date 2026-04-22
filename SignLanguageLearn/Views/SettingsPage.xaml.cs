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

            // 1. Встановлюємо перемикачі мови жестів
            if (MainWindow.AppData.AppSettings.CurrentLanguage == "UA")
                RbUa.IsChecked = true;
            else
                RbEn.IsChecked = true;

            // 2. Встановлюємо перемикачі теми (замість ThemeCheckBox)
            if (MainWindow.AppData.AppSettings.CurrentTheme == "Dark")
                RbDark.IsChecked = true;
            else
                RbLight.IsChecked = true;

            _isReady = true;
        }

        private void TriggerUpdate()
        {
            if (!_isReady || MainWindow.AppData == null) return;

            // Зберігаємо зміни в наш data.json
            DataManager.SaveData(MainWindow.AppData);

            // Оновлюємо кольори теми
            try
            {
                App.ColorUpdate(MainWindow.AppData.AppSettings.CurrentTheme == "Dark");
            }
            catch { /* Обробка, якщо метод ще не реалізований */ }

            // Перезавантажуємо сторінку
            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                mainWin.MainFrame.Navigate(new SettingsPage());
            }
        }

        // --- ОБРОБНИКИ МОВИ ---
        private void RbUa_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AppData != null)
            {
                MainWindow.AppData.AppSettings.CurrentLanguage = "UA";
                TriggerUpdate();
            }
        }

        private void RbEn_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AppData != null)
            {
                MainWindow.AppData.AppSettings.CurrentLanguage = "EN";
                TriggerUpdate();
            }
        }

        // --- ОБРОБНИКИ ТЕМИ (Додано замість старих DarkTheme_Checked) ---
        private void RbLight_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AppData != null)
            {
                MainWindow.AppData.AppSettings.CurrentTheme = "Light";
                TriggerUpdate();
            }
        }

        private void RbDark_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AppData != null)
            {
                MainWindow.AppData.AppSettings.CurrentTheme = "Dark";
                TriggerUpdate();
            }
        }
    }
}