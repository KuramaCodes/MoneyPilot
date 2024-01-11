using System.Windows;
using System.Windows.Controls;
namespace MoneyPilot
{
    public partial class Login : UserControl
    {
        public event RoutedEventHandler ButtonClick;
        public Login()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }
    }
}
