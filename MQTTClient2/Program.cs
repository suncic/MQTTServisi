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
        private static MySqlConnection connection;
        private static IFiles f = new Files();
        private static IFileChanges fileChanges;
        private static SubServis subServis = new SubServis(mqttClient, connection);

        static void Main(string[] args)
        {
            FileDBDet();
        }

        private static void FileDBDet()
        {
            switch (Configs.FileDBDet)
            {
                case "FILE":
                    setupFileChanges();
                    break;
                case "DB":
                    setupDBConnection();
                    break;
                default:
                    throw new ArgumentException("Not recognized... ");
            }
        }

        private static void setupDBConnection()
        {
            try
            {
                connection = new MySqlConnection(Configs.ConnString);
                connection.Open();
                Log4net.log.Info("Connected on database");

                IPubService pubServis = new PubServis(mqttClient, f, connection, fileChanges);
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

        private static void setupFileChanges()
        {
            switch (Configs.FileChangeDetMethod)
            {
                case "MANUAL":
                    fileChanges = new FileChangesManual(f);
                    break;
                case "EVENT":
                    fileChanges = new FileChangesEvent(f);
                    break;
                default:
                    throw new ArgumentException("Not recognized... ");
            }

            IPubService pubServis = new PubServis(mqttClient, f, connection, fileChanges);
            ConnectOnBroker();

            fileChanges.onChange(poruka =>
            {
                mqttClient.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));
            });
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
    }
}
