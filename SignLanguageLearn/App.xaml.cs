using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SignLanguageLearn.Services;

namespace SignLanguageLearn
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Читаємо JSON при старті
            var data = DataManager.LoadData();

            if (data != null)
            {
                // 2. Записуємо в MainWindow (тепер воно знайде AppData через повне ім'я)
                SignLanguageLearn.MainWindow.AppData = data;

                // 3. Фарбуємо додаток
                ColorUpdate(data.AppSettings.CurrentTheme == "Dark");
            }
        }

        public static void StringUpdate(string langFile)
        {
            // Порожній метод для сумісності
        }

        public static void ColorUpdate(bool isDark)
        {
            // Визначаємо кольори
            Color bgColor = isDark ? Color.FromRgb(30, 27, 41) : Color.FromRgb(205, 233, 254);
            Color cardColor = isDark ? Color.FromRgb(45, 41, 59) : Colors.White;
            Color textColor = isDark ? Colors.White : Color.FromRgb(45, 52, 54);

            // Оновлюємо ресурси через Current.Resources
            var res = Application.Current.Resources;

            // Оновлюємо ВСІ можливі ключі одночасно
            res["BackgroundBrush"] = res["PrimaryBackground"] = new SolidColorBrush(bgColor);
            res["CardBrush"] = res["CardBackground"] = new SolidColorBrush(cardColor);
            res["TextBrush"] = res["MainText"] = new SolidColorBrush(textColor);
        }
    }
}