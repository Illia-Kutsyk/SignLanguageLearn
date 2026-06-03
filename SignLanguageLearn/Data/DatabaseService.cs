using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Services
{
    public static class DatabaseService
    {
        private static readonly string _filePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

        private static AppStorage _storage;

        private const string Salt = "SignLanguage_Secret_2026";

        public static void Initialize()
        {
            if (!File.Exists(_filePath))
            {
                _storage = new AppStorage();

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

                    _storage = JsonSerializer.Deserialize<AppStorage>(json)
                               ?? new AppStorage();
                }
                catch
                {
                    _storage = new AppStorage();
                }
            }
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                string salted = password + Salt;

                byte[] bytes = sha.ComputeHash(
                    Encoding.UTF8.GetBytes(salted));

                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        public static User Login(string login, string password)
        {
            if (_storage == null)
                Initialize();

            string hash = HashPassword(password);

            return _storage.Users.FirstOrDefault
            (
                u => u.Login == login &&
                     u.Password == hash
            );
        }

        public static bool Register(string login, string password)
        {
            if (_storage == null)
                Initialize();

            if (_storage.Users.Any(u => u.Login == login))
                return false;

            int nextId = 1;

            if (_storage.Users.Count > 0)
                nextId = _storage.Users.Max(x => x.Id) + 1;

            _storage.Users.Add(new User
            {
                Id = nextId,
                Login = login,
                Password = HashPassword(password)
            });

            Save();

            return true;
        }

        public static void Save()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(_storage, options);

            File.WriteAllText(_filePath, json);
        }
    }

    public class AppStorage
    {
        public List<User> Users { get; set; } = new List<User>();

        public List<SignWord> Lessons { get; set; } = new List<SignWord>();
    }
}