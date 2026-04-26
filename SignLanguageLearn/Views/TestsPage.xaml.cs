using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Логіка взаємодії для сторінки тестування знань (TestPage.xaml).
    /// Забезпечує вибір теми, генерацію питань, відтворення відео та підрахунок результатів.
    /// </summary>
    public partial class TestPage : Page
    {
        private string _correctAnswer;
        private int _totalQuestions = 0;
        private int _correctCount = 0;
        private readonly Random _random = new Random();
        private string[] _testItems;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="TestPage"/>.
        /// </summary>
        public TestPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обробник події натискання на кнопку вибору теми тесту.
        /// Приховує меню, ініціалізує дані для обраної теми та запускає перше питання.
        /// </summary>
        /// <param name="sender">Джерело події (кнопка теми).</param>
        /// <param name="e">Аргументи події.</param>
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

        /// <summary>
        /// Налаштовує масив доступних тестових елементів (букв або слів) 
        /// залежно від обраної теми та поточної мови застосунку (UA/EN).
        /// </summary>
        /// <param name="theme">Тема тесту (наприклад, "Alphabet").</param>
        private void SetupTestData(string theme)
        {
            string lang = "UA";
            if (MainWindow.AppData != null && MainWindow.AppData.AppSettings != null)
            {
                lang = MainWindow.AppData.AppSettings.CurrentLanguage;
            }

            if (theme == "Alphabet")
            {
                _testItems = (lang == "EN")
                    ? new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" }
                    : new[] { "А", "Б", "В", "Г", "Ґ", "Д", "Е", "Є", "Ж", "З" };
            }
            else
            {
                _testItems = (lang == "EN")
                    ? new[] { "Hello", "Thanks" }
                    : new[] { "Привіт", "Дякую" };
            }
        }

        /// <summary>
        /// Генерує нове тестове питання.
        /// Випадковим чином обирає правильну відповідь, формує список хибних варіантів,
        /// перемішує їх та оновлює інтерфейс користувача.
        /// </summary>
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

            if (shuffled.Count > 1)
            {
                Answer2.Content = shuffled[1];
                Answer2.Visibility = Visibility.Visible;
            }
            else Answer2.Visibility = Visibility.Collapsed;

            if (shuffled.Count > 2)
            {
                Answer3.Content = shuffled[2];
                Answer3.Visibility = Visibility.Visible;
            }
            else Answer3.Visibility = Visibility.Collapsed;

            PlayTestVideo(_correctAnswer);
        }

        /// <summary>
        /// Формує шлях до відеофайлу на основі переданого жесту та відтворює його у плеєрі.
        /// </summary>
        /// <param name="letter">Слово або буква для відтворення.</param>
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
            catch { /* Ігноруємо помилки завантаження відео */ }
        }

        /// <summary>
        /// Асинхронний обробник натискання на кнопку відповіді.
        /// Перевіряє правильність вибору, оновлює статистику, підсвічує кнопку 
        /// відповідним кольором та генерує наступне питання після затримки.
        /// </summary>
        /// <param name="sender">Кнопка з обраною відповіддю.</param>
        /// <param name="e">Аргументи події.</param>
        private async void Answer_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            SetButtonsEnabled(false);

            _totalQuestions++;
            bool isCorrect = btn.Content != null && btn.Content.ToString() == _correctAnswer;

            if (isCorrect)
            {
                _correctCount++;
                btn.Background = Brushes.LightGreen;
            }
            else
            {
                btn.Background = Brushes.Crimson;
                btn.Foreground = Brushes.White;
            }

            ScoreText.Text = string.Format("Рахунок: {0}/{1}", _correctCount, _totalQuestions);

            await Task.Delay(800);

            btn.ClearValue(Button.BackgroundProperty);
            btn.ClearValue(Button.ForegroundProperty);

            SetButtonsEnabled(true);
            GenerateQuestion();
        }

        /// <summary>
        /// Блокує або розблоковує всі кнопки варіантів відповідей, 
        /// щоб уникнути подвійних натискань під час анімації затримки.
        /// </summary>
        /// <param name="isEnabled">Стан доступності кнопок (true - доступні, false - заблоковані).</param>
        private void SetButtonsEnabled(bool isEnabled)
        {
            if (AnswersGrid == null) return;

            foreach (UIElement child in AnswersGrid.Children)
            {
                Button b = child as Button;
                if (b != null) b.IsEnabled = isEnabled;
            }
        }

        /// <summary>
        /// Трансформує текст (букву чи слово) у відповідну назву відеофайлу.
        /// </summary>
        /// <param name="text">Текст жесту.</param>
        /// <returns>Форматована назва файлу без розширення.</returns>
        private string GetFileName(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
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

        /// <summary>
        /// Обробник події для кнопки "Назад".
        /// Зупиняє відео, скидає прогрес тестування та повертає користувача до меню вибору теми.
        /// </summary>
        /// <param name="sender">Джерело події.</param>
        /// <param name="e">Аргументи події.</param>
        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            TestVideo.Stop();
            TestInterface.Visibility = Visibility.Collapsed;
            TestMenu.Visibility = Visibility.Visible;
            _totalQuestions = 0;
            _correctCount = 0;
            ScoreText.Text = "Рахунок: 0/0";
        }

        /// <summary>
        /// Обробник події завершення відтворення відео.
        /// Забезпечує постійне зациклення відеофрагменту доти, доки користувач не дасть відповідь.
        /// </summary>
        /// <param name="sender">Медіаелемент, що завершив відтворення.</param>
        /// <param name="e">Аргументи події.</param>
        private void TestVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            TestVideo.Position = TimeSpan.Zero;
            TestVideo.Play();
        }
    }
}