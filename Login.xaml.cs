using System.Windows;
using System.Windows.Controls;
namespace MoneyPilot
{
    public partial class Login : UserControl
    {
        public event RoutedEventHandler ButtonClick;
        public event RoutedEventHandler CheckboxClick;
        public Login()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            CheckboxClick?.Invoke(this, e);
        }
    }
}
