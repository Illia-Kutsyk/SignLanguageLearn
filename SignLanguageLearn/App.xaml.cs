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

            var data = DataManager.LoadData();

            if (data != null)
            {
                SignLanguageLearn.MainWindow.AppData = data;
                ColorUpdate(data.AppSettings.CurrentTheme == "Dark");
            }
        }

        public static void StringUpdate(string langFile) { }

        public static void ColorUpdate(bool isDark)
        {
            Color bgColor = isDark ? Color.FromRgb(30, 27, 41) : Color.FromRgb(205, 233, 254);
            Color cardColor = isDark ? Color.FromRgb(45, 41, 59) : Colors.White;
            Color textColor = isDark ? Colors.White : Color.FromRgb(45, 52, 54);

            Color fieldColor = isDark ? Color.FromRgb(35, 32, 48) : Colors.White;

            var res = Application.Current.Resources;

            res["BackgroundBrush"] = res["PrimaryBackground"] = new SolidColorBrush(bgColor);
            res["CardBrush"] = res["CardBackground"] = new SolidColorBrush(cardColor);
            res["TextBrush"] = res["MainText"] = new SolidColorBrush(textColor);

            res["FieldBackBrush"] = new SolidColorBrush(fieldColor);

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