using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SignLanguageLearn.Services;
using SignLanguageLearn.Views;
using SignLanguageLearn.Models;

namespace SignLanguageLearn
{
    /// <summary>
    /// Головне вікно застосунку, що виконує роль базового контейнера для всіх сторінок.
    /// Забезпечує ініціалізацію глобальних даних, керування візуальними темами, 
    /// навігацію та перемикання режимів відображення (наприклад, повноекранний тест).
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Глобальне сховище даних застосунку, доступне з будь-якого модуля.
        /// Містить налаштування інтерфейсу, інформацію про користувача та його прогрес.
        /// </summary>
        public static RootData AppData { get; set; }

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="MainWindow"/>.
        /// Завантажує дані конфігурації, застосовує обрану тему та відкриває головне меню.
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
        /// Статичний метод для швидкого збереження прогресу та досягнень користувача
        /// у локальну базу даних (JSON).
        /// </summary>
        public static void SaveAchievements()
        {
            if (AppData != null)
            {
                DataManager.SaveData(AppData);
            }
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
        /// Керує видимістю бічного меню навігації. 
        /// Використовується для розширення модуля тестування на весь екран, 
        /// приховуючи зайві елементи керування.
        /// </summary>
        /// <param name="isTestActive">Якщо true — ховає меню, якщо false — повертає його на місце.</param>
        public void ToggleTestMode(bool isTestActive)
        {
            if (isTestActive)
            {
                // Ховаємо меню і згортаємо колонку
                SidebarMenu.Visibility = Visibility.Collapsed;
                SidebarColumn.Width = new GridLength(0);

                // Розтягуємо Frame на весь екран (на обидві колонки)
                Grid.SetColumn(MainFrame, 0);
                Grid.SetColumnSpan(MainFrame, 2);
            }
            else
            {
                // Повертаємо видимість меню
                SidebarMenu.Visibility = Visibility.Visible;
                SidebarColumn.Width = new GridLength(220);

                // Повертаємо Frame на своє початкове місце
                Grid.SetColumn(MainFrame, 1);
                Grid.SetColumnSpan(MainFrame, 1);
            }
        }

        // --- Обробники кнопок меню ---

        /// <summary>
        /// Обробник переходу на головну сторінку. Відновлює видимість бічного меню.
        /// </summary>
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            ToggleTestMode(false);
            MainFrame.Navigate(new HomePage());
        }

        /// <summary>
        /// Обробник переходу до профілю користувача. Відновлює видимість бічного меню.
        /// </summary>
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            ToggleTestMode(false);
            MainFrame.Navigate(new ProfilePage());
        }

        /// <summary>
        /// Обробник переходу до розділу налаштувань. Відновлює видимість бічного меню.
        /// </summary>
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            ToggleTestMode(false);
            MainFrame.Navigate(new SettingsPage());
        }

        /// <summary>
        /// Обробник переходу до сторінки інструкцій. Відновлює видимість бічного меню.
        /// </summary>
        private void BtnInstruction_Click(object sender, RoutedEventArgs e)
        {
            ToggleTestMode(false);
            MainFrame.Navigate(new InstructionPage());
        }

        /// <summary>
        /// Обробник завершення роботи програми. 
        /// Гарантує збереження всіх поточних змін користувача перед закриттям застосунку.
        /// </summary>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (AppData != null) DataManager.SaveData(AppData);
            Application.Current.Shutdown();
        }
    }
}