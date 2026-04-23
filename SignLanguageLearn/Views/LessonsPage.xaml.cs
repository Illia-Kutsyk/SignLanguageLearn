using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SignLanguageLearn.Services;

namespace SignLanguageLearn.Views
{
    public partial class LessonsPage : Page
    {
        public LessonsPage()
        {
            InitializeComponent();
        }

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

        private void Play_Click(object sender, RoutedEventArgs e) => LessonVideo.Play();
        private void Pause_Click(object sender, RoutedEventArgs e) => LessonVideo.Pause();
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            LessonVideo.Position = TimeSpan.Zero;
            LessonVideo.Play();
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            LessonVideo.Stop();
            LessonContent.Visibility = Visibility.Collapsed;
            CategoryMenu.Visibility = Visibility.Visible;
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LessonVideo != null) LessonVideo.SpeedRatio = e.NewValue;
        }

        private void LessonVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            LessonVideo.Position = TimeSpan.Zero;
            LessonVideo.Stop();
        }
    }
}