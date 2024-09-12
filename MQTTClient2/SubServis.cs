﻿using log4net;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTClient2
{ 

    internal class SubServis
    {
        private MqttClient client;
        private ILog logger;
        private DateTime dt;
        private HashSet<string> _messagesId;
        private readonly string rootFolder = @"C:\Users\student\git_demo\MQTTServisi\MQTTClient2\";

        public DateTime Dt
        { 
            get => dt;
            set => dt = value;
        
        }

        public SubServis(MqttClient client, ILog logger) 
        {
            this.client = client;
            this.logger = logger;
            this._messagesId = new HashSet<string>();
            client.Subscribe(new string[] { ConfigurationManager.AppSettings["topic2"] },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            _messagesId.Clear();

            dt = DateTime.UtcNow;
            string tema = e.Topic;
            string poruka = Encoding.UTF8.GetString(e.Message);

            if (tema.Equals(ConfigurationManager.AppSettings["topic2"]) 
                && !_messagesId.Contains(poruka))
            { 
                _messagesId.Add(poruka);

                string imef = rootFolder + dt.ToString("yyyy-MM-dd_HH-mm-ss");
                using (StreamWriter sw = new StreamWriter(imef))
                {
                    sw.WriteLine(poruka);
                }
                
                logger.Info("Objavljena je: " + poruka + " u vreme " + dt.TimeOfDay);
            }
        }
    }
}
