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
        private Files f;

        public PubServis(MqttClient client)
        {
            this.client = client;
            this.t1 = new Thread(Publish);
            t1.Start();

            f = new Files();
        }

        public void Publish()
        {
            StringBuilder sb = f.GetText();

            if (!sb.ToString().Equals(""))
            {
                client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(sb.ToString()));
                f.AddText(sb.ToString());
                Thread.Sleep(1000);
            }

            MonitorFileChanges();
        }

        private void MonitorFileChanges()
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = Path.GetDirectoryName(Configs.File);
            fileSystemWatcher.Filter = Path.GetFileName(Configs.File);
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;

            fileSystemWatcher.Changed += onChange;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void onChange(object sender, FileSystemEventArgs e)
        {
            StringBuilder sb = f.GetText();

            if (!sb.ToString().Equals(""))
            {
                client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(sb.ToString()));
                Thread.Sleep(1000);
            }
            
        }
    }
}
