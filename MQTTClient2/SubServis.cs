using log4net;
using Microsoft.SqlServer.Server;
using MySql.Data.MySqlClient;
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

    internal class SubServis : ISubService
    {
        private MqttClient client;
        private DateTime lastMessage;
        private MySqlConnection con;
        private PersonSubscribe pSub;

        public bool isSubscribed { get; set; } = false;

        public DateTime LastMessage
        { 
            get => lastMessage; 
            set => lastMessage = value; 
        }

        public SubServis(MqttClient client, MySqlConnection con) 
        {
            this.client = client;
            this.con = con;
            pSub = new PersonSubscribe(con);
        }

        public void Subscribe()
        {
            client.Subscribe(new string[] { Configs.Topic2 }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            isSubscribed = true;
            client.MqttMsgPublishReceived -= Client_MqttMsgPublishReceived;
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            LastMessage = DateTime.UtcNow;
            string tema = e.Topic;
            string poruka = Encoding.UTF8.GetString(e.Message);

            Files f = new Files();
            if (tema.Equals(Configs.Topic2))
            {
                string imef = Configs.RootFile + LastMessage.ToString("yyyy-MM-dd_HH-mm-ss");
                f.WriteText(poruka, imef);
                int i = pSub.AddInDatabase(poruka);

                if (i <= -1)
                {
                    Log4net.log.Warn("Nije dodat ni jedan red");
                }
                else
                {
                    Log4net.log.Info("Dodato je " + i + " redova");
                }

                Log4net.log.Info("Objavljena je: " + poruka + " u vreme " + LastMessage.TimeOfDay);
            }
        }

        public void Unsubscribe()
        {
            client.Unsubscribe(new string[] { Configs.Topic2 });
            isSubscribed = false;
        }
    }
}
