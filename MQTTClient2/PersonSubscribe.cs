using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MQTTClient2
{
    internal class PersonSubscribe : IPersonSubscribe
    {
        //ono sto je objavljeno se salje na dodavanje u bazu
        private MySqlConnection conn;

        public PersonSubscribe(MySqlConnection conn)
        {
            this.conn = conn;
        }

        public int AddInDatabase(String poruka)
        {
            //automatizovati dodavanje id-a
            if (RegexMessage(poruka))
            {
                string[] tokens = poruka.Split(' ');
                string sql = "insert into person(PesonName, PersonSurname, PersonCol) value('" + tokens[0] + "','" + tokens[1] + "'," + int.Parse(tokens[2]) + ")";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();

                return i;
            }

            return -1;
        }

        private bool RegexMessage(string poruka)
        {
            string pattern = @"\w+\s\w+\s\d+";

            bool isMatch = Regex.IsMatch(poruka, pattern);
            return isMatch;
        }
    }
}
