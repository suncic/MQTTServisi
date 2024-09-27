using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using uPLibrary.Networking.M2Mqtt;

namespace MQTTClient2
{
    internal class DBChanges : IDBChanges
    {
        private MySqlConnection conn;
        private Thread t;
        private MqttClient client;
        private int lastId = 0;
        private MySqlDataReader reader;
        public DBChanges(MySqlConnection conn, MqttClient client)
        {
            this.conn = conn;
            t = new Thread(() => onChange(client, reader));
            t.Start();
        }

        public void onChange(MqttClient client, MySqlDataReader reader)
        {
            this.client = client;
            this.reader = reader;
            while (true)
            {
                string sql = "select * from person where PersonId > @lastId";

                using (var command = new MySqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("lastId", lastId);
                    try
                    {
                        reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            int newId = reader.GetInt32("PersonID");
                            string name = reader.GetString("PesonName");
                            string surname = reader.GetString("PersonSurname");
                            int age = reader.GetInt32("PersonCol");
                            string poruka = name + " " + surname + ", " + age;
                            client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));

                            lastId = newId;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4net.log.Error(ex.Message);
                    }
                    finally
                    {
                        if (reader != null && !reader.IsClosed)
                        {
                            reader.Close();
                            reader.Dispose();
                        }
                    }
                    
                }
                Thread.Sleep(5000);
            }
        }
    }
}
