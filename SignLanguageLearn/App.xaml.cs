using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SignLanguageLearn.Services;

namespace SignLanguageLearn
{
    /// <summary>
    /// Основний клас застосунку, що керує його життєвим циклом.
    /// Відповідає за початкове завантаження даних та динамічне керування 
    /// спільними ресурсами (темами оформлення).
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Обробник події запуску застосунку.
        /// Виконує попередню підготовку даних та налаштування інтерфейсу 
        /// перед відображенням головного вікна.
        /// </summary>
        /// <param name="e">Аргументи події запуску.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Завантажуємо дані користувача та конфігурацію з файлу
            var data = DataManager.LoadData();

            if (data != null)
            {
                // Передаємо завантажені дані в MainWindow для глобального доступу
                SignLanguageLearn.MainWindow.AppData = data;

                // Застосовуємо тему оформлення, що була збережена минулого разу
                ColorUpdate(data.AppSettings.CurrentTheme == "Dark");
            }
        }

        /// <summary>
        /// Резервний метод для оновлення локалізації рядків (буде реалізовано у майбутніх версіях).
        /// </summary>
        /// <param name="langFile">Шлях або назва файлу локалізації.</param>
        public static void StringUpdate(string langFile) { }

        /// <summary>
        /// Динамічно оновлює ресурси кольорів (Brushes) у словнику ресурсів застосунку.
        /// Дозволяє змінювати тему інтерфейсу (світла/темна) «на льоту» без перезапуску вікон.
        /// </summary>
        /// <param name="isDark">Визначає, чи слід застосувати темну тему.</param>
        public static void ColorUpdate(bool isDark)
        {
            // Визначення палітри кольорів для основних елементів
            Color bgColor = isDark ? Color.FromRgb(30, 27, 41) : Color.FromRgb(205, 233, 254);
            Color cardColor = isDark ? Color.FromRgb(45, 41, 59) : Colors.White;
            Color textColor = isDark ? Colors.White : Color.FromRgb(45, 52, 54);
            Color fieldColor = isDark ? Color.FromRgb(35, 32, 48) : Colors.White;

            var res = Application.Current.Resources;

            // Оновлення ресурсів, на які посилаються XAML-сторінки через StaticResource/DynamicResource
            res["BackgroundBrush"] = res["PrimaryBackground"] = new SolidColorBrush(bgColor);
            res["CardBrush"] = res["CardBackground"] = new SolidColorBrush(cardColor);
            res["TextBrush"] = res["MainText"] = new SolidColorBrush(textColor);
            res["FieldBackBrush"] = new SolidColorBrush(fieldColor);

            // Налаштування акцентного кольору (для кнопок та виділень)
            if (isDark)
            {
                res["AccentBrush"] = new SolidColorBrush(Color.FromRgb(100, 115, 125));
            }
            else
            {
                res["AccentBrush"] = new SolidColorBrush(Color.FromRgb(69, 90, 100));
            }
        }
    }
}