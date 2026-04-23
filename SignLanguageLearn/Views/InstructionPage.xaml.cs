using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SignLanguageLearn.Views
{
    public partial class InstructionPage : Page
    {
        public InstructionPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}