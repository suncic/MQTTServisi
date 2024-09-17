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
    internal class PubServis : PubServiceInterface, IDisposable
    {
        private MqttClient client;
        private Thread t1 = null;
        private Files f;

        private static int lastLenFile = 0;
        bool disposed = false;

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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                if (disposing)
                {
                    if (this != null)
                    {
                        this.Dispose();
                    }


                    if (client != null && client.IsConnected)
                    {
                        client.Disconnect();
                    }
                    client = null;
                }

                disposed = true;
            }
        }
        
        ~PubServis()
        {
            Dispose(false);
        }
    }
}
