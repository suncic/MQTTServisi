using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using System.Configuration;
using log4net;
using log4net.Config;
using log4net.Appender;
using System.IO;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using static uPLibrary.Networking.M2Mqtt.MqttClient;
using static System.Net.WebRequestMethods;
using System.Threading;
using MySql.Data.MySqlClient;

namespace MQTTClient2
{
    internal class Program
    {
        private static MqttClient mqttClient = new MqttClient(Configs.Broker, Configs.Port, false, null,
                null, MqttSslProtocols.TLSv1_2);
        private static MySqlConnection connection = new MySqlConnection(Configs.ConnString);
        private static IFiles f = new Files();
        private static MySqlDataReader reader = null;
        private static SubServis subServis = new SubServis(mqttClient, connection);


        static void Main(string[] args)
        {
            try
            {
                connection.Open();
                CloseExsistingReader();
                Log4net.log.Info("Connected on database");

                //IPubService pubServis = new PubServis(mqttClient, f, connection, reader);
                ConnectOnBroker();
            }
            catch (MySqlException ex)
            {
                Log4net.log.Error(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private static void ConnectOnBroker()
        {
            while (true)
            {
                try
                {
                    if (!mqttClient.IsConnected)
                    {
                        mqttClient.Connect(Guid.NewGuid().ToString(), Configs.Username, Configs.Password, false, 60);
                        Log4net.log.Info("Connected successfully!");
                        subServis.Subscribe();
                    }
                }
                catch (Exception ex)
                {
                    Log4net.log.Error("Connection failed! " + ex.StackTrace);
                }

                Thread.Sleep(5000);
            }
        }

        private static void CloseExsistingReader()
        {
            if(reader != null && !reader.IsClosed)
            {
                reader.Close();
                reader.Dispose();
            }
        }
    }
}
