using System;
using System.Windows;
using System.Windows.Controls;
using SignLanguageLearn.Services;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Сторінка налаштувань додатка, що дозволяє користувачеві змінювати мову інтерфейсу, тему оформлення та рівень складності.
    /// </summary>
    public partial class SettingsPage : Page
    {
        /// <summary>
        /// Прапор, який вказує, чи завершено початкове завантаження елементів інтерфейсу, щоб запобігти передчасному спрацьовуванню подій.
        /// </summary>
        private bool _isReady = false;

        /// <summary>
        /// Ініціалізує новий екземпляр сторінки налаштувань та завантажує поточні параметри.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
            LoadToggles();
            _isReady = true;
        }

        /// <summary>
        /// Завантажує збережені налаштування конфігурації та встановлює відповідний стан перемикачів на формі.
        /// </summary>
        private void LoadToggles()
        {
            if (MainWindow.AppData == null) return;
            _isReady = false;

            if (MainWindow.AppData.AppSettings.CurrentLanguage == "UA") RbUa.IsChecked = true;
            else RbEn.IsChecked = true;

            if (MainWindow.AppData.AppSettings.CurrentTheme == "Dark") RbDark.IsChecked = true;
            else RbLight.IsChecked = true;

            string diff = MainWindow.AppData.AppSettings.Difficulty;
            if (diff == "Easy") RbEasy.IsChecked = true;
            else if (diff == "Hardcore") RbHardcore.IsChecked = true;
            else RbNormal.IsChecked = true;

            _isReady = true;
        }

        /// <summary>
        /// Обробляє зміну будь-якого параметра налаштувань, оновлює глобальну конфігурацію, зберігає її та перезапускає тему додотка.
        /// </summary>
        private void Difficulty_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isReady || MainWindow.AppData == null) return;

            MainWindow.AppData.AppSettings.CurrentLanguage = RbUa.IsChecked == true ? "UA" : "EN";
            MainWindow.AppData.AppSettings.CurrentTheme = RbDark.IsChecked == true ? "Dark" : "Light";

            if (RbEasy.IsChecked == true) MainWindow.AppData.AppSettings.Difficulty = "Easy";
            else if (RbHardcore.IsChecked == true) MainWindow.AppData.AppSettings.Difficulty = "Hardcore";
            else MainWindow.AppData.AppSettings.Difficulty = "Normal";

            DataManager.SaveData(MainWindow.AppData);

            App.ColorUpdate(MainWindow.AppData.AppSettings.CurrentTheme == "Dark");

            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                mainWin.MainFrame.Navigate(new SettingsPage());
            }
        }
    }
}