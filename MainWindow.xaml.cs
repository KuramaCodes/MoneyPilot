using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Windows;
namespace MoneyPilot
{
    public partial class MainWindow : Window
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string Database = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Pfad + "\\Data\\MoneyPilot_Database.accdb;Persist Security Info=False";
        public string ID = "", Username = "", Password = "";
        public bool NewUser = false, Admin = false;
        public Login login = new Login();
        public OleDbConnection con = new OleDbConnection(Database);
        public OleDbCommand cmd = new OleDbCommand();
        public Decrypt decrypt = new Decrypt();
        public MainWindow()
        {
            InitializeComponent();
            login.ButtonClick += Login_ButtonClick;
            cmd.Connection = con;
            Control.Content = login;
        }
        private void Login_ButtonClick(object sender, RoutedEventArgs e)
        {
            cmd.CommandType = CommandType.Text;

            ReadData();
            if (login.Username.Text == Username && login.Password.Password == decrypt.DecryptFromBase64(Password) && Admin == true)
            {
                AdminLogin();
                goto Ende;
            }
            if (login.Username.Text == Username && login.Password.Password == decrypt.DecryptFromBase64(Password))
            {
                Login();
            }
            else
            {
                MessageBox.Show("Wrong Password or Username", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        Ende:;
        }
        public void Login()
        {
            GUI gui = new GUI();
            gui.Benutzer.Id = Convert.ToInt32(ID);
            gui.Benutzer.Username = Username;
            Control.Content = gui;
            gui.LoadData_Income();
            gui.LoadData_Expenses();
            gui.LoadData_Finances();
        }
        public void AdminLogin()
        {
            Adminpanel admin = new Adminpanel();
            Control.Content = admin;
        }
        public void ReadData()
        {
            cmd.CommandText = "SELECT * FROM Benutzer WHERE Benutzername = '" + login.Username.Text + "'";
            try
            {
                con.Open();
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = reader["Benutzer-ID"].ToString();
                    Username = reader["Benutzername"].ToString();
                    Password = reader["Passwort"].ToString();
                    Admin = Convert.ToBoolean(reader["Admin"]);
                }
                con.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong Password or Username", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
