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

namespace MQTTClient2
{
    internal class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static string broker = ConfigurationManager.AppSettings["Mqtt broker"];
        private static int port = int.Parse(ConfigurationManager.AppSettings["port"]);
        private static string username = ConfigurationManager.AppSettings["passwd"];
        private static string password = ConfigurationManager.AppSettings["username"];
        private static MqttClient mqttClient = new MqttClient("localhost", port, false, null,
                null, MqttSslProtocols.TLSv1_2);

        static async Task Main(string[] args)
        {
            var log4netConfigFile = "App1.config";
            if (File.Exists(log4netConfigFile))
            {
                XmlConfigurator.Configure(new FileInfo(log4netConfigFile));
            }
            else
            {
                log.Warn($"Configuration file '{log4netConfigFile}' not found.");
                return;
            }

            await ConnectingToBroker();
        }

        public static async Task ConnectingToBroker()
        {
            while (true)
            {
                try
                {
                    if (!mqttClient.IsConnected)
                    {
                        mqttClient.Connect(Guid.NewGuid().ToString(), username, password);
                        log.Info("Konekcija uspela!");

                        PubServis ps = new PubServis(mqttClient);
                        //SubServis ss = new SubServis(mqttClient, log);
                    }

                    await Task.Delay(5000);
                }
                catch (Exception ex) 
                {
                    log.Error(ex.Message);
                    await Task.Delay(5000);
                }
                
            }
        }
    }
}
