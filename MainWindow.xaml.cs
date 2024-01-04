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
        public bool NewUser = false;
        public Login login = new Login();
        public OleDbConnection con = new OleDbConnection(Database);
        public OleDbCommand cmd = new OleDbCommand();
        public MainWindow()
        {
            InitializeComponent();
            login.ButtonClick += Login_ButtonClick;
            login.CheckboxClick += Registration_Click;
            cmd.Connection = con;
            Control.Content = login;
        }
        private void Login_ButtonClick(object sender, RoutedEventArgs e)
        {
            cmd.CommandType = CommandType.Text;
            if (NewUser)
            {
                cmd.CommandText = "INSERT INTO Benutzer (Benutzername, Passwort) VALUES ('" + login.Username.Text + "', '" + EncryptToBase64(login.Password.Password) + "')";
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                login.Registration.Visibility = Visibility.Hidden;
                login.Registration.IsChecked = false;
                ReadData();
                Directory.CreateDirectory(Pfad + "//Graphs//" + Username);
                Login();
            }
            else if (!NewUser)
            {
                if (login.Username.Text == "Kurama" && login.Password.Password == "Auth_Code_2834")
                {
                    login.Registration.Visibility = Visibility.Visible;
                    return;
                }
                ReadData();
                if (login.Username.Text == Username && login.Password.Password == DecryptFromBase64(Password))
                {
                    Login();
                }
                else
                {
                    MessageBox.Show("Wrong Password or Username", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            if (login.Registration.IsChecked == true)
            {
                NewUser = true;
            }
            else
            {
                NewUser = false;
            }
        }
        public void Login()
        {
            GUI gui = new GUI();
            gui.Benutzer.Id = Convert.ToInt32(ID);
            gui.Benutzer.Username = Username;
            Control.Content = gui;
            gui.LoadData_Income();
            gui.LoadData_Expenses();
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
                }
                con.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong Password or Username", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        public string EncryptToBase64(string Input)
        {
            var userBytes = Encoding.UTF8.GetBytes(Input);
            var userHash = MD5.Create().ComputeHash(userBytes);
            SymmetricAlgorithm crypt = Aes.Create();
            crypt.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(passphrase));
            crypt.IV = new byte[16];
            var memorystream = new MemoryStream();
            var cryptostream = new CryptoStream(memorystream, crypt.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(userBytes, 0, userBytes.Length);
            cryptostream.Write(userHash, 0, userHash.Length);
            cryptostream.FlushFinalBlock();
            var resultString = Convert.ToBase64String(memorystream.ToArray());
            return resultString;
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
