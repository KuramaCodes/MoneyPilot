using MySqlConnector;
namespace MoneyPilot
{
    public class Connection
    {
        public MySqlConnection con;
        public bool isConnected = false;
        public void SetupConnection()
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
        }
        public void OpenConnection()
        {
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
