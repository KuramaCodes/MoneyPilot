using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Data;
namespace MoneyPilot
{
    public partial class Add_User : Window
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string Database = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Pfad + "\\Data\\MoneyPilot_Database.accdb;Persist Security Info=False";
        public OleDbConnection con = new OleDbConnection(Database);
        public OleDbCommand cmd = new OleDbCommand();
        public Encrypt encrypt = new Encrypt();
        public Add_User()
        {
            InitializeComponent();
        }
        private void Add_New_User_Click(object sender, RoutedEventArgs e)
        {
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO Benutzer (Benutzername, Passwort) VALUES ('" + Username.Text + "', '" + encrypt.EncryptToBase64(Password.Password) + "')";
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            Directory.CreateDirectory(Pfad + "//Graphs//" + Username.Text);
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
