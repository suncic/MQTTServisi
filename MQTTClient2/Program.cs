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

namespace MQTTClient2
{
    internal class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static MqttClient mqttClient;
        public static string username;
        public static string password;

        static async Task Main(string[] args)
        {
            var log4netConfigFile = "App1.config";
            if (File.Exists(log4netConfigFile))
            {
                XmlConfigurator.Configure(new FileInfo(log4netConfigFile));
            }
            else
            {
                Console.WriteLine($"Configuration file '{log4netConfigFile}' not found.");
                return;
            }

            string broker = ConfigurationManager.AppSettings["Mqtt broker"];
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            username = ConfigurationManager.AppSettings["username"];
            password = ConfigurationManager.AppSettings["passwd"];

            mqttClient = new MqttClient("localhost", port, false, null,
                null, MqttSslProtocols.TLSv1_2);

            mqttClient.ConnectionClosed += async (sender, e) =>
            {
                Console.WriteLine("Connection closed.");
                await Konektovanje();
            };

            await Konektovanje();

            Console.ReadLine();
        }

        public static async Task Konektovanje()
        {
            while (true)
            {
                try
                {
                    if (!mqttClient.IsConnected)
                    {
                        mqttClient.Connect(Guid.NewGuid().ToString(), username, password);
                        log.Info("Konekcija uspela");

                        PubServis ps = new PubServis(mqttClient);
                        SubServis ss = new SubServis(mqttClient, log);
                    }

                    await Task.Delay(5000);
                }
                catch (MqttConnectionException ex)
                {
                    log.Error("Failed to connect: " + ex.StackTrace);
                    await Task.Delay(5000);
                }
                catch (MqttCommunicationException ex1)
                {
                    log.Error("Failed to connect: " + ex1.StackTrace);
                    await Task.Delay(5000);
                }
            }
        }
    }
}
