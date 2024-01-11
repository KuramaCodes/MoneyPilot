using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
namespace MoneyPilot
{
    public partial class MainWindow : Window
    {
        private string passphrase = "Auth_Kurama_2834";
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string Database = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Pfad + "\\Data\\MoneyPilot_Database.accdb;Persist Security Info=False";
        public string ID = "", Username = "", Password = "";
        public bool NewUser = false, Admin = false;
        public Login login = new Login();
        public OleDbConnection con = new OleDbConnection(Database);
        public OleDbCommand cmd = new OleDbCommand();
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
            if (login.Username.Text == Username && login.Password.Password == DecryptFromBase64(Password) && Admin == true)
            {
                AdminLogin();
                goto Ende;
            }
            if (login.Username.Text == Username && login.Password.Password == DecryptFromBase64(Password))
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
        public string DecryptFromBase64(string Input)
        {
            var encryptedBytes = Convert.FromBase64String(Input);
            SymmetricAlgorithm crypt = Aes.Create();
            crypt.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(passphrase));
            crypt.IV = new byte[16];
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            cryptoStream.FlushFinalBlock();
            var allBytes = memoryStream.ToArray();
            var userLen = allBytes.Length - 16;
            if (userLen < 0) throw new Exception("Invalid Len");
            var userHash = new byte[16];
            Array.Copy(allBytes, userLen, userHash, 0, 16);
            var decryptHash = MD5.Create().ComputeHash(allBytes, 0, userLen);
            if (userHash.SequenceEqual(decryptHash) == false) throw new Exception("Invalid Hash");
            var resultString = Encoding.UTF8.GetString(allBytes, 0, userLen);
            return resultString;
        }
    }
}
