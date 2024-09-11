using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTClient2
{
    //dobijam teme na koje sam subscribe-ovana

    internal class SubServis
    {
        MqttClient client;
        ILog logger;
        private DateTime dt;
        private static HashSet<string> _messagesId = new HashSet<string>();

        public DateTime Dt
        { 
            get => dt;
            set => dt = value;
        
        }

        public SubServis(MqttClient client, ILog logger) 
        {
            this.client = client;
            this.logger = logger;
            client.Subscribe(new string[] { ConfigurationManager.AppSettings["topic2"] },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            dt = DateTime.UtcNow;
            string tema = e.Topic;
            string poruka = Encoding.UTF8.GetString(e.Message);
            if (tema.Equals(ConfigurationManager.AppSettings["topic2"]) 
                && !_messagesId.Contains(poruka))
            {
                _messagesId.Add(poruka);
                logger.Info("Objavljena je: " + poruka + " u vreme " + dt.TimeOfDay);
            }
        }
    }
}
