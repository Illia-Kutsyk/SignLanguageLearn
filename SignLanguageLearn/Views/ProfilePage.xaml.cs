using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using SignLanguageLearn.Services;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Сторінка профілю користувача, яка керує процесами авторизації (вхід, реєстрація, вихід) та відображенням даних облікового запису.
    /// </summary>
    public partial class ProfilePage : Page
    {
        /// <summary>
        /// Таймер для автоматичного приховування спливаючих сповіщень.
        /// </summary>
        private DispatcherTimer _timer;

        /// <summary>
        /// Ініціалізує новий екземпляр сторінки профілю, налаштовує таймер сповіщень та завантажує поточний стан користувача.
        /// </summary>
        public ProfilePage()
        {
            InitializeComponent();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(4)
            };

            _timer.Tick += (s, e) =>
            {
                NotificationBox.Visibility = Visibility.Collapsed;
                _timer.Stop();
            };

            try
            {
                DatabaseService.Initialize();
            }
            catch
            {
            }

            var data = DataManager.LoadData();
            if (data.UserData == null)
            {
                data.UserData = new UserData
                {
                    Id = 0,
                    UserName = "Гість",
                    Level = "Початківець",
                    TotalPoints = 0,
                    IsLoggedIn = false
                };
                DataManager.SaveData(data);
            }

            if (data.UserData.IsLoggedIn)
                ShowProfile(data.UserData.UserName);
            else
                ShowGuestState();
        }

        /// <summary>
        /// Відображає інформаційне сповіщення або повідомлення про помилку з автоматичним приховуванням.
        /// </summary>
        /// <param name="message">Текст повідомлення.</param>
        /// <param name="isError">Значення true, якщо це помилка (рамка буде червоною), або false для успішної дії (зелена рамка).</param>
        private void ShowAlert(string message, bool isError = true)
        {
            NotificationText.Text = message;

            NotificationBox.BorderBrush = isError
                ? new SolidColorBrush(Color.FromRgb(231, 76, 60))
                : Brushes.LimeGreen;

            NotificationBox.Visibility = Visibility.Visible;
            _timer.Stop();
            _timer.Start();
        }

        /// <summary>
        /// Обробляє клік по кнопці закриття сповіщення, миттєво приховуючи його.
        /// </summary>
        private void CloseNotification(object sender, RoutedEventArgs e)
        {
            NotificationBox.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Обробляє зміну тексту в полі введення логіну.
        /// </summary>
        private void LoginTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        /// <summary>
        /// Обробляє натискання кнопки входу, перевіряє дані в базі та авторизує користувача.
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

            var user = DatabaseService.Login(login, password);

            if (user != null)
            {
                var data = DataManager.LoadData();

                data.UserData.Id = user.Id;
                data.UserData.UserName = user.Login;
                data.UserData.IsLoggedIn = true;

                string l = user.Login.ToLower();
                data.UserData.IsDeveloper = (l == "admin" || l == "dev");

                DataManager.SaveData(data);
                ShowProfile(user.Login);
            }
            else
            {
                ShowAlert("Невірний логін або пароль.");
            }
        }

        /// <summary>
        /// Обробляє натискання кнопки реєстрації, створюючи новий запис користувача в базі даних.
        /// </summary>
        public void RegisterClick(object sender, RoutedEventArgs e)
        {
            string login = LoginTxt.Text.Trim();
            string password = PassBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowAlert("Заповніть поля для реєстрації!");
                return;
            }

            if (DatabaseService.Register(login, password))
            {
                SetGuestState();
                ShowAlert("Реєстрація успішна! Тепер увійдіть.", false);
            }
            else
            {
                ShowAlert("Користувач з таким логіном вже існує.");
            }
        }

        /// <summary>
        /// Обробляє натискання кнопки виходу, повертаючи додаток у стан гостя та очищуючи поля форми.
        /// </summary>
        public void LogoutClick(object sender, RoutedEventArgs e)
        {
            SetGuestState();

            LoginTxt.Text = "";
            PassBox.Password = "";

            ShowGuestState();
            ShowAlert("Ви вийшли з акаунта.", false);
        }

        /// <summary>
        /// Записує початкові дані гостя у загальну структуру даних додатка та зберігає зміни у локальний файл.
        /// </summary>
        private void SetGuestState()
        {
            var data = DataManager.LoadData();

            data.UserData.Id = 0;
            data.UserData.UserName = "Гість";
            data.UserData.Level = "Початківець";
            data.UserData.TotalPoints = 0;
            data.UserData.IsLoggedIn = false;

            DataManager.SaveData(data);
        }

        /// <summary>
        /// Відображає блок авторизації та приховує блок інформації про профіль.
        /// </summary>
        private void ShowGuestState()
        {
            AuthBlock.Visibility = Visibility.Visible;
            ProfileBlock.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Відображає блок профілю авторизованого користувача та приховує форму входу.
        /// </summary>
        /// <param name="login">Логін користувача для відображення в інтерфейсі.</param>
        private void ShowProfile(string login)
        {
            UserNameLbl.Text = login;
            AuthBlock.Visibility = Visibility.Collapsed;
            ProfileBlock.Visibility = Visibility.Visible;
        }
    }
}