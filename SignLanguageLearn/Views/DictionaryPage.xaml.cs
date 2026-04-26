using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Логіка взаємодії для сторінки словника (DictionaryPage.xaml).
    /// Забезпечує доступ до внутрішніх словникових ресурсів та зовнішніх онлайн-платформ 
    /// для глибшого вивчення жестів.
    /// </summary>
    public partial class DictionaryPage : Page
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="DictionaryPage"/>.
        /// </summary>
        public DictionaryPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обробник події для відкриття зовнішнього веб-словника жестової мови.
        /// Використовує системну оболонку для запуску браузера за замовчуванням.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію.</param>
        /// <param name="e">Аргументи події.</param>
        private void OpenWebDictionary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Створення процесу для відкриття URL-адреси у браузері за замовчуванням
                Process.Start(new ProcessStartInfo("https://www.spreadthesign.com") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Виведення повідомлення у разі помилки запуску зовнішнього процесу
                MessageBox.Show("Не вдалося відкрити посилання: " + ex.Message);
            }
        }

        /// <summary>
        /// Обробник натискання кнопки "Назад".
        /// Повертає користувача на попередню сторінку в історії навігації додатка.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію.</param>
        /// <param name="e">Аргументи події.</param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}