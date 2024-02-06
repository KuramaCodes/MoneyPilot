using System.Windows;
using System.Data;
using MySqlConnector;
namespace MoneyPilot
{
    public partial class Add_User : Window
    {
        public MySqlCommand cmd = new MySqlCommand();
        public Encrypt encrypt = new Encrypt();
        public Connection Connect = new Connection();
        public Add_User()
        {
            InitializeComponent();
        }
        private void Add_New_User_Click(object sender, RoutedEventArgs e)
        {
            Connect.OpenConnection();
            cmd.Connection = Connect.con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO Benutzer (Benutzername, Passwort, Admin) VALUES (@1, @2, @3)";
            cmd.Parameters.AddWithValue("@1", Username.Text);
            cmd.Parameters.AddWithValue("@2", encrypt.EncryptToBase64(Password.Password));
            cmd.Parameters.AddWithValue("@3", 0);

            if (Connect.isConnected)
            {
                cmd.ExecuteNonQuery();
            }
            Connect.CloseConnection();
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
