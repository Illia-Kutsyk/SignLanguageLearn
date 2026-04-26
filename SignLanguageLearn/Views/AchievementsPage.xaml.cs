using System;
using System.Windows.Controls;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Логіка взаємодії для сторінки досягнень (AchievementsPage.xaml).
    /// Відображає прогрес навчання користувача, статистику пройдених тестів 
    /// та отримані нагороди.
    /// </summary>
    public partial class AchievementsPage : Page
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="AchievementsPage"/>.
        /// Викликає завантаження статистичних даних користувача при відкритті сторінки.
        /// </summary>
        public AchievementsPage()
        {
            InitializeComponent();
            LoadUserStats();
        }

        /// <summary>
        /// Завантажує актуальні дані про успішність користувача з глобального профілю.
        /// Використовується для оновлення елементів інтерфейсу (прогрес-барів, лічильників).
        /// </summary>
        private void LoadUserStats()
        {
            // Перевірка наявності даних перед завантаженням
            if (MainWindow.AppData?.UserData == null) return;

            var user = MainWindow.AppData.UserData;

            // TODO: Реалізувати прив'язку даних користувача до елементів інтерфейсу
            // Наприклад: 
            // TotalLessonsLbl.Text = user.CompletedLessonsCount.ToString();
            // TestScoreProgress.Value = user.AverageTestScore;
        }
    }
}