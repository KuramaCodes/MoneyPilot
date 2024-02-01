using MySqlConnector;
using System.Windows;
using System.Data;

namespace MoneyPilot
{
    public partial class ChangePassword : Window
    {
        public Connection Connect = new Connection();
        public MySqlCommand cmd = new MySqlCommand();
        public Encrypt encrypt = new Encrypt();
        public ChangePassword()
        {
            InitializeComponent();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordOne.Password == NewPasswordTwo.Password)
            {
                cmd.Connection = Connect.con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Update Benutzer Set Passwort = '" + encrypt.EncryptToBase64(NewPasswordTwo.Password) + "' Where Admin = true";
                Connect.OpenConnection();
                if (Connect.isConnected)
                {
                    cmd.ExecuteNonQuery();
                }
                Connect.CloseConnection();
                this.Close();
            }
            else if (NewPasswordOne.Password != NewPasswordTwo.Password)
            {
                MessageBox.Show("Passwords are not matching", "Error_13", MessageBoxButton.OK ,MessageBoxImage.Error);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
