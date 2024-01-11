using System.Windows.Controls;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.IO;
namespace MoneyPilot
{
    public partial class Adminpanel : UserControl
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string Database = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Pfad + "\\Data\\MoneyPilot_Database.accdb;Persist Security Info=False";
        public OleDbConnection con = new OleDbConnection(Database);
        public OleDbCommand cmd = new OleDbCommand();
        public Adminpanel()
        {
            InitializeComponent();
            LoadData();
        }
        public void LoadData()
        {
            DataTable dt = new DataTable();
            string SQL = "Select * From Benutzer Where Admin <> true";
            OleDbDataAdapter Data = new OleDbDataAdapter(SQL,con);
            Data.Fill(dt);
            Users.DataContext = dt.DefaultView;
        }
        private void AddUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Add_User AddNew = new Add_User();
            AddNew.ShowDialog();
            LoadData();
        }
        private void RemoveUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
