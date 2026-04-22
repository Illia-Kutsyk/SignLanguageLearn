using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SignLanguageLearn.Services
{
    public class AppSettings
    {
        public string AppName { get; set; }
        public string Version { get; set; }
        public string CurrentLanguage { get; set; }
        public string CurrentTheme { get; set; }
    }

    public class UserData
    {
        public string UserName { get; set; }
        public string Level { get; set; }
        public int TotalPoints { get; set; }
        public bool IsLoggedIn { get; set; }
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
    }

    public static class DataManager
    {
        private static readonly string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GestoPlatform");
        private static readonly string FilePath = Path.Combine(FolderPath, "appdata.json");

        public static RootData SharedData { get; set; }

        public static void SaveData(RootData data)
        {
            try
            {
                if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
                SharedData = data;
                string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(FilePath, jsonString);
            }
            catch { }
        }

        public static RootData LoadData()
        {
            if (SharedData != null) return SharedData;

            if (!File.Exists(FilePath)) return SharedData = CreateDefaultData();

            try
            {
                string jsonString = File.ReadAllText(FilePath);
                return SharedData = JsonConvert.DeserializeObject<RootData>(jsonString);
            }
            catch { return SharedData = CreateDefaultData(); }
        }

        private static RootData CreateDefaultData()
        {
            return new RootData
            {
                AppSettings = new AppSettings { AppName = "Gesto", Version = "1.0.0", CurrentLanguage = "UA", CurrentTheme = "Light" },
                UserData = new UserData { UserName = "Студент", Level = "Початківець", TotalPoints = 0, IsLoggedIn = false },
                Sections = new List<Section>
                {
                    new Section { Id = "Lessons", Title = "Уроки", ProgressPercentage = 0 },
                    new Section { Id = "Dictionary", Title = "Словник", ProgressPercentage = 0 }
                }
            };
        }
    }
}