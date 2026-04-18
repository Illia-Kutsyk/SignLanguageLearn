using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SignLanguageLearn
{
    public partial class App : Application
    {
        public static void StringUpdate(string langFileName)
        {
            try
            {
                var oldLang = Current.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Lang"));

                if (oldLang != null)
                    Current.Resources.MergedDictionaries.Remove(oldLang);

                ResourceDictionary dict = new ResourceDictionary
                {
                    Source = new Uri($"/{langFileName}", UriKind.Relative)
                };
                Current.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex) { MessageBox.Show("Помилка словника: " + ex.Message); }
        }

        public static void ColorUpdate(bool isDark)
        {
            if (isDark)
            {
                Current.Resources["PrimaryBackground"] = new SolidColorBrush(Color.FromRgb(30, 27, 41));
                Current.Resources["CardBackground"] = new SolidColorBrush(Color.FromRgb(45, 41, 59));
                Current.Resources["MainText"] = new SolidColorBrush(Colors.White);
            }
            else
            {
                Current.Resources["PrimaryBackground"] = new SolidColorBrush(Color.FromRgb(205, 233, 254));
                Current.Resources["CardBackground"] = new SolidColorBrush(Colors.White);
                Current.Resources["MainText"] = new SolidColorBrush(Color.FromRgb(45, 52, 54));
            }
        }
    }
}