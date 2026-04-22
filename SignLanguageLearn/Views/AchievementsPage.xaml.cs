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
            var user = MainWindow.AppData.UserData;

        }
    }
}