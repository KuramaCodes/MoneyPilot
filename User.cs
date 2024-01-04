using System.Collections.Generic;
namespace MoneyPilot
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public List<double> Einnahmen = new List<double>();
        public List<double> Ausgaben = new List<double>();
    }
}
