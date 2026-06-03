using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Сторінка тестування, яка керує процесом генерації питань, відтворенням відеоматеріалів,
    /// перевіркою відповідей, таймінгом та збереженням досягнень користувача.
    /// </summary>
    public partial class TestPage : Page
    {
        /// <summary>
        /// Рядкове значення правильної відповіді для поточного питання.
        /// </summary>
        private string _correctAnswer;

        /// <summary>
        /// Порядковий номер поточного питання.
        /// </summary>
        private int _currentQuestion;

        /// <summary>
        /// Кількість правильних відповідей, наданих користувачем під час тесту.
        /// </summary>
        private int _correctCount;

        /// <summary>
        /// Ідентифікатор поточної категорії тесту.
        /// </summary>
        private int _categoryId;

        /// <summary>
        /// Загальна кількість питань у поточному тесті.
        /// </summary>
        private int _totalQuestions;

        /// <summary>
        /// Час початку проходження тесту.
        /// </summary>
        private DateTime _testStartTime;

        /// <summary>
        /// Таймер для відліку та відображення часу проходження тесту.
        /// </summary>
        private readonly DispatcherTimer _dispatcherTimer;

        /// <summary>
        /// Список елементів (слів або літер), доступних для формування питань у вибраній категорії.
        /// </summary>
        private List<string> _availableItems = new List<string>();

        /// <summary>
        /// Список елементів, які вже були використані в поточному сеансі тестування.
        /// </summary>
        private readonly List<string> _usedItems = new List<string>();

        /// <summary>
        /// Генератор випадкових чисел для змішування питань та варіантів відповідей.
        /// </summary>
        private readonly Random _random = new Random();

        /// <summary>
        /// Словник, що відображає текстові слова на назви відповідних відеофайлів (без урахування регістру).
        /// </summary>
        private Dictionary<string, string> wordToVideoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Ініціалізує новий екземпляр сторінки тестування, налаштовує таймер, завантажує мапу слів та перевіряє доступність категорій.
        /// </summary>
        public TestPage()
        {
            InitializeComponent();

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ToggleTestMode(false);
            }

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;

            LoadWordMap();
            CheckLocks();
        }

        /// <summary>
        /// Завантажує словник відповідності слів до відеофайлів із зовнішнього файлу JSON.
        /// </summary>
        private void LoadWordMap()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "wordMap.json");

                if (File.Exists(path))
                {
                    string jsonContent = File.ReadAllText(path);
                    var deserializedMap = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

                    if (deserializedMap != null)
                    {
                        wordToVideoMap = new Dictionary<string, string>(deserializedMap, StringComparer.OrdinalIgnoreCase);
                    }
                }
                else
                {
                    MessageBox.Show("Критична помилка: Файл конфігурації тесту 'wordMap.json' не знайдено!", "Помилка архітектури");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при завантаженні словника: " + ex.Message, "Помилка JSON");
            }
        }

        /// <summary>
        /// Обробляє кожен такт таймера, оновлюючи відображення тривалості тесту на формі.
        /// </summary>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - _testStartTime;
            if (TimerText != null)
            {
                TimerText.Text = string.Format("⏱ {0:mm\\:ss}", elapsed);
            }
        }

        /// <summary>
        /// Перевіряє за допомогою рефлексії, чи увімкнено режим підвищеної складності (Hardcore) у налаштуваннях додатка.
        /// </summary>
        /// <returns>Значення true, якщо режим Hardcore активний; інакше false.</returns>
        private bool IsHardcoreEnabled()
        {
            try
            {
                var data = MainWindow.AppData;
                if (data == null) return false;

                var settingsProp = data.GetType().GetProperty("AppSettings", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)?.GetValue(data)
                                ?? data.GetType().GetField("AppSettings", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)?.GetValue(data);

                if (settingsProp == null) return false;

                var diffValue = settingsProp.GetType().GetProperty("Difficulty", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)?.GetValue(settingsProp)
                             ?? settingsProp.GetType().GetField("Difficulty", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)?.GetValue(settingsProp);

                if (diffValue != null && diffValue.ToString().Equals("Hardcore", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                var hardcoreProp = settingsProp.GetType().GetProperty("IsHardcore", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)?.GetValue(settingsProp)
                                ?? settingsProp.GetType().GetField("IsHardcore", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)?.GetValue(settingsProp)
                                ?? settingsProp.GetType().GetProperty("HardcoreMode", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)?.GetValue(settingsProp)
                                ?? settingsProp.GetType().GetField("HardcoreMode", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)?.GetValue(settingsProp);

                if (hardcoreProp is bool b && b) return true;
                if (hardcoreProp?.ToString().Equals("true", StringComparison.OrdinalIgnoreCase) == true) return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Керує доступністю або видимістю бічної навігаційної панелі головного вікна під час тестування.
        /// </summary>
        /// <param name="isAllowed">Значення true, якщо навігація дозволена; false, якщо її потрібно заблокувати.</param>
        private void SetNavigationState(bool isAllowed)
        {
            MainWindow mainWin = Application.Current.MainWindow as MainWindow;
            if (mainWin == null) return;

            UIElement sidebar = mainWin.FindName("Sidebar") as UIElement
                         ?? mainWin.FindName("NavigationPanel") as UIElement
                         ?? mainWin.FindName("SidebarMenu") as UIElement
                         ?? mainWin.FindName("NavMenu") as UIElement;

            if (sidebar != null)
            {
                sidebar.Visibility = isAllowed ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                try
                {
                    foreach (var child in LogicalTreeHelper.GetChildren(mainWin))
                    {
                        FrameworkElement fe = child as FrameworkElement;
                        if (fe != null && fe.Name != "MainFrame" && !(child is Frame))
                        {
                            fe.IsEnabled = isAllowed;
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Перевіряє прогрес авторизованого користувача та блокує або розблоковує кнопки вибору тем тестування.
        /// </summary>
        private void CheckLocks()
        {
            BtnNumbersTest.IsEnabled = false; TxtNumbersTest.Text = "🔒 Числа та Математика";
            BtnPropertiesTest.IsEnabled = false; TxtPropertiesTest.Text = "🔒 Кольори та Властивості";
            BtnPhrasesTest.IsEnabled = false; TxtPhrasesTest.Text = "🔒 Базові фрази";
            BtnTimeTest.IsEnabled = false; TxtTimeTest.Text = "🔒 Час та Календар";
            BtnSocietyTest.IsEnabled = false; TxtSocietyTest.Text = "🔒 Люди та Суспільство";
            BtnEnvironmentTest.IsEnabled = false; TxtEnvironmentTest.Text = "🔒 Навколишній світ";
            BtnActionsTest.IsEnabled = false; TxtActionsTest.Text = "🔒 Дії та Рух";
            BtnStatesTest.IsEnabled = false; TxtStatesTest.Text = "🔒 Стани та Почуття";
            BtnObjectsTest.IsEnabled = false; TxtObjectsTest.Text = "🔒 Предмети та Побут";
            BtnNmtTest.IsEnabled = false; TxtNmtTest.Text = "🔒 Комплексний тест (НМТ)";

            var data = MainWindow.AppData;
            if (data?.UserData == null || !data.UserData.IsLoggedIn) return;
            if (data.UserData.IsDeveloper) { EnableAll(); return; }

            int userId = data.UserData.Id;
            var ach = data.Achievements;

            if (ach.Any(a => a.UserId == userId && a.LessonId == 1)) { BtnNumbersTest.IsEnabled = true; TxtNumbersTest.Text = "Числа та Математика"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 2)) { BtnPropertiesTest.IsEnabled = true; TxtPropertiesTest.Text = "Кольори та Властивості"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 3)) { BtnPhrasesTest.IsEnabled = true; TxtPhrasesTest.Text = "Базові фрази"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 4)) { BtnTimeTest.IsEnabled = true; TxtTimeTest.Text = "Час та Календар"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 5)) { BtnSocietyTest.IsEnabled = true; TxtSocietyTest.Text = "Люди та Суспільство"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 6)) { BtnEnvironmentTest.IsEnabled = true; TxtEnvironmentTest.Text = "Навколишній світ"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 7)) { BtnActionsTest.IsEnabled = true; TxtActionsTest.Text = "Дії та Рух"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 8)) { BtnStatesTest.IsEnabled = true; TxtStatesTest.Text = "Стани та Почуття"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 9)) { BtnObjectsTest.IsEnabled = true; TxtObjectsTest.Text = "Предмети та Побут"; }

            if (ach.Any(a => a.UserId == userId && a.LessonId == 10)) { BtnNmtTest.IsEnabled = true; TxtNmtTest.Text = "🎓 Комплексний тест (НМТ)"; }
        }

        /// <summary>
        /// Примусово розблоковує всі категорії тестування (використовується для акаунтів розробників).
        /// </summary>
        private void EnableAll()
        {
            BtnNumbersTest.IsEnabled = true; TxtNumbersTest.Text = "Числа та Математика";
            BtnPropertiesTest.IsEnabled = true; TxtPropertiesTest.Text = "Кольори та Властивості";
            BtnPhrasesTest.IsEnabled = true; TxtPhrasesTest.Text = "Базові фрази";
            BtnTimeTest.IsEnabled = true; TxtTimeTest.Text = "Час та Календар";
            BtnSocietyTest.IsEnabled = true; TxtSocietyTest.Text = "Люди та Суспільство";
            BtnEnvironmentTest.IsEnabled = true; TxtEnvironmentTest.Text = "Навколишній світ";
            BtnActionsTest.IsEnabled = true; TxtActionsTest.Text = "Дії та Рух";
            BtnStatesTest.IsEnabled = true; TxtStatesTest.Text = "Стани та Почуття";
            BtnObjectsTest.IsEnabled = true; TxtObjectsTest.Text = "Предмети та Побут";
            BtnNmtTest.IsEnabled = true; TxtNmtTest.Text = "🎓 Комплексний тест (НМТ)";
        }

        /// <summary>
        /// Обробляє клік по кнопці вибору теми, ініціалізує набір питань за допомогою рефлексії з LessonsPage та запускає інтерфейс тесту.
        /// </summary>
        private void SelectTheme_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.Tag == null) return;

            string category = btn.Tag.ToString();
            _categoryId = GetCategoryIdByTag(category);

            string lang = MainWindow.AppData?.AppSettings?.CurrentLanguage ?? "UA";

            if (category == "NMT")
            {
                _availableItems = (lang == "EN")
                    ? wordToVideoMap.Values.Distinct().ToList()
                    : wordToVideoMap.Keys.ToList();

                if (TimerText != null) TimerText.Visibility = Visibility.Visible;
            }
            else
            {
                LessonsPage lessonsPage = new LessonsPage();
                System.Reflection.MethodInfo method = lessonsPage.GetType().GetMethod("GetItemsForCategory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    string[] items = method.Invoke(lessonsPage, new object[] { category, lang }) as string[];
                    if (items != null && items.Length >= 3)
                    {
                        _availableItems = items.ToList();
                    }
                    else
                    {
                        MessageBox.Show("Недостатньо матеріалу для створення тесту!");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Недостатньо матеріалу для створення тесту!");
                    return;
                }

                if (TimerText != null) TimerText.Visibility = Visibility.Collapsed;
            }

            _usedItems.Clear();
            _currentQuestion = 0;
            _correctCount = 0;
            _totalQuestions = _availableItems.Count;

            _testStartTime = DateTime.Now;
            if (TimerText != null) TimerText.Text = "⏱ 00:00";
            _dispatcherTimer.Start();

            TestMenu.Visibility = Visibility.Collapsed;
            TestInterface.Visibility = Visibility.Visible;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ToggleTestMode(true);
            }

            SetNavigationState(false);
            GenerateNextQuestion();
        }

        /// <summary>
        /// Повертає числовий ідентифікатор категорії на основі її рядкового тегу.
        /// </summary>
        /// <param name="tag">Текстовий тег категорії.</param>
        /// <returns>Числовий ідентифікатор категорії.</returns>
        private int GetCategoryIdByTag(string tag)
        {
            switch (tag)
            {
                case "Alphabet": return 1;
                case "Numbers": return 2;
                case "Properties": return 3;
                case "Phrases": return 4;
                case "Time": return 5;
                case "Society": return 6;
                case "Environment": return 7;
                case "Actions": return 8;
                case "States": return 9;
                case "Objects": return 10;
                case "NMT": return 11;
                default: return 0;
            }
        }

        /// <summary>
        /// Генерує наступне питання, підбирає два випадкові неправильні варіанти відповідей та запускає відповідне відео.
        /// </summary>
        private void GenerateNextQuestion()
        {
            if (_currentQuestion >= _totalQuestions || _availableItems.Count == 0)
            {
                EndTest();
                return;
            }

            _currentQuestion++;
            ScoreText.Text = string.Format("{0}/{1}", _currentQuestion, _totalQuestions);

            string correctWord;
            do
            {
                int index = _random.Next(_availableItems.Count);
                correctWord = _availableItems[index];
            } while (_usedItems.Contains(correctWord) && _usedItems.Count < _availableItems.Count);

            _usedItems.Add(correctWord);
            _correctAnswer = correctWord;

            var wrongAnswers = _availableItems.Where(x => x != _correctAnswer).OrderBy(x => _random.Next()).Take(2).ToList();

            while (wrongAnswers.Count < 2) wrongAnswers.Add("---");

            var allAnswers = new List<string> { _correctAnswer, wrongAnswers[0], wrongAnswers[1] };
            allAnswers = allAnswers.OrderBy(x => _random.Next()).ToList();

            Answer1.Content = allAnswers[0];
            Answer2.Content = allAnswers[1];
            Answer3.Content = allAnswers[2];

            ResetButtonColors();

            string videoName = GetVideoFileName(correctWord);
            PlayTestVideo(videoName);
        }

        /// <summary>
        /// Визначає ім'я цільового відеофайлу для заданого слова з урахуванням обраної мови та транслітерації алфавіту.
        /// </summary>
        /// <param name="inputWord">Слово, для якого шукається назва відеофайлу.</param>
        /// <returns>Ім'я відеофайлу або null, якщо відповідність не знайдена.</returns>
        private string GetVideoFileName(string inputWord)
        {
            string cleanWord = inputWord.Trim().ToLower();
            string lang = MainWindow.AppData?.AppSettings?.CurrentLanguage ?? "UA";

            if (lang == "EN") return cleanWord;

            if (wordToVideoMap.TryGetValue(cleanWord, out string englishFileName))
                return englishFileName;

            if (cleanWord.Length == 1)
            {
                char letter = cleanWord[0];
                var letterMap = new Dictionary<char, string>
                {
                    {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "h"}, {'ґ', "g"}, {'д', "d"},
                    {'е', "e"}, {'є', "ye"}, {'ж', "zh"}, {'з', "z"}, {'и', "y"}, {'і', "i"},
                    {'ї', "yi"}, {'й', "j"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
                    {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"}, {'у', "u"},
                    {'ф', "f"}, {'х', "kh"}, {'ц', "ts"}, {'ч', "ch"}, {'ш', "sh"}, {'щ', "shch"},
                    {'ь', "softsign"}, {'ю', "yu"}, {'я', "ya"}
                };
                if (letterMap.TryGetValue(letter, out string englishLetter)) return englishLetter;
            }

            return null;
        }

        /// <summary>
        /// Формує повний шлях до файлу та запускає відтворення тестового відеоролика через MediaElement.
        /// </summary>
        /// <param name="fileName">Ім'я файлу відео без розширення та мовного префіксу.</param>
        private void PlayTestVideo(string fileName)
        {
            try
            {
                string lang = MainWindow.AppData?.AppSettings?.CurrentLanguage?.ToLower() ?? "ua";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Videos", string.Format("{0}_{1}.mp4", lang, fileName));

                if (File.Exists(path))
                {
                    TestVideo.Source = new Uri(path);
                    TestVideo.Play();
                }
            }
            catch { }
        }

        /// <summary>
        /// Обробляє натискання на один з варіантів відповідей, підсвічує результат (зелений/червоний) та враховує правила режиму Hardcore у разі помилки.
        /// </summary>
        private async void Answer_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            AnswersGrid.IsEnabled = false;

            if (clickedButton.Content.ToString() == _correctAnswer)
            {
                clickedButton.Background = new SolidColorBrush(Colors.Green);
                _correctCount++;

                ScoreText.Text = string.Format("{0}/{1}", _currentQuestion, _totalQuestions);
                await Task.Delay(1500);
                AnswersGrid.IsEnabled = true;
                GenerateNextQuestion();
            }
            else
            {
                clickedButton.Background = new SolidColorBrush(Colors.Red);
                HighlightCorrectButton();

                ScoreText.Text = string.Format("{0}/{1}", _currentQuestion, _totalQuestions);

                if (IsHardcoreEnabled())
                {
                    _dispatcherTimer.Stop();
                    await Task.Delay(1500);
                    // Оновлений текст повідомлення для режиму Хардкор
                    MessageBox.Show("Режим ХАРДКОР! Перша ж помилка завершує тест.", "Тест завершено");

                    TestVideo.Stop();
                    BackToMenu_Click(null, null);
                    AnswersGrid.IsEnabled = true;
                    return;
                }

                await Task.Delay(1500);
                AnswersGrid.IsEnabled = true;
                GenerateNextQuestion();
            }
        }

        /// <summary>
        /// Підсвічує кнопку з правильною відповіддю зеленим кольором у разі помилки користувача.
        /// </summary>
        private void HighlightCorrectButton()
        {
            if (Answer1.Content.ToString() == _correctAnswer) Answer1.Background = new SolidColorBrush(Colors.Green);
            if (Answer2.Content.ToString() == _correctAnswer) Answer2.Background = new SolidColorBrush(Colors.Green);
            if (Answer3.Content.ToString() == _correctAnswer) Answer3.Background = new SolidColorBrush(Colors.Green);
        }

        /// <summary>
        /// Скидає фонове забарвлення кнопок відповідей до стандартних значень.
        /// </summary>
        private void ResetButtonColors()
        {
            Answer1.ClearValue(Button.BackgroundProperty);
            Answer2.ClearValue(Button.BackgroundProperty);
            Answer3.ClearValue(Button.BackgroundProperty);
        }

        /// <summary>
        /// Завершує процес тестування, вираховує успішність та нараховує відповідні досягнення в залежності від категорії та умов.
        /// </summary>
        private void EndTest()
        {
            _dispatcherTimer.Stop();
            TestVideo.Stop();

            TimeSpan duration = DateTime.Now - _testStartTime;

            MessageBox.Show(string.Format("Тестування завершено!\nВаш результат: {0} з {1}\nЧас проходження: {2} хв {3} сек", _correctCount, _totalQuestions, duration.Minutes, duration.Seconds), "Результат тесту");

            double successRate = (double)_correctCount / _totalQuestions;
            bool isPassed = successRate >= 0.7;

            if (isPassed)
            {
                var data = MainWindow.AppData;
                if (data?.UserData != null && data.UserData.IsLoggedIn)
                {
                    int userId = data.UserData.Id;
                    bool isHardcore = IsHardcoreEnabled();
                    bool anyNewAchievement = false;

                    if (!data.Achievements.Any(a => a.UserId == userId && a.LessonId == _categoryId))
                    {
                        data.Achievements.Add(new Achievement { UserId = userId, LessonId = _categoryId });
                        anyNewAchievement = true;
                    }

                    if (_categoryId == 1)
                    {
                        if (!data.Achievements.Any(a => a.UserId == userId && a.LessonId == 101))
                        {
                            data.Achievements.Add(new Achievement { UserId = userId, LessonId = 101 });
                            anyNewAchievement = true;
                            MessageBox.Show("🏆 Досягнення розблоковано: Перший крок (Пройдено Алфавіт)!");
                        }
                    }

                    bool allCategoriesDone = true;
                    for (int cat = 1; cat <= 10; cat++)
                    {
                        if (cat == _categoryId) continue;
                        if (!data.Achievements.Any(a => a.UserId == userId && a.LessonId == cat))
                        {
                            allCategoriesDone = false;
                            break;
                        }
                    }
                    if (allCategoriesDone && _categoryId >= 1 && _categoryId <= 10)
                    {
                        if (!data.Achievements.Any(a => a.UserId == userId && a.LessonId == 102))
                        {
                            data.Achievements.Add(new Achievement { UserId = userId, LessonId = 102 });
                            anyNewAchievement = true;
                            MessageBox.Show("🏆 Досягнення розблоковано: Перфекціоніст (Пройдено всі темы окрім НМТ)!");
                        }
                    }

                    if (isHardcore)
                    {
                        if (!data.Achievements.Any(a => a.UserId == userId && a.LessonId == 103))
                        {
                            data.Achievements.Add(new Achievement { UserId = userId, LessonId = 103 });
                            anyNewAchievement = true;
                            MessageBox.Show("🏆 Досягнення розблоковано: Сталеві нерви (Будь-який тест на хардкорі)!");
                        }
                    }

                    if (_categoryId == 11)
                    {
                        if (!data.Achievements.Any(a => a.UserId == userId && a.LessonId == 104))
                        {
                            data.Achievements.Add(new Achievement { UserId = userId, LessonId = 104 });
                            anyNewAchievement = true;
                            MessageBox.Show("🏆 Досягнення розблоковано: Абітурієнт (Пройдено комплексний НМТ)!");
                        }

                        if (isHardcore)
                        {
                            if (!data.Achievements.Any(a => a.UserId == userId && a.LessonId == 105))
                            {
                                data.Achievements.Add(new Achievement { UserId = userId, LessonId = 105 });
                                anyNewAchievement = true;
                                MessageBox.Show("🏆 Досягнення розблоковано: Майстер Екстриму (НМТ на хардкорі без помилок)!");
                            }

                            if (duration.TotalMinutes <= 10)
                            {
                                if (!data.Achievements.Any(a => a.UserId == userId && a.LessonId == 106))
                                {
                                    data.Achievements.Add(new Achievement { UserId = userId, LessonId = 106 });
                                    anyNewAchievement = true;
                                    MessageBox.Show("🏆 Досягнення розблоковано: Блискавичний Фініш (НМТ на хардкорі за <10 хв)!");
                                }
                            }
                        }
                    }

                    if (anyNewAchievement)
                    {
                        MainWindow.SaveAchievements();
                    }
                }
            }

            BackToMenu_Click(null, null);
        }

        /// <summary>
        /// Зупиняє тест і перенаправляє користувача назад на головну сторінку програми.
        /// </summary>
        private void BtnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();
            TestVideo.Stop();
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ToggleTestMode(false);
                mainWindow.MainFrame.Navigate(new HomePage());
            }
        }

        /// <summary>
        /// Перериває активне тестування, повертаючи користувача до внутрішнього меню вибору тем тестування.
        /// </summary>
        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();
            TestVideo.Stop();
            TestInterface.Visibility = Visibility.Collapsed;
            TestMenu.Visibility = Visibility.Visible;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ToggleTestMode(false);
            }

            SetNavigationState(true);
            CheckLocks();
        }

        /// <summary>
        /// Циклічно перезапускає відтворення відео, коли воно доходить до кінця.
        /// </summary>
        private void TestVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            TestVideo.Position = TimeSpan.Zero;
            TestVideo.Play();
        }
    }
}