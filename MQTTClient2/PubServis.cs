using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Configuration;

namespace MQTTClient2
{
    //saljem svoje publisovane teme 

    internal class PubServis
    {
        private MqttClient client;
        private Thread t1 = null;
        private Thread t2 = null;

        public PubServis(MqttClient client)
        {
            this.client = client;
            this.t1 = new Thread(Ispis1);
            this.t2 = new Thread(Ispis2);
            t1.Start();
            t2.Start();
        }

        public void Ispis1()
        {
            string poruka = "Objavljena poruka od niti1 broj ";
            int i = 0;
            while (true)
            {
                client.Publish(ConfigurationManager.AppSettings["topic1"], 
                    Encoding.UTF8.GetBytes(poruka + i));
                i++;
                Thread.Sleep(1000);
            }
        }

        public void Ispis2()
        {
            string poruka = "Objavljena poruka od niti2 broj ";
            int i = 0;
            while (true)
            {
                client.Publish(ConfigurationManager.AppSettings["topic1"], 
                    Encoding.UTF8.GetBytes(poruka + i));
                i++;
                Thread.Sleep(1000);
            }
        }

    }
}
