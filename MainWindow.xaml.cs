using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;
using MySqlConnector;
namespace MoneyPilot
{
    public partial class MainWindow : Window
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public string ID = "", Username = "", Password = "";
        public bool NewUser = false, Admin = false;
        public Login login = new Login();
        public Decrypt decrypt = new Decrypt();
        public Connection Database = new Connection();
        public MySqlConnection con = new MySqlConnection();
        public MySqlCommand cmd = new MySqlCommand();
        public MainWindow()
        {
            InitializeComponent();
            login.ButtonClick += Login_ButtonClick;
            cmd.Connection = con;
            Control.Content = login;
        }
        private void OnExit(object sender, EventArgs e)
        {
            con.Close();
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
            cmd.CommandText = "SELECT * FROM Benutzer WHERE Benutzername = @1";
            cmd.Parameters.AddWithValue("@1", login.Username.Text);
            try
            {
                Database.OpenConnection(); //Open Database
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) //Read user Data From Database
                {
                    ID = reader["Benutzer-ID"].ToString();
                    Username = reader["Benutzername"].ToString();
                    Password = reader["Passwort"].ToString();
                    Admin = Convert.ToBoolean(reader["Admin"]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Loading Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Database.CloseConnection(); //Close Database
            }
        }
    }
}
