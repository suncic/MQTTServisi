﻿using System;
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

namespace MQTTClient2
{
    internal class Program
    {
        private static MqttClient mqttClient = new MqttClient(Configs.Broker, Configs.Port, false, null,
                null, MqttSslProtocols.TLSv1_2);

        static async Task Main(string[] args)
        {
            
                await ConnectingToBroker();
           
        }

        public static async Task ConnectingToBroker()
        {
            try
            {
                
                    mqttClient.ConnectionClosed += async (sender, e) =>
                    {
                        mqttClient.Connect(Guid.NewGuid().ToString(), Configs.Username, Configs.Password);
                        Log4net.log.Info("Connected successfully!");
                    };
                

                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                Log4net.log.Error("Connection failed! " + ex.StackTrace);
                await Task.Delay(5000);
            }
            
        }
    }
}
