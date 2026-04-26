using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using SignLanguageLearn.Services;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Логіка взаємодії для сторінки профілю (ProfilePage.xaml).
    /// Забезпечує функціонал авторизації, реєстрації користувачів, 
    /// а також відображення інформації про поточний профіль.
    /// </summary>
    public partial class ProfilePage : Page
    {
        /// <summary>
        /// Таймер для автоматичного приховування спливаючих сповіщень.
        /// </summary>
        private DispatcherTimer _timer;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="ProfilePage"/>.
        /// Налаштовує систему сповіщень та перевіряє стан поточної сесії користувача.
        /// </summary>
        public ProfilePage()
        {
            InitializeComponent();

            // Таймер для автоматичного приховування сповіщень через 4 секунди
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            _timer.Tick += (s, e) => { NotificationBox.Visibility = Visibility.Collapsed; _timer.Stop(); };

            // Ініціалізація бази даних (створення файлу JSON, якщо його немає)
            try { DatabaseService.Initialize(); } catch { }

            // Перевірка, чи користувач уже залогінений при відкритті сторінки
            var data = DataManager.LoadData();
            if (data?.UserData != null && data.UserData.IsLoggedIn)
            {
                ShowProfile(data.UserData.UserName);
            }
        }

        /// <summary>
        /// Відображає кастомне сповіщення у верхній частині сторінки.
        /// </summary>
        /// <param name="message">Текст повідомлення для користувача.</param>
        /// <param name="isError">Визначає колір рамки: true — червоний (помилка), false — колір акценту (успіх).</param>
        private void ShowAlert(string message, bool isError = true)
        {
            NotificationText.Text = message;
            NotificationBox.BorderBrush = isError ?
                new SolidColorBrush(Color.FromRgb(231, 76, 60)) :
                (Brush)FindResource("AccentBrush");

            NotificationBox.Visibility = Visibility.Visible;
            _timer.Stop();
            _timer.Start();
        }

        /// <summary>
        /// Обробник події для закриття сповіщення вручну через кнопку-хрестик.
        /// </summary>
        private void CloseNotification(object sender, RoutedEventArgs e)
        {
            NotificationBox.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Обробник зміни тексту у полі логіну. Використовується для валідації вводу в реальному часі.
        /// </summary>
        private void LoginTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            // На даний момент логіка валідації на ходу не потрібна
        }

        /// <summary>
        /// Обробник натискання кнопки "Увійти". 
        /// Виконує перевірку облікових даних через <see cref="DatabaseService"/>.
        /// </summary>
        public void LoginClick(object sender, RoutedEventArgs e)
        {
            string login = LoginTxt.Text;
            string password = PassBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowAlert("Будь ласка, введіть логін та пароль!");
                return;
            }

            if (DatabaseService.Login(login, password))
            {
                var data = DataManager.LoadData();
                data.UserData.IsLoggedIn = true;
                data.UserData.UserName = login;
                DataManager.SaveData(data);
                ShowProfile(login);
            }
            else
            {
                ShowAlert("Невірний логін або пароль.");
            }
        }

        /// <summary>
        /// Обробник натискання кнопки "Реєстрація".
        /// Додає нового користувача до бази даних, якщо логін вільний.
        /// </summary>
        public void RegisterClick(object sender, RoutedEventArgs e)
        {
            string login = LoginTxt.Text;
            string password = PassBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowAlert("Заповніть поля для реєстрації!");
                return;
            }

            if (DatabaseService.Register(login, password))
            {
                ShowAlert("Реєстрація успішна! Тепер увійдіть.", false);
            }
            else
            {
                ShowAlert("Користувач з таким логіном вже існує.");
            }
        }

        /// <summary>
        /// Обробник натискання кнопки "Вийти".
        /// Скидає статус авторизації в налаштуваннях та повертає інтерфейс до форми входу.
        /// </summary>
        public void LogoutClick(object sender, RoutedEventArgs e)
        {
            var data = DataManager.LoadData();
            data.UserData.IsLoggedIn = false;
            data.UserData.UserName = "Студент";
            DataManager.SaveData(data);

            LoginTxt.Text = "";
            PassBox.Password = "";
            ProfileBlock.Visibility = Visibility.Collapsed;
            AuthBlock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Перемикає візуальний стан сторінки на режим відображення профілю залогіненого користувача.
        /// </summary>
        /// <param name="login">Логін користувача для виведення на екран.</param>
        private void ShowProfile(string login)
        {
            UserNameLbl.Text = login;
            AuthBlock.Visibility = Visibility.Collapsed;
            ProfileBlock.Visibility = Visibility.Visible;
        }
    }
}