using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Services
{
    public static class DatabaseService
    {
        private static readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        private static AppStorage _storage;

        // "Сіль" для захисту хешу від простого підбору
        private const string Salt = "SignLanguage_Secret_2026";

        public static void Initialize()
        {
            if (!File.Exists(_filePath))
            {
                _storage = new AppStorage();
                // Для адміна теж одразу хешуємо пароль "123"
                _storage.Users.Add(new User
                {
                    Id = 1,
                    Login = "admin",
                    Password = HashPassword("123")
                });
                Save();
            }
            else
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    _storage = JsonSerializer.Deserialize<AppStorage>(json) ?? new AppStorage();
                }
                catch { _storage = new AppStorage(); }
            }
        }

        // --- НОВИЙ МЕТОД ДЛЯ БЕЗПЕКИ ---
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Змішуємо пароль із сіллю
                string saltedPassword = password + Salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Перетворюємо в HEX-рядок
                }
                return builder.ToString();
            }
        }

        public static bool Login(string login, string password)
        {
            if (_storage == null) Initialize();

            // Хешуємо введений пароль, щоб порівняти його з тим, що в базі
            string hashedPassword = HashPassword(password);
            return _storage.Users.Any(u => u.Login == login && u.Password == hashedPassword);
        }

        public static bool Register(string login, string password)
        {
            if (_storage == null) Initialize();
            if (_storage.Users.Any(u => u.Login == login)) return false;

            _storage.Users.Add(new User
            {
                Id = _storage.Users.Count + 1,
                Login = login,
                // Зберігаємо лише хеш! Чистий пароль ніколи не потрапить у файл
                Password = HashPassword(password)
            });
            Save();
            return true;
        }

        public static void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_storage, options);
            File.WriteAllText(_filePath, json);
        }

        public static void ApplyTheme(string theme) { }
    }

    public class AppStorage
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<SignWord> Lessons { get; set; } = new List<SignWord>();
    }
}