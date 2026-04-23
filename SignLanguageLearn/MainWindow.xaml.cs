using System;
using System.Windows;
using System.Windows.Media;
using SignLanguageLearn.Services;
using SignLanguageLearn.Views;

namespace SignLanguageLearn
{
    public partial class MainWindow : Window
    {
        public static RootData AppData { get; set; }

        public MainWindow()
        {
            if (AppData == null) AppData = DataManager.LoadData();

            if (AppData != null && AppData.AppSettings != null)
            {
                App.ColorUpdate(AppData.AppSettings.CurrentTheme == "Dark");
            }

            InitializeComponent();

            MainFrame.Navigate(new HomePage());
        }

        public void UpdateVisuals()
        {
            if (AppData == null || AppData.AppSettings == null) return;
            App.ColorUpdate(AppData.AppSettings.CurrentTheme == "Dark");
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new HomePage());

        private void BtnProfile_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new ProfilePage());

        private void BtnSettings_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new SettingsPage());

        private void BtnInstruction_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new InstructionPage());

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (AppData != null) DataManager.SaveData(AppData);
            Application.Current.Shutdown();
        }
    }
}