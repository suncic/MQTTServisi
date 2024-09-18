using log4net;
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

    internal class SubServis : SubServiceInterface
    {
        private MqttClient client;
        private DateTime lastMessage;
        bool disposed = false;

        public DateTime LastMessage
        { 
            get => lastMessage; 
            set => lastMessage = value; 
        }

        public SubServis(MqttClient client) 
        {
            this.client = client;
            client.Subscribe(new string[] { Configs.Topic2 }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }


        public void Subscribe()
        {
            client.MqttMsgPublishReceived += (sender, e) =>
            {
                LastMessage = DateTime.UtcNow;
                string tema = e.Topic;
                string poruka = Encoding.UTF8.GetString(e.Message);
                Subscribe(tema, poruka);
            };
        }

        private void Subscribe(string tema, string poruka)
        {
            Files f = new Files();
            if (tema.Equals(Configs.Topic2))
            {
                string imef = Configs.RootFile + LastMessage.ToString("yyyy-MM-dd_HH-mm-ss");
                f.WriteText(poruka, imef);

                Log4net.log.Info("Objavljena je: " + poruka + " u vreme " + LastMessage.TimeOfDay);
            }
        }

       
    }
}
