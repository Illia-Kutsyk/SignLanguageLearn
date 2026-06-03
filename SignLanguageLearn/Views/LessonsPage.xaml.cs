using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using SignLanguageLearn.Services;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Сторінка уроків, яка відображає категорії, списки слів та відтворює навчальні відео з мови жестів.
    /// </summary>
    public partial class LessonsPage : Page
    {
        /// <summary>
        /// Словник, що зіставляє українські/англійські слова з назвами відповідних відеофайлів.
        /// </summary>
        private Dictionary<string, string> wordToVideoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Поточна обрана категорія уроку (використовується для визначення контексту, наприклад, в алфавіті).
        /// </summary>
        private string currentCategory = "";

        /// <summary>
        /// Статичний словник для транслітерації українських літер у назви відеофайлів.
        /// </summary>
        private static readonly Dictionary<char, string> letterMap = new Dictionary<char, string>
        {
            { 'а', "a" }, { 'б', "b" }, { 'в', "v" }, { 'г', "h" }, { 'ґ', "g" }, { 'д', "d" },
            { 'е', "e" }, { 'є', "ye" }, { 'ж', "zh" }, { 'з', "z" }, { 'и', "y" }, { 'і', "i" },
            { 'ї', "yi" }, { 'й', "j" }, { 'к', "k" }, { 'л', "l" }, { 'м', "m" }, { 'н', "n" },
            { 'о', "o" }, { 'п', "p" }, { 'р', "r" }, { 'с', "s" }, { 'т', "t" }, { 'у', "u" },
            { 'ф', "f" }, { 'х', "kh" }, { 'ц', "ts" }, { 'ч', "ch" }, { 'ш', "sh" }, { 'щ', "shch" },
            { 'ь', "softsign" }, { 'ю', "yu" }, { 'я', "ya" }
        };

        /// <summary>
        /// Ініціалізує новий екземпляр сторінки уроків.
        /// </summary>
        public LessonsPage()
        {
            InitializeComponent();
            LoadWordMap();
            this.Loaded += (s, e) => CheckLocks();
        }

        /// <summary>
        /// Завантажує словник зіставлення слів та відео з конфігураційного JSON-файлу.
        /// </summary>
        private void LoadWordMap()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "wordMap.json");
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    var loadedMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    if (loadedMap != null)
                    {
                        wordToVideoMap = new Dictionary<string, string>(loadedMap, StringComparer.OrdinalIgnoreCase);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження словника: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Перевіряє досягнення користувача та блокує або розблоковує доступ до категорій уроків.
        /// </summary>
        private void CheckLocks()
        {
            BtnNumbers.IsEnabled = false; TxtNumbers.Text = "🔒 Числа та Математика";
            BtnProperties.IsEnabled = false; TxtProperties.Text = "🔒 Кольори та Властивості";
            BtnPhrases.IsEnabled = false; TxtPhrases.Text = "🔒 Базові фрази";
            BtnTime.IsEnabled = false; TxtTime.Text = "🔒 Час та Календар";
            BtnSociety.IsEnabled = false; TxtSociety.Text = "🔒 Люди та Суспільство";
            BtnEnvironment.IsEnabled = false; TxtEnvironment.Text = "🔒 Навколишній світ";
            BtnActions.IsEnabled = false; TxtActions.Text = "🔒 Дії та Рух";
            BtnStates.IsEnabled = false; TxtStates.Text = "🔒 Стани та Почуття";
            BtnObjects.IsEnabled = false; TxtObjects.Text = "🔒 Предмети та Побут";

            var data = MainWindow.AppData;

            if (data == null || data.UserData == null || !data.UserData.IsLoggedIn) return;

            if (data.UserData.IsDeveloper) { EnableAll(); return; }

            int userId = data.UserData.Id;
            var ach = data.Achievements;

            if (ach.Any(a => a.UserId == userId && a.LessonId == 1)) { BtnNumbers.IsEnabled = true; TxtNumbers.Text = "Числа та Математика"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 2)) { BtnProperties.IsEnabled = true; TxtProperties.Text = "Кольори та Властивості"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 3)) { BtnPhrases.IsEnabled = true; TxtPhrases.Text = "Базові фрази"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 4)) { BtnTime.IsEnabled = true; TxtTime.Text = "Час та Календар"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 5)) { BtnSociety.IsEnabled = true; TxtSociety.Text = "Люди та Суспільство"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 6)) { BtnEnvironment.IsEnabled = true; TxtEnvironment.Text = "Навколишній світ"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 7)) { BtnActions.IsEnabled = true; TxtActions.Text = "Дії та Рух"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 8)) { BtnStates.IsEnabled = true; TxtStates.Text = "Стани та Почуття"; }
            if (ach.Any(a => a.UserId == userId && a.LessonId == 9)) { BtnObjects.IsEnabled = true; TxtObjects.Text = "Предмети та Побут"; }
        }

        /// <summary>
        /// Розблоковує всі категорії уроків (використовується для режиму розробника).
        /// </summary>
        private void EnableAll()
        {
            BtnNumbers.IsEnabled = true; TxtNumbers.Text = "Числа та Математика";
            BtnProperties.IsEnabled = true; TxtProperties.Text = "Кольори та Властивості";
            BtnPhrases.IsEnabled = true; TxtPhrases.Text = "Базові фрази";
            BtnTime.IsEnabled = true; TxtTime.Text = "Час та Календар";
            BtnSociety.IsEnabled = true; TxtSociety.Text = "Люди та Суспільство";
            BtnEnvironment.IsEnabled = true; TxtEnvironment.Text = "Навколишній світ";
            BtnActions.IsEnabled = true; TxtActions.Text = "Дії та Рух";
            BtnStates.IsEnabled = true; TxtStates.Text = "Стани та Почуття";
            BtnObjects.IsEnabled = true; TxtObjects.Text = "Предмети та Побут";
        }

        /// <summary>
        /// Обробляє клік по категорії уроку та формує список слів для вивчення.
        /// </summary>
        private void Category_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || btn.Tag == null) return;

            CategoryMenu.Visibility = Visibility.Collapsed;
            LessonContent.Visibility = Visibility.Visible;
            ItemsList.Items.Clear();

            currentCategory = btn.Tag.ToString();

            string lang = MainWindow.AppData?.AppSettings?.CurrentLanguage ?? "UA";
            string[] items = GetItemsForCategory(currentCategory, lang);

            foreach (var i in items) ItemsList.Items.Add(new ListBoxItem { Content = i });
        }

        /// <summary>
        /// Повертає масив слів для вказаної категорії відповідно до обраної мови.
        /// </summary>
        private string[] GetItemsForCategory(string category, string lang)
        {
            bool isEn = lang == "EN";

            switch (category)
            {
                case "Alphabet":
                    return isEn ? new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" }
                                : new[] { "А", "Б", "В", "Г", "Ґ", "Д", "Е", "Є", "Ж", "З", "И", "І", "Ї", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ь", "Ю", "Я" };
                case "Numbers":
                    return isEn ? new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "15", "20", "30", "40", "50", "100", "1000", "+", "-" }
                                : new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "15", "20", "30", "40", "50", "100", "1000", "Плюс", "Мінус" };
                case "Properties":
                    return isEn ? new[] { "Red", "Blue", "Green", "Yellow", "Black", "White", "Grey", "Brown", "Purple", "Pink", "Dark", "Big", "Small", "Long", "Short", "Wide", "Narrow", "Hard", "Soft", "Hot", "Cold" }
                                : new[] { "Червоний", "Синій", "Зелений", "Жовтий", "Чорний", "Білий", "Сірий", "Коричневий", "Фіолетовий", "Рожевий", "Темний", "Великий", "Маленький", "Довгий", "Короткий", "Широкий", "Вузький", "Твердий", "М'який", "Гарячий", "Холодний" };
                case "Phrases":
                    return isEn ? new[] { "Hello", "Thanks", "Please", "Sorry", "Yes", "No", "Dont know", "Good", "Bad", "Goodbye", "Good morning", "Good afternoon", "Good evening", "Good night", "Help", "Whats that", "Where", "When", "Why", "Correct", "Wrong" }
                                : new[] { "Привіт", "Дякую", "Будь ласка", "Вибачте", "Так", "Ні", "Не знаю", "Добре", "Погано", "До побачення", "Добрий ранок", "Добрий день", "Добрий вечір", "На добраніч", "Допомога", "Що це", "Де", "Коли", "Чому", "Правильно", "Неправильно" };
                case "Time":
                    return isEn ? new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "Spring", "Summer", "Autumn", "Winter", "Morning", "Day", "Evening", "Night", "Today", "Yesterday", "Tomorrow", "Now" }
                                : new[] { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя", "Січень", "Лютий", "Березень", "Квітень", "Травень", "Червень", "Липень", "Серпень", "Вересень", "Жовтень", "Листопад", "Грудень", "Весна", "Літо", "Осінь", "Зима", "Ранок", "День", "Вечір", "Ніч", "Сьогодні", "Вчора", "Завтра", "Зараз" };
                case "Society":
                    return isEn ? new[] { "Me", "He", "She", "We", "They", "Human", "Job", "Family", "Friend", "Doctor", "Teacher", "Student", "Husband", "Wife", "Male", "Female", "Child", "Boy", "Girl", "Police officer" }
                                : new[] { "Я", "Він", "Вона", "Ми", "Вони", "Люди", "Робота", "Сім'я", "Друг", "Лікар", "Вчитель", "Студент", "Чоловік", "Дружина", "Мужчина", "Жінка", "Дитина", "Хлопець", "Дівчина", "Поліція" };
                case "Environment":
                    return isEn ? new[] { "City", "Village", "Street", "House", "Car", "Bus", "Tree", "Flower", "Sun", "Rain", "Snow", "Wind", "Water", "Earth", "Sky", "Fire", "Mountain", "Forest", "Dog", "Cat" }
                                : new[] { "Місто", "Село", "Вулиця", "Будинок", "Машина", "Автобус", "Дерево", "Квітка", "Сонце", "Дощ", "Сніг", "Вітер", "Вода", "Земля", "Небо", "Вогонь", "Гора", "Ліс", "Собака", "Кіт" };
                case "Actions":
                    return isEn ? new[] { "Go", "Stay", "Sit", "Run", "Do", "Think", "Know", "See", "Hear", "Speak", "Silent", "Pick", "Give", "Helping", "Love", "Working", "Studying", "Play", "Sleep", "Eat", "Drink", "Write", "Read" }
                                : new[] { "Йти", "Стояти", "Сидіти", "Бігти", "Робити", "Думати", "Знати", "Бачити", "Чути", "Говорити", "Брати", "Давати", "Допомагати", "Любити", "Працювати", "Вчитися", "Грати", "Спати", "Їсти", "Пити", "Писати", "Читати" };
                case "States":
                    return isEn ? new[] { "Happy", "Sad", "Angry", "Tired", "Sick", "Healthy", "Hungry", "Busy", "Free", "Brave", "Scared", "Alive", "Dead" }
                                : new[] { "Щасливий", "Сумний", "Злий", "Втомлений", "Хворий", "Здоровий", "Голодний", "Зайнятий", "Вільний", "Сміливий", "Наляканий", "Живий", "Мертвий" };
                case "Objects":
                    return isEn ? new[] { "Telephone", "Computer", "Book", "Table", "Chair", "Door", "Window", "Bed", "Cloth", "Shoes", "Money", "Bag", "Watch", "Glasses", "Key", "Pen" }
                                : new[] { "Телефон", "Комп'ютер", "Книга", "Стіл", "Стілець", "Двері", "Вікно", "Ліжко", "Одяг", "Взуття", "Гроші", "Сумка", "Годинник", "Окуляри", "Ключ", "ручка" };
                default:
                    return new string[] { };
            }
        }

        /// <summary>
        /// Обробляє вибір елемента у списку та запускає відтворення відповідного відео.
        /// </summary>
        private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsList.SelectedItem is ListBoxItem selected && selected.Content != null)
            {
                string fileName = GetVideoFileName(selected.Content.ToString(), currentCategory);
                if (!string.IsNullOrEmpty(fileName))
                {
                    PlayVideo(fileName);
                }
            }
        }

        /// <summary>
        /// Повертає базову назву відеофайлу на основі введеного слова та поточної категорії.
        /// </summary>
        private string GetVideoFileName(string inputWord, string category)
        {
            string cleanWord = inputWord.Trim().ToLower();

            if (category == "Alphabet" && cleanWord.Length == 1)
            {
                if (letterMap.TryGetValue(cleanWord[0], out string englishLetter))
                {
                    return englishLetter;
                }
            }

            if (wordToVideoMap.TryGetValue(cleanWord, out string englishFileName))
            {
                return englishFileName;
            }

            string lang = MainWindow.AppData?.AppSettings?.CurrentLanguage ?? "UA";
            if (lang == "EN")
            {
                return cleanWord;
            }

            if (cleanWord.Length == 1 && letterMap.TryGetValue(cleanWord[0], out string englishLetterFallback))
            {
                return englishLetterFallback;
            }

            return null;
        }

        /// <summary>
        /// Відтворює відео з папки Resources на основі згенерованої назви файлу та обраної мови.
        /// </summary>
        private void PlayVideo(string fileName)
        {
            try
            {
                string lang = MainWindow.AppData?.AppSettings?.CurrentLanguage?.ToLower() ?? "ua";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Videos", $"{lang}_{fileName}.mp4");

                if (File.Exists(path))
                {
                    LessonVideo.Source = new Uri(path);
                    LessonVideo.Play();
                }
            }
            catch { }
        }

        /// <summary>
        /// Повертає користувача до меню вибору категорій, зупиняючи поточне відео.
        /// </summary>
        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            LessonVideo.Stop();
            LessonContent.Visibility = Visibility.Collapsed;
            CategoryMenu.Visibility = Visibility.Visible;
            CheckLocks();
        }

        /// <summary>
        /// Змінює швидкість відтворення відео відповідно до значення повзунка.
        /// </summary>
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { if (LessonVideo != null) LessonVideo.SpeedRatio = e.NewValue; }

        /// <summary>
        /// Відновлює відтворення відео.
        /// </summary>
        private void Play_Click(object sender, RoutedEventArgs e) => LessonVideo.Play();

        /// <summary>
        /// Призупиняє відтворення відео.
        /// </summary>
        private void Pause_Click(object sender, RoutedEventArgs e) => LessonVideo.Pause();

        /// <summary>
        /// Перезапускає відео з початку.
        /// </summary>
        private void Restart_Click(object sender, RoutedEventArgs e) { LessonVideo.Position = TimeSpan.Zero; LessonVideo.Play(); }

        /// <summary>
        /// Обробляє завершення відтворення відео (повертає на початок і зупиняє).
        /// </summary>
        private void LessonVideo_MediaEnded(object sender, RoutedEventArgs e) { LessonVideo.Position = TimeSpan.Zero; LessonVideo.Stop(); }
    }
}