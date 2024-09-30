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

        private static readonly object _lock = new object();

        public DBChanges(MySqlConnection conn, MqttClient client)
        {
            this.conn = conn;
            this.client = client;
            t = new Thread(onChange);
            t.Start();
        }

        public void onChange()
        {
            while (true)
            {
                lock (_lock)
                {

                    string sql = "select * from person where PersonId > @lastId";
                    using (var command = new MySqlCommand(sql, conn))
                    {
                        command.Parameters.AddWithValue("lastId", lastId);
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    int newId = reader.GetInt32(Configs.Col);
                                    string name = reader.GetString(Configs.Col1);
                                    string surname = reader.GetString(Configs.Col2);
                                    int age = reader.GetInt32(Configs.Col3);
                                    string poruka = name + " " + surname + ", " + age;
                                    client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));

                                    lastId = newId;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4net.log.Error(ex.Message);
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}
