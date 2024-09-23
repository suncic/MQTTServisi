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

namespace MQTTClient2
{
    internal class Program
    {
        private static MqttClient mqttClient = new MqttClient(Configs.Broker, Configs.Port, false, null,
                null, MqttSslProtocols.TLSv1_2);
        private static FilesInterface f;
        private static PubServiceInterface pubServis = new PubServis(mqttClient, f);
        private static SubServis subServis = new SubServis(mqttClient);   


        static void Main(string[] args)
        {
            pubServis.Publish();

            while (true)
            {
                try
                {
                    if (!mqttClient.IsConnected) {
                        mqttClient.Connect(Guid.NewGuid().ToString(), Configs.Username, Configs.Password, false, 60);
                        Log4net.log.Info("Connected successfully!");


                        if (!subServis.isSubscribed)
                        {
                            subServis.Subscribe();
                            Log4net.log.Info("Subscribed on topic");
                        }
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
