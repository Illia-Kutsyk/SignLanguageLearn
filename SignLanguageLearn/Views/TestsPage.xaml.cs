using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SignLanguageLearn.Views
{
    public partial class TestPage : Page
    {
        private string _correctAnswer;
        private int _totalQuestions = 0;
        private int _correctCount = 0;
        private readonly Random _random = new Random(); // Поле тільки для читання
        private string[] _testItems;

        public TestPage()
        {
            InitializeComponent();
        }

        private void SelectTheme_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                SetupTestData(btn.Tag.ToString());
                TestMenu.Visibility = Visibility.Collapsed;
                TestInterface.Visibility = Visibility.Visible;
                GenerateQuestion();
            }
        }

        private void SetupTestData(string theme)
        {
            // Беремо мову напряму, без зайвих змінних
            string lang = "UA";
            if (MainWindow.AppData != null && MainWindow.AppData.AppSettings != null)
            {
                lang = MainWindow.AppData.AppSettings.CurrentLanguage;
            }

            if (theme == "Alphabet")
            {
                _testItems = lang == "EN"
                    ? new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" }
                    : new[] { "А", "Б", "В", "Г", "Ґ", "Д", "Е", "Є", "Ж", "З" };
            }
            else
            {
                _testItems = lang == "EN" ? new[] { "Hello", "Thanks" } : new[] { "Привіт", "Дякую" };
            }
        }

        private void GenerateQuestion()
        {
            if (_testItems == null || _testItems.Length == 0) return;

            _correctAnswer = _testItems[_random.Next(_testItems.Length)];
            List<string> options = new List<string> { _correctAnswer };

            while (options.Count < Math.Min(3, _testItems.Length))
            {
                string randomItem = _testItems[_random.Next(_testItems.Length)];
                if (!options.Contains(randomItem)) options.Add(randomItem);
            }

            var shuffled = options.OrderBy(x => _random.Next()).ToList();

            Answer1.Content = shuffled[0];

            // Налаштування видимості кнопок
            if (shuffled.Count > 1) { Answer2.Content = shuffled[1]; Answer2.Visibility = Visibility.Visible; }
            else Answer2.Visibility = Visibility.Collapsed;

            if (shuffled.Count > 2) { Answer3.Content = shuffled[2]; Answer3.Visibility = Visibility.Visible; }
            else Answer3.Visibility = Visibility.Collapsed;

            PlayTestVideo(_correctAnswer);
        }

        private void PlayTestVideo(string letter)
        {
            try
            {
                string lang = "ua";
                if (MainWindow.AppData != null && MainWindow.AppData.AppSettings != null)
                    lang = MainWindow.AppData.AppSettings.CurrentLanguage.ToLower();

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Videos", lang + "_" + GetFileName(letter) + ".mp4");
                if (File.Exists(path))
                {
                    TestVideo.Source = new Uri(path);
                    TestVideo.Play();
                }
            }
            catch { }
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.Content == null) return;

            _totalQuestions++;
            if (btn.Content.ToString() == _correctAnswer)
            {
                _correctCount++;
                MessageBox.Show("Правильно! ✨");
            }
            else MessageBox.Show("Неправильно. Це був жест: " + _correctAnswer);

            ScoreText.Text = "Рахунок: " + _correctCount + "/" + _totalQuestions;
            GenerateQuestion();
        }

        private string GetFileName(string text)
        {
            string input = text.ToLower();
            switch (input)
            {
                case "а": case "a": return "a";
                case "б": case "b": return "b";
                case "в": return "v";
                case "c": return "c";
                case "г": case "h": return "h";
                case "ґ": case "g": return "g";
                case "д": case "d": return "d";
                case "е": case "e": return "e";
                case "є": return "ye";
                case "ж": case "j": return "j";
                case "з": case "z": return "z";
                case "привіт": case "hello": return "hello";
                case "дякую": case "thanks": return "thanks";
                default: return input;
            }
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            TestVideo.Stop();
            TestInterface.Visibility = Visibility.Collapsed;
            TestMenu.Visibility = Visibility.Visible;
            _totalQuestions = 0;
            _correctCount = 0;
            ScoreText.Text = "Рахунок: 0/0";
        }

        private void TestVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            TestVideo.Position = TimeSpan.Zero;
            TestVideo.Play();
        }
    }
}