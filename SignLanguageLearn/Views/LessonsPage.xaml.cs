using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SignLanguageLearn.Services;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Логіка взаємодії для сторінки уроків (LessonsPage.xaml).
    /// Забезпечує вибір категорій навчання, відображення списку жестів 
    /// та керування відтворенням відеоуроків.
    /// </summary>
    public partial class LessonsPage : Page
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="LessonsPage"/>.
        /// </summary>
        public LessonsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обробник натискання на кнопку категорії (Алфавіт або Фрази).
        /// Перемикає інтерфейс на список уроків та заповнює його даними відповідно до обраної мови.
        /// </summary>
        /// <param name="sender">Кнопка категорії з відповідним Tag.</param>
        /// <param name="e">Аргументи події.</param>
        private void Category_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.Tag == null) return;

            string category = btn.Tag.ToString();

            CategoryMenu.Visibility = Visibility.Collapsed;
            LessonContent.Visibility = Visibility.Visible;

            ItemsList.Items.Clear();

            string lang = "UA";
            if (MainWindow.AppData != null && MainWindow.AppData.AppSettings != null)
            {
                lang = MainWindow.AppData.AppSettings.CurrentLanguage;
            }

            if (category == "Alphabet")
            {
                string[] alphabet;
                if (lang == "EN")
                {
                    alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
                }
                else
                {
                    alphabet = new string[] { "А", "Б", "В", "Г", "Ґ", "Д", "Е", "Є", "Ж", "З" };
                }

                foreach (string l in alphabet)
                {
                    ItemsList.Items.Add(new ListBoxItem { Content = l });
                }
            }
            else if (category == "Phrases")
            {
                if (lang == "EN")
                {
                    ItemsList.Items.Add(new ListBoxItem { Content = "Hello" });
                    ItemsList.Items.Add(new ListBoxItem { Content = "Thanks" });
                }
                else
                {
                    ItemsList.Items.Add(new ListBoxItem { Content = "Привіт" });
                    ItemsList.Items.Add(new ListBoxItem { Content = "Дякую" });
                }
            }
        }

        /// <summary>
        /// Обробник зміни вибору у списку жестів.
        /// Отримує назву обраного елемента та запускає відповідне відео.
        /// </summary>
        private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selected = ItemsList.SelectedItem as ListBoxItem;
            if (selected != null && selected.Content != null)
            {
                string text = selected.Content.ToString();
                string code = GetFileName(text);
                PlayVideo(code);
            }
        }

        /// <summary>
        /// Формує шлях до медіафайлу та запускає відтворення відео з урахуванням поточної швидкості.
        /// </summary>
        /// <param name="code">Кодова назва жесту для пошуку файлу.</param>
        private void PlayVideo(string code)
        {
            try
            {
                string lang = "ua";
                if (MainWindow.AppData != null && MainWindow.AppData.AppSettings != null)
                {
                    lang = MainWindow.AppData.AppSettings.CurrentLanguage.ToLower();
                }

                string fileName = lang + "_" + code + ".mp4";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Videos", fileName);

                if (File.Exists(path))
                {
                    LessonVideo.Source = new Uri(path);
                    LessonVideo.SpeedRatio = SpeedSlider.Value;
                    LessonVideo.Play();
                }
                else
                {
                    MessageBox.Show("Файл не знайдено: " + fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка плеєра: " + ex.Message);
            }
        }

        /// <summary>
        /// Перетворює відображуваний текст жесту у технічне ім'я файлу (латиницею).
        /// </summary>
        /// <param name="text">Текст жесту (буква або слово).</param>
        /// <returns>Рядок, що використовується як частина імені файлу.</returns>
        private string GetFileName(string text)
        {
            string input = text.ToLower();
            switch (input)
            {
                case "а": return "a";
                case "б": return "b";
                case "в": return "v";
                case "г": return "h";
                case "ґ": return "g";
                case "д": return "d";
                case "е": return "e";
                case "є": return "ye";
                case "ж": return "j";
                case "з": return "z";

                case "a": return "a";
                case "b": return "b";
                case "c": return "c";
                case "d": return "d";
                case "e": return "e";
                case "f": return "f";
                case "g": return "g";
                case "h": return "h";
                case "i": return "i";
                case "j": return "j";

                case "привіт":
                case "hello": return "hello";
                case "дякую":
                case "thanks": return "thanks";

                default: return input;
            }
        }

        /// <summary>
        /// Відновлює відтворення відео.
        /// </summary>
        private void Play_Click(object sender, RoutedEventArgs e) => LessonVideo.Play();

        /// <summary>
        /// Призупиняє відтворення відео.
        /// </summary>
        private void Pause_Click(object sender, RoutedEventArgs e) => LessonVideo.Pause();

        /// <summary>
        /// Скидає відео на початок та запускає відтворення.
        /// </summary>
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            LessonVideo.Position = TimeSpan.Zero;
            LessonVideo.Play();
        }

        /// <summary>
        /// Повертає користувача до меню вибору категорій, зупиняючи плеєр.
        /// </summary>
        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            LessonVideo.Stop();
            LessonContent.Visibility = Visibility.Collapsed;
            CategoryMenu.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Оновлює швидкість відтворення відео при зміні положення повзунка.
        /// </summary>
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LessonVideo != null) LessonVideo.SpeedRatio = e.NewValue;
        }

        /// <summary>
        /// Скидає позицію відео після завершення відтворення.
        /// </summary>
        private void LessonVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            LessonVideo.Position = TimeSpan.Zero;
            LessonVideo.Stop();
        }
    }
}