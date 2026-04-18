using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
            var btn = sender as Button;
            string category = btn.Tag.ToString();

            CategoryMenu.Visibility = Visibility.Collapsed;
            LessonContent.Visibility = Visibility.Visible;

            ItemsList.Items.Clear();
            if (category == "Alphabet")
            {
                string[] alphabet = { "А", "Б", "В", "Г", "Д", "Е", "Є", "Ж", "З" };
                foreach (var l in alphabet) ItemsList.Items.Add(new ListBoxItem { Content = l });
            }
            else if (category == "Phrases")
            {
                ItemsList.Items.Add(new ListBoxItem { Content = "Привіт" });
                ItemsList.Items.Add(new ListBoxItem { Content = "Дякую" });
            }
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            LessonVideo.Stop();
            LessonContent.Visibility = Visibility.Collapsed;
            CategoryMenu.Visibility = Visibility.Visible;
        }

        private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsList.SelectedItem is ListBoxItem selected)
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
                string lang = DataManager.LoadData()?.AppSettings?.CurrentLanguage?.ToLower() ?? "ua";
                string fullFileName = $"{lang}_{code}.mp4";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Videos", fullFileName);

                if (File.Exists(path))
                {
                    LessonVideo.Source = new Uri(path);
                    LessonVideo.SpeedRatio = SpeedSlider.Value;
                    LessonVideo.Play();
                }
                else
                {
                    MessageBox.Show($"Файл не знайдено: {fullFileName}");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private string GetFileName(string text)
        {
            switch (text.ToLower())
            {
                case "а": return "a";
                case "б": return "b";
                case "в": return "v";
                case "г": return "g";
                case "д": return "d";
                case "е": return "e";
                case "є": return "ye";
                case "ж": return "j";
                case "з": return "z";
                case "привіт": return "hello";
                case "дякую": return "thanks";
                default: return text;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e) => LessonVideo.Play();
        private void Pause_Click(object sender, RoutedEventArgs e) => LessonVideo.Pause();
        private void Restart_Click(object sender, RoutedEventArgs e) { LessonVideo.Position = TimeSpan.Zero; LessonVideo.Play(); }

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