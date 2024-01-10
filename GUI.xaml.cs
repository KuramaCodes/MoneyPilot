using System.IO;
using System.Windows.Controls;
using ScottPlot;
using System.Reflection;
using System;
using System.Windows.Media.Imaging;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Collections.Generic;
namespace MoneyPilot
{
    public partial class GUI : UserControl
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string Database = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Pfad + "\\Data\\MoneyPilot_Database.accdb;Persist Security Info=False";
        private static readonly Regex _regex = new Regex("[0-9,.]");
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
        #region Income
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
        #endregion
        #region Expenses
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
        #endregion
        #region Interest Calcuator
        void CalculateInterest()
        {
            double Money = Convert.ToDouble(_Amount.Text);
            int Runtime = Convert.ToInt32(_Runtime.Text);
            double Interest = Convert.ToDouble(_Rate.Text);
            List<double> X = new List<double>();
            List<double> Y = new List<double>();

            if (Monthly.IsChecked == true)
            {
                for (int i = 0; i < Runtime; i++)
                {
                    X.Add(Money);
                    Y.Add(i);
                    Money *= (Interest + 1);
                    for (int j = 0; j < Runtime * 12; j++)
                    {
                        Money += Convert.ToDouble(_Amount.Text);
                    }
                }
                Zinsgraf.Plot.AddScatter(Y.ToArray(), X.ToArray());
                Zinsgraf.Refresh();
                Zinsgraf.Plot.AxisAuto();
            }
            else if (Monthly.IsChecked == false) 
            {
                for (int i = 0; i < Runtime; i++)
                {
                    X.Add(Money);
                    Y.Add(i);
                    Money *= (Interest + 1);
                }
                Zinsgraf.Plot.AddScatter(Y.ToArray(), X.ToArray());
                Zinsgraf.Refresh();
                Zinsgraf.Plot.AxisAuto();
            }
        }
        private static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (String)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        private void _Rate_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            CalculateInterest();
        }
        #endregion
        #region Financial Overview




        #endregion
    }
}

