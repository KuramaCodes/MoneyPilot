using System.Windows.Controls;
using System.Data;
using System.Collections.Generic;
using System;
using MySqlConnector;
namespace MoneyPilot
{
    public partial class Adminpanel : UserControl
    {
        public Connection Connect = new Connection();
        public MySqlCommand cmd = new MySqlCommand();
        public Adminpanel()
        {
            InitializeComponent();
            LoadData();
        }
        public void LoadData()
        {
            cmd.Parameters.Clear();
            DataTable dt = new DataTable();
            cmd.CommandText = "Select * From Benutzer Where Admin = @1";
            cmd.Parameters.AddWithValue("@1", 0);
            Connect.OpenConnection();
            cmd.Connection = Connect.con;
            MySqlDataAdapter data = new MySqlDataAdapter(cmd);
            data.Fill(dt);
            Connect.CloseConnection();
            Users.DataContext = dt;
        }
        private void AddUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Add_User AddNew = new Add_User();
            AddNew.ShowDialog();
            LoadData();
        }
        private void RemoveUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            List<int> UserID = new List<int>();
            List<string> UserNames = new List<string>();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = Connect.con;
            cmd.CommandType = CommandType.Text;

            foreach (DataRowView row in Users.SelectedItems)
            {
                UserID.Add(Convert.ToInt32(row.Row.ItemArray[0]));
            }
            foreach (int ID in UserID)
            {
                string SQL = "Delete From Benutzer Where [BenutzerID] = " + ID;
                cmd.CommandText = SQL;
                Connect.OpenConnection();
                cmd.ExecuteNonQuery();
                Connect.CloseConnection();
            }
            LoadData();
        }
        private void Change_Password_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangePassword Change = new ChangePassword();
            Change.ShowDialog();
        }
    }
}
