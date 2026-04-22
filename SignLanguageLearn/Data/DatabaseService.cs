using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Services
{
    public static class DatabaseService
    {
        private static readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        private static AppStorage _storage;

        public static void Initialize()
        {
            if (!File.Exists(_filePath))
            {
                _storage = new AppStorage();
                // Тестовий користувач
                _storage.Users.Add(new User { Id = 1, Login = "admin", Password = "123" });
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

        public static bool Login(string login, string password)
        {
            if (_storage == null) Initialize();
            return _storage.Users.Any(u => u.Login == login && u.Password == password);
        }

        public static bool Register(string login, string password)
        {
            if (_storage == null) Initialize();
            if (_storage.Users.Any(u => u.Login == login)) return false;

            _storage.Users.Add(new User
            {
                Id = _storage.Users.Count + 1,
                Login = login,
                Password = password
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

        // Порожній метод для сумісності з твоїм ProfilePage
        public static void ApplyTheme(string theme) { }
    }

    public class AppStorage
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<SignWord> Lessons { get; set; } = new List<SignWord>();
    }
}