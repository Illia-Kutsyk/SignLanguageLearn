using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SignLanguageLearn.Models;

namespace SignLanguageLearn.Services
{
    public class AppSettings
    {
        public string AppName { get; set; } = "SignLanguageLearn";
        public string Version { get; set; } = "1.0.0";
        public string CurrentLanguage { get; set; } = "UA";
        public string CurrentTheme { get; set; } = "Light";
        public string Difficulty { get; set; } = "Normal";
    }

    public class UserData
    {
        public int Id { get; set; } = 0;

        public string UserName { get; set; } = "Гість";

        public string Level { get; set; } = "Початківець";

        public int TotalPoints { get; set; } = 0;

        public bool IsLoggedIn { get; set; } = false;

        public bool IsDeveloper { get; set; } = false;
    }

    public class Section
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int ProgressPercentage { get; set; }
    }

    public class RootData
    {
        public AppSettings AppSettings { get; set; }

        public UserData UserData { get; set; }

        public List<Section> Sections { get; set; }

        public List<Achievement> Achievements { get; set; }
    }

    public static class DataManager
    {
        private static readonly string FolderPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GestoPlatform"
            );

        private static readonly string FilePath =
            Path.Combine(FolderPath, "appdata.json");

        public static RootData SharedData { get; set; }

        public static void SaveData(RootData data)
        {
            try
            {
                if (data == null)
                    return;

                NormalizeData(data);

                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                SharedData = data;

                string jsonString =
                    JsonConvert.SerializeObject(data, Formatting.Indented);

                File.WriteAllText(FilePath, jsonString);
            }
            catch
            {
            }
        }

        public static RootData LoadData()
        {
            if (SharedData != null)
            {
                NormalizeData(SharedData);
                return SharedData;
            }

            if (!File.Exists(FilePath))
            {
                SharedData = CreateDefaultData();
                SaveData(SharedData);
                return SharedData;
            }

            try
            {
                string jsonString = File.ReadAllText(FilePath);

                SharedData =
                    JsonConvert.DeserializeObject<RootData>(jsonString);

                if (SharedData == null)
                    SharedData = CreateDefaultData();

                NormalizeData(SharedData);

                return SharedData;
            }
            catch
            {
                SharedData = CreateDefaultData();
                SaveData(SharedData);
                return SharedData;
            }
        }

        private static RootData CreateDefaultData()
        {
            RootData data = new RootData();

            data.AppSettings = new AppSettings
            {
                AppName = "Gesto",
                Version = "1.0.0",
                CurrentLanguage = "UA",
                CurrentTheme = "Light",
                Difficulty = "Normal"
            };

            data.UserData = new UserData
            {
                Id = 0,
                UserName = "Гість",
                Level = "Початківець",
                TotalPoints = 0,
                IsLoggedIn = false
            };

            data.Sections = new List<Section>
            {
                new Section
                {
                    Id = "Lessons",
                    Title = "Уроки",
                    ProgressPercentage = 0
                },

                new Section
                {
                    Id = "Dictionary",
                    Title = "Словник",
                    ProgressPercentage = 0
                }
            };

            data.Achievements = new List<Achievement>();

            return data;
        }

        private static void NormalizeData(RootData data)
        {
            if (data.AppSettings == null)
            {
                data.AppSettings = new AppSettings();
            }

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
            }

            if (data.Sections == null)
            {
                data.Sections = new List<Section>();
            }

            if (data.Achievements == null)
            {
                data.Achievements = new List<Achievement>();
            }

            // ГОЛОВНЕ ВИПРАВЛЕННЯ
            // Якщо користувач НЕ залогінений —
            // завжди ставимо режим гостя
            if (!data.UserData.IsLoggedIn)
            {
                data.UserData.Id = 0;
                data.UserData.UserName = "Гість";
                data.UserData.Level = "Початківець";
            }
        }
    }
}