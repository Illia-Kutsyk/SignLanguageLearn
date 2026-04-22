using System;
using System.Windows;
using System.Windows.Media;
using SignLanguageLearn.Services;
using SignLanguageLearn.Views; // Додано для прямого доступу до класів сторінок

namespace SignLanguageLearn
{
    public partial class MainWindow : Window
    {
        // Дані застосунку
        public static RootData AppData { get; set; }

        public MainWindow()
        {
            // Завантаження даних, якщо вони ще не ініціалізовані
            if (AppData == null) AppData = DataManager.LoadData();

            // Встановлення теми перед ініціалізацією компонентів
            if (AppData != null && AppData.AppSettings != null)
            {
                App.ColorUpdate(AppData.AppSettings.CurrentTheme == "Dark");
            }

            InitializeComponent();

            // Завантажуємо головну сторінку за замовчуванням при старті
            MainFrame.Navigate(new HomePage());
        }

        public void UpdateVisuals()
        {
            if (AppData == null || AppData.AppSettings == null) return;
            App.ColorUpdate(AppData.AppSettings.CurrentTheme == "Dark");
        }

        // Навігація
        private void BtnHome_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new HomePage());

        private void BtnProfile_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new ProfilePage());

        private void BtnSettings_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new SettingsPage());

        // Метод для відкриття інструкції
        private void BtnInstruction_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new InstructionPage());

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (AppData != null) DataManager.SaveData(AppData);
            Application.Current.Shutdown();
        }
    }
}