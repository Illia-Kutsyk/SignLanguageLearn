using System.Windows.Controls;

namespace SignLanguageLearn.Views
{
    public partial class ListPage : Page
    {
        public ListPage(string title)
        {
            InitializeComponent();
            TxtTitle.Text = title;
        }
    }
}