using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using SignLanguageLearn.Services;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Views
{
    public partial class ProfilePage : Page
    {
        private DispatcherTimer _timer;

        public ProfilePage()
        {
            InitializeComponent();

            // Таймер для автоматичного приховування сповіщень через 4 секунди
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            _timer.Tick += (s, e) => { NotificationBox.Visibility = Visibility.Collapsed; _timer.Stop(); };

            // Ініціалізація бази даних
            try { DatabaseService.Initialize(); } catch { }

            // Перевірка, чи користувач уже залогінений
            var data = DataManager.LoadData();
            if (data?.UserData != null && data.UserData.IsLoggedIn)
            {
                ShowProfile(data.UserData.UserName);
            }
        }

        // Метод для показу красивих сповіщень (червоне - помилка, колір акценту - успіх)
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

        private void CloseNotification(object sender, RoutedEventArgs e)
        {
            NotificationBox.Visibility = Visibility.Collapsed;
        }

        // Обробка введення тексту (якщо потрібно додати валідацію на ходу)
        private void LoginTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Зараз порожньо, щоб не заважати вводу
        }

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

        private void ShowProfile(string login)
        {
            UserNameLbl.Text = login;
            AuthBlock.Visibility = Visibility.Collapsed;
            ProfileBlock.Visibility = Visibility.Visible;
        }
    }
}