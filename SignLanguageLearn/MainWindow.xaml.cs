using System.Windows;
using System.Windows.Controls;
namespace SignLanguageLearn
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                string buttonName = clickedButton.Name;

                switch (buttonName)
                {
                    case "BtnProfile":
                        MessageBox.Show("Ваш профіль: Студент. Рівень навчання: Початківець.", "👤 Профіль");
                        break;
                    case "BtnLessons":
                        MessageBox.Show("Відкриваємо список доступних уроків (15 тем).", "📚 Уроки");
                        break;
                    case "BtnDictionary":
                        MessageBox.Show("Відкриваємо словник жестів: завантажено 500+ слів.", "📖 Словник");
                        break;
                    case "BtnSettings":
                        MessageBox.Show("Меню налаштувань: зміна мови (UA/EN) та теми.", "⚙️ Налаштування");
                        break;
                    case "BtnStats":
                        MessageBox.Show("Ваші нагороди: отримано 3 нових бейджі!", "🏆 Досягнення");
                        break;
                    case "BtnQuiz":
                        MessageBox.Show("Запуск модуля тестування. Ви готові до перевірки знань?", "📝 Тестування");
                        break;
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Бажаєте вийти з програми?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}