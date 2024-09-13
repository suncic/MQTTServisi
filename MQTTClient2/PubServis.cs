using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Configuration;
using System.IO;

namespace MQTTClient2
{
    internal class PubServis : PubServiceInterface
    {
        private MqttClient client;
        private Thread t1 = null;

        public PubServis(MqttClient client)
        {
            this.client = client;
            this.t1 = new Thread(Publish);
            t1.Start();
        }

        public void Publish()
        {
            Files f = new Files();
            StringBuilder sb = f.GetText();
            client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(sb.ToString())); 
            //publishovati bez konvertovanja u bajove
        }

    }
}
