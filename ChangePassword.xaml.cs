using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MoneyPilot
{
    public partial class ChangePassword : Window
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string Database = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Pfad + "\\Data\\MoneyPilot_Database.accdb;Persist Security Info=False";
        public OleDbConnection con = new OleDbConnection(Database);
        public OleDbCommand cmd = new OleDbCommand();
        public Encrypt encrypt = new Encrypt();
        public ChangePassword()
        {
            InitializeComponent();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordOne.Password == NewPasswordTwo.Password)
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Update Benutzer Set Passwort = '" + encrypt.EncryptToBase64(NewPasswordTwo.Password) + "' Where Admin = true";
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
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
