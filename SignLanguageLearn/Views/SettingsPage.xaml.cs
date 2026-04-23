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

            if (MainWindow.AppData.AppSettings.CurrentTheme == "Dark")
                RbDark.IsChecked = true;
            else
                RbLight.IsChecked = true;

            _isReady = true;
        }

        private void TriggerUpdate()
        {
            if (!_isReady || MainWindow.AppData == null) return;

            DataManager.SaveData(MainWindow.AppData);

            try
            {
                App.ColorUpdate(MainWindow.AppData.AppSettings.CurrentTheme == "Dark");
            }
            catch { /* Обробка, якщо метод ще не реалізований */ }

            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                mainWin.MainFrame.Navigate(new SettingsPage());
            }
        }

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