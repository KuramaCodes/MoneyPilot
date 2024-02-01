using MySqlConnector;
namespace MoneyPilot
{
    public class Connection
    {
        public MySqlConnection con;
        public bool isConnected = false;
        public void OpenConnection()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "212.132.70.198",
                Port = 3306,
                UserID = "Admin",
                Password = "KuramaCode",
                Database = "Work",
            };
            con = new MySqlConnection(builder.ConnectionString);
            con.Open();
            isConnected = true;
        }
        public void CloseConnection()
        {
            con.Close();
            isConnected = false;
        }
    }
}
