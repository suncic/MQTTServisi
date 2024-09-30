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
        private MySqlConnection conn;

        public PersonSubscribe(MySqlConnection conn)
        {
            this.conn = conn;
        }

        public int AddInDatabase(String poruka)
        {
            if (Regex.IsMatch(poruka, @Configs.Pattern))
            {
                string[] tokens = poruka.Split(' ');
                string sql = "insert into person(PersonName, PersonSurname, PersonAge) value('" + tokens[0] + "','" + tokens[1] + "'," + int.Parse(tokens[2]) + ")";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();

                return i;
            }
            else
            {
                Log4net.log.Info("Primljena poruka nije odgovarajuceg formata");
            }

            return -1;
        }
    }
}
