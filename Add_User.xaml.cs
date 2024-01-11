using System;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Data;
namespace MoneyPilot
{
    public partial class Add_User : Window
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string Database = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Pfad + "\\Data\\MoneyPilot_Database.accdb;Persist Security Info=False";
        private string passphrase = "Auth_Kurama_2834";
        public OleDbConnection con = new OleDbConnection(Database);
        public OleDbCommand cmd = new OleDbCommand();
        public Add_User()
        {
            InitializeComponent();
        }
        private void Add_New_User_Click(object sender, RoutedEventArgs e)
        {
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO Benutzer (Benutzername, Passwort) VALUES ('" + Username.Text + "', '" + EncryptToBase64(Password.Password) + "')";
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
    }
}
