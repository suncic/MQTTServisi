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
    internal class PubServis
    {
        private MqttClient client;
        private Thread t1 = null;
        private Thread t2 = null;
        private readonly string file = @"C:\Users\student\git_demo\MQTTServisi\MQTTClient2\publish.txt";

        public PubServis(MqttClient client)
        {
            this.client = client;
            this.t1 = new Thread(Ispis);
            this.t2 = new Thread(Ispis);
            t1.Start();
            t2.Start();
        }

        private StringBuilder ForPublishing()
        {
            StringBuilder sb = new StringBuilder(); ;
            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    int count = 0;
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.Append(line);
                        count++;
                    }
                }
            }

            return sb;
        }

        private void Ispis()
        {   
            StringBuilder sb = ForPublishing();
            int i = 0;
            foreach (string s in sb)
            {
                i = Thread.CurrentThread.ManagedThreadId
                client.Publish(ConfigurationManager.AppSettings["topic1"],
                    Encoding.UTF8.GetBytes(s + " od niti " + i));
                Thread.Sleep(1000);
            }
        }
    }
}
