using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SignLanguageLearn.Views
{
    public partial class DictionaryPage : Page
    {
        public DictionaryPage()
        {
            InitializeComponent();
        }

        private void OpenWebDictionary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://www.spreadthesign.com") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося відкрити посилання: " + ex.Message);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}