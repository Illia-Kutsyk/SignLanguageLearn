using System.Windows.Controls;

namespace SignLanguageLearn.Views
{
    public partial class AchievementsPage : Page
    {
        public AchievementsPage()
        {
            InitializeComponent();
            LoadUserStats();
        }

        private void LoadUserStats()
        {
            // Беремо дані з MainWindow
            var user = MainWindow.AppData.UserData;

            // Наприклад, якщо у тебе є TextBlock з ім'ям txtPoints
            // txtPoints.Text = $"Ваші бали: {user.TotalPoints}";
        }
    }
}