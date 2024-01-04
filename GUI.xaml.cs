using System.IO;
using System.Windows.Controls;
using ScottPlot;
using System.Reflection;
using System;
using System.Windows.Media.Imaging;
using System.Data.OleDb;
using System.Data;
namespace MoneyPilot
{
    public partial class GUI : UserControl
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string Database = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Pfad + "\\Data\\MoneyPilot_Database.accdb;Persist Security Info=False";
        public OleDbConnection con = new OleDbConnection(Database);
        public User Benutzer = new User();
        public GUI()
        {
            InitializeComponent();
        }
        private void Refresh_Income_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WriteIncome();
            LoadData_Income();
        }
        private void Refresh_Expenses_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WriteExpenses();
            LoadData_Expenses();
        }
        public void LoadData_Income()
        {
            if (!File.Exists(Pfad + "\\Data\\MoneyPilot_Database.accdb"))
            {
                throw new Exception("Database was moved or deleted");
            }
            else
            {
                string SQL = "SELECT * FROM Einnahmen WHERE (([Benutzer-ID])=" + Benutzer.Id + ");";
                OleDbDataAdapter Data = new OleDbDataAdapter(SQL, Database);
                DataTable dt = new DataTable();
                Data.Fill(dt);
                Benutzer.Einnahmen.Clear();
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    Benutzer.Einnahmen.Add(Convert.ToDouble(dt.Rows[i][2]));
                }
                var Plot = new Plot(256, 256);

                double[] values = Benutzer.Einnahmen.ToArray();
                var pie = Plot.AddPie(values);
                pie.Explode = true;
                pie.ShowValues = true;

                try
                {
                    File.Delete(Pfad + "//Graphs//" + Benutzer.Username + "//Income.PNG");
                    Plot.SaveFig(Pfad + "//Graphs//" + Benutzer.Username + "//Income.PNG");
                    BitmapImage Income = new BitmapImage();
                    Income.BeginInit();
                    Income.CacheOption = BitmapCacheOption.OnLoad;
                    Income.UriSource = new Uri(Pfad + "//Graphs//" + Benutzer.Username + "//Income.PNG");
                    Income.EndInit();
                    Einkommen_Graph.Source = Income;
                }
                catch { }
            }
        }
        public void LoadData_Expenses()
        {
            if (!File.Exists(Pfad + "\\Data\\MoneyPilot_Database.accdb"))
            {
                throw new Exception("Database was moved or deleted");
            }
            else
            {
                string SQL = "SELECT * FROM Ausgaben WHERE (([Benutzer-ID])=" + Benutzer.Id + ");";
                OleDbDataAdapter Data = new OleDbDataAdapter(SQL, Database);
                DataTable dt = new DataTable();
                Data.Fill(dt);
                Benutzer.Ausgaben.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Benutzer.Ausgaben.Add(Convert.ToDouble(dt.Rows[i][2]));
                }
                var Plot = new Plot(256, 256);

                double[] values = Benutzer.Ausgaben.ToArray();
                var pie = Plot.AddPie(values);
                pie.Explode = true;
                pie.ShowValues = true;

                try
                {
                    File.Delete(Pfad + "//Graphs//" + Benutzer.Username + "//Expenses.PNG");
                    Plot.SaveFig(Pfad + "//Graphs//" + Benutzer.Username + "//Expenses.PNG");
                    BitmapImage Income = new BitmapImage();
                    Income.BeginInit();
                    Income.CacheOption = BitmapCacheOption.OnLoad;
                    Income.UriSource = new Uri(Pfad + "//Graphs//" + Benutzer.Username + "//Expenses.PNG");
                    Income.EndInit();
                    Ausgaben_Graph.Source = Income;
                }
                catch { }
            }
        }
        void WriteIncome()
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO Einnahmen ([Benutzer-ID], Betrag, Einnahmequelle) Values (" + Benutzer.Id + ", " + Einkommen_Wert.Text.Replace(",", ".") + ", '" + Einkommen_Kategorie.Text + "')";
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        void WriteExpenses()
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO Ausgaben ([Benutzer-ID], Betrag, Kategorie) Values (" + Benutzer.Id + ", " + Ausgaben_Wert.Text.Replace(",", ".") + ", '" + Ausgaben_Kategorie.Text + "')";
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}

