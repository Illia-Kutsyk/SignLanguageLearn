using System;
using System.Windows;
using System.Windows.Controls;
using SignLanguageLearn.Services;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Логіка взаємодії для сторінки налаштувань (SettingsPage.xaml).
    /// Забезпечує зміну мови локалізації та візуальної теми (світла/темна),
    /// а також їх збереження у профіль користувача.
    /// </summary>
    public partial class SettingsPage : Page
    {
        /// <summary>
        /// Прапорець, що запобігає хибному спрацьовуванню подій Checked 
        /// під час програмної ініціалізації перемикачів при завантаженні сторінки.
        /// </summary>
        private bool _isReady = false;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="SettingsPage"/>.
        /// Завантажує поточні налаштування та готує інтерфейс до роботи.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
            LoadToggles();
            _isReady = true;
        }

        /// <summary>
        /// Зчитує поточні параметри мови та теми з глобальних даних застосунку 
        /// і встановлює відповідні перемикачі (RadioButtons) в активний стан.
        /// </summary>
        private void LoadToggles()
        {
            if (MainWindow.AppData == null) return;

            // Тимчасово блокуємо виклик TriggerUpdate під час налаштування
            _isReady = false;

            if (MainWindow.AppData.AppSettings.CurrentLanguage == "UA")
                RbUa.IsChecked = true;
            else
                RbEn.IsChecked = true;

            if (MainWindow.AppData.AppSettings.CurrentTheme == "Dark")
                RbDark.IsChecked = true;
            else
                RbLight.IsChecked = true;

            // Знімаємо блокування після успішного налаштування
            _isReady = true;
        }

        /// <summary>
        /// Зберігає оновлені налаштування у файл конфігурації (JSON), 
        /// ініціює глобальну зміну кольорової теми та перезавантажує поточну сторінку 
        /// для миттєвого застосування нової мови інтерфейсу.
        /// </summary>
        private void TriggerUpdate()
        {
            // Уникаємо збереження, якщо сторінка ще завантажується
            if (!_isReady || MainWindow.AppData == null) return;

            DataManager.SaveData(MainWindow.AppData);

            try
            {
                App.ColorUpdate(MainWindow.AppData.AppSettings.CurrentTheme == "Dark");
            }
            catch { /* Обробка винятку, якщо метод ColorUpdate відсутній або виникла помилка */ }

            // Перезавантажуємо сторінку налаштувань для оновлення локалізованих ресурсів
            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                mainWin.MainFrame.Navigate(new SettingsPage());
            }
        }

        /// <summary>
        /// Обробник події встановлення української мови інтерфейсу.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Аргументи події.</param>
        private void RbUa_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AppData != null)
            {
                MainWindow.AppData.AppSettings.CurrentLanguage = "UA";
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Обробник події встановлення англійської мови інтерфейсу.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Аргументи події.</param>
        private void RbEn_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AppData != null)
            {
                MainWindow.AppData.AppSettings.CurrentLanguage = "EN";
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Обробник події вибору світлої візуальної теми застосунку.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Аргументи події.</param>
        private void RbLight_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AppData != null)
            {
                MainWindow.AppData.AppSettings.CurrentTheme = "Light";
                TriggerUpdate();
            }
        }

        /// <summary>
        /// Обробник події вибору темної візуальної теми застосунку.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Аргументи події.</param>
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