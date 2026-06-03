using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using SignLanguageLearn.Models;
using SignLanguageLearn.Services;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Сторінка відображення досягнень користувача та загального прогресу навчання.
    /// </summary>
    public partial class AchievementsPage : Page
    {
        /// <summary>
        /// Ініціалізує новий екземпляр сторінки досягнень.
        /// </summary>
        public AchievementsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadUserStats();
        }

        /// <summary>
        /// Завантажує свіжу статистику користувача з файлу та оновлює інтерфейс досягнень.
        /// </summary>
        private void LoadUserStats()
        {
            var data = DataManager.LoadData();
            MainWindow.AppData = data;

            if (data?.UserData == null || !data.UserData.IsLoggedIn)
            {
                ResetUI();
                return;
            }

            int currentUserId = data.UserData.Id;
            var allAchievements = data.Achievements ?? new List<Achievement>();

            List<Achievement> userRecords = allAchievements
                .Where(a => a.UserId == currentUserId)
                .ToList();

            bool hasAch101 = userRecords.Any(a => a.LessonId == 101);
            bool hasAch102 = userRecords.Any(a => a.LessonId == 102);
            bool hasAch103 = userRecords.Any(a => a.LessonId == 103);
            bool hasAch104 = userRecords.Any(a => a.LessonId == 104);
            bool hasAch105 = userRecords.Any(a => a.LessonId == 105);
            bool hasAch106 = userRecords.Any(a => a.LessonId == 106);

            Ach101.IsChecked = hasAch101;
            Ach102.IsChecked = hasAch102;
            Ach103.IsChecked = hasAch103;
            Ach104.IsChecked = hasAch104;
            Ach105.IsChecked = hasAch105;
            Ach106.IsChecked = hasAch106;

            int completedCount = 0;
            if (hasAch101) completedCount++;
            if (hasAch102) completedCount++;
            if (hasAch103) completedCount++;
            if (hasAch104) completedCount++;
            if (hasAch105) completedCount++;
            if (hasAch106) completedCount++;

            double progressPercent = (completedCount / 6.0) * 100;

            OverallProgress.Value = progressPercent;
            ProgressText.Text = $"{(int)progressPercent}% (Відкрито досягнень: {completedCount} з 6)";
        }

        /// <summary>
        /// Скидає інтерфейс сторінки до початкового стану (наприклад, коли користувач не авторизований).
        /// </summary>
        private void ResetUI()
        {
            Ach101.IsChecked = false;
            Ach102.IsChecked = false;
            Ach103.IsChecked = false;
            Ach104.IsChecked = false;
            Ach105.IsChecked = false;
            Ach106.IsChecked = false;
            OverallProgress.Value = 0;
            ProgressText.Text = "0%";
        }
    }
}