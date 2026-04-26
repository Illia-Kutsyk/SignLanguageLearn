using System;
using System.Windows;
using System.Windows.Media;
using SignLanguageLearn.Services;
using SignLanguageLearn.Views;
using SignLanguageLearn.Models;

namespace SignLanguageLearn
{
    /// <summary>
    /// Головне вікно застосунку, що виконує роль контейнера для всіх сторінок.
    /// Забезпечує ініціалізацію глобальних даних, керування візуальними темами 
    /// та головну навігаційну панель.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Глобальне сховище даних застосунку, доступне з будь-якої сторінки.
        /// Містить налаштування та інформацію про користувача.
        /// </summary>
        public static RootData AppData { get; set; }

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="MainWindow"/>.
        /// Завантажує дані користувача, встановлює тему та відкриває стартову сторінку.
        /// </summary>
        public MainWindow()
        {
            // Завантаження профілю та налаштувань із файлу при запуску
            if (AppData == null) AppData = DataManager.LoadData();

            // Застосування збереженої теми (Dark/Light) до компонентів Windows
            if (AppData != null && AppData.AppSettings != null)
            {
                App.ColorUpdate(AppData.AppSettings.CurrentTheme == "Dark");
            }

            InitializeComponent();

            // Встановлення початкової сторінки у головний фрейм
            MainFrame.Navigate(new HomePage());
        }

        /// <summary>
        /// Оновлює візуальну тему застосунку на основі поточних налаштувань у AppData.
        /// </summary>
        public void UpdateVisuals()
        {
            if (AppData == null || AppData.AppSettings == null) return;
            App.ColorUpdate(AppData.AppSettings.CurrentTheme == "Dark");
        }

        /// <summary>
        /// Обробник для переходу на головну сторінку.
        /// </summary>
        private void BtnHome_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new HomePage());

        /// <summary>
        /// Обробник для переходу до профілю користувача.
        /// </summary>
        private void BtnProfile_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new ProfilePage());

        /// <summary>
        /// Обробник для переходу до розділу налаштувань.
        /// </summary>
        private void BtnSettings_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new SettingsPage());

        /// <summary>
        /// Обробник для переходу до сторінки інструкцій.
        /// </summary>
        private void BtnInstruction_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new InstructionPage());

        /// <summary>
        /// Обробник завершення роботи програми. 
        /// Зберігає всі поточні зміни у файл перед виходом.
        /// </summary>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (AppData != null) DataManager.SaveData(AppData);
            Application.Current.Shutdown();
        }
    }
}