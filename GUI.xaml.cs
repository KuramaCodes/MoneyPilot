using MySqlConnector;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
namespace MoneyPilot
{
    public partial class GUI : UserControl
    {
        public static string Pfad = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly Regex _regex = new Regex("[0-9,.]");
        public User Benutzer = new User();
        public Connection Connect = new Connection();
        public MySqlCommand cmd = new MySqlCommand();
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
            if (Connect.isConnected)
            {
                Connect.OpenConnection();
                cmd.Connection = Connect.con;
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT * FROM Einnahmen WHERE BenutzerID = @ID;";
                cmd.Parameters.AddWithValue("@ID", Benutzer.Id);

                MySqlDataAdapter Data = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                Data.Fill(dt);

                Connect.CloseConnection();

                Benutzer.Einnahmen.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Benutzer.Einnahmen.Add(Convert.ToDouble(dt.Rows[i][2]));
                }

                Einkommen_Graph.Plot.Clear();
                var Plot = new Plot(256, 256);
                double[] values = Benutzer.Einnahmen.ToArray();
                var pie = Plot.AddPie(values, true);
                pie.Explode = true;
                pie.ShowValues = true;

                Einkommen_Graph.Plot.Add(pie);
                Einkommen_Graph.Refresh();
            }
        }
        void WriteIncome()
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = Connect.con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO Einnahmen ([Benutzer-ID], Betrag, Einnahmequelle) Values (@1, @2, @3)";
            cmd.Parameters.AddWithValue("@1", Benutzer.Id);
            cmd.Parameters.AddWithValue("@2", Einkommen_Wert.Text.Replace(",", "."));
            cmd.Parameters.AddWithValue("@3", Einkommen_Kategorie.Text);

            Connect.OpenConnection();
            cmd.ExecuteNonQuery();
            Connect.CloseConnection();
        }
        #endregion
        #region Expenses
        public void LoadData_Expenses()
        {
            Connect.OpenConnection();
            cmd.Connection = Connect.con;
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT * FROM Ausgaben WHERE (BenutzerID = @ID);";
            cmd.Parameters.AddWithValue("@ID", Benutzer.Id);
            MySqlDataAdapter Data = new MySqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            Data.Fill(dt);
            Connect.CloseConnection();

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
        void WriteExpenses()
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = Connect.con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO Ausgaben (BenutzerID, Betrag, Kategorie) Values (" + Benutzer.Id + ", " + Ausgaben_Wert.Text.Replace(",", ".") + ", '" + Ausgaben_Kategorie.Text + "')";
            cmd.Parameters.AddWithValue("@1", Benutzer.Id);
            cmd.Parameters.AddWithValue("@2", Ausgaben_Wert.Text.Replace(",", "."));
            cmd.Parameters.AddWithValue("@3", Ausgaben_Kategorie.Text);

            Connect.OpenConnection();
            if (Connect.isConnected)
            {
                cmd.ExecuteNonQuery();
            }
            Connect.CloseConnection();
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
                    Money *= Interest + 1;
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
                    Money *= Interest + 1;
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
        public void LoadData_Finances()
        {
            double Einnahmen = 0;
            double Ausgaben = 0;
            double Rest;

            foreach (double Einnahme in Benutzer.Einnahmen)
            {
                Einnahmen += Einnahme;
            }
            foreach (double Ausgabe in Benutzer.Ausgaben)
            {
                Ausgaben += Ausgabe;
            }
            Rest = Einnahmen - Ausgaben;
            _Income.Content = "Income: " + Einnahmen + "€";
            _Expenses.Content = "- Expenses: " + Ausgaben + "€";
            _Remaining.Content = "= Remaining: " + Rest + "€";

            DrawFinanceGraph(Ausgaben, Rest);
        }
        void DrawFinanceGraph(double Ausgaben, double Rest)
        {
            var Plot = new Plot(480, 316);

            if (Rest < 0)
            {
                Rest = 0;
            }
            double[] values = { Ausgaben, Rest };
            string[] labels = { "Expenses", "Remaining" };
            var pie = Plot.AddPie(values);
            pie.SliceLabels = labels;
            pie.Explode = true;
            pie.ShowPercentages = true;
            Plot.Legend();

            try
            {
                File.Delete(Pfad + "//Graphs//" + Benutzer.Username + "//Finances.PNG");
                Plot.SaveFig(Pfad + "//Graphs//" + Benutzer.Username + "//Finances.PNG");
                BitmapImage Finances = new BitmapImage();
                Finances.BeginInit();
                Finances.CacheOption = BitmapCacheOption.OnLoad;
                Finances.UriSource = new Uri(Pfad + "//Graphs//" + Benutzer.Username + "//Finances.PNG");
                Finances.EndInit();
                Overview.Source = Finances;
            }
            catch { }
        }
        #endregion
    }
}

