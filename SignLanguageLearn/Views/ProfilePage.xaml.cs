using System;
using System.Windows;
using System.Windows.Controls;
using SignLanguageLearn.Services;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Views
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();

            // Важливо: переконайся, що DatabaseService має метод Initialize()
            try { DatabaseService.Initialize(); } catch { }

            var data = DataManager.LoadData();

            if (data?.UserData != null && data.UserData.IsLoggedIn)
            {
                ShowProfile(data.UserData.UserName);
            }
        }

        // Події натискання кнопок мають бути public або private, 
        // але назви мають ТОЧНО збігатися з XAML
        public void LoginClick(object sender, RoutedEventArgs e)
        {
            string login = LoginTxt.Text;
            string password = PassBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Будь ласка, введіть логін та пароль!");
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
                MessageBox.Show("Невірний логін або пароль.");
            }
        }

        public void RegisterClick(object sender, RoutedEventArgs e)
        {
            string login = LoginTxt.Text;
            string password = PassBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заповніть поля для реєстрації!");
                return;
            }

            if (DatabaseService.Register(login, password))
            {
                MessageBox.Show("Реєстрація успішна! Тепер увійдіть.");
            }
            else
            {
                MessageBox.Show("Користувач з таким логіном вже існує.");
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