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
using System.Web.Hosting;

namespace MQTTClient2
{
    internal class PubServis : PubServiceInterface
    {
        private MqttClient client;
        private Thread t1 = null;
        private FilesInterface f;

        private static int lastLenFile = 0;
        bool disposed = false;

        public PubServis(MqttClient client, FilesInterface f)
        {
            this.client = client;
            this.f = f;
            f = new Files();
            this.t1 = new Thread(Publish);
            t1.Start();
        }

        public void Publish()
        {
            StringBuilder sb = f.GetText();
            lastLenFile = sb.Length;

            if (!sb.ToString().Equals(""))
            {
                client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(sb.ToString()));
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
            StringBuilder fileInfo = f.GetText();

            if (fileInfo.Length > lastLenFile)
            {
                using(var stream = new FileStream(Configs.File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using(var reader = new StreamReader(stream))
                {
                    stream.Seek(lastLenFile + 1, SeekOrigin.Begin);
                    string poruka = reader.ReadToEnd();
                    lastLenFile = fileInfo.Length;

                    client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));
                    Thread.Sleep(1000);
                }
            }
            
        }
       
    }
}
