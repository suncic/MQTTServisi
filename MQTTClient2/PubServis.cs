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
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

namespace MQTTClient2
{
    internal class PubServis : IPubService, IFileChanges
    {
        private MqttClient client;
        private Thread t1 = null;
        private IFiles file;

        private static StringBuilder oldInfo = new StringBuilder();

        public PubServis(MqttClient client, IFiles f)
        {
            this.client = client;
            this.file = f;
            this.t1 = new Thread(Publish);
            t1.Start();
        }

        public void Publish()
        {
            StringBuilder sb = file.GetText();
            client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(sb.ToString()));
            Thread.Sleep(1000);

            MonitorFileChanges();
        }

        public void MonitorFileChanges()
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = Path.GetDirectoryName(Configs.File);
            fileSystemWatcher.Filter = Path.GetFileName(Configs.File);
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            oldInfo = file.GetText();

            fileSystemWatcher.Changed += onChange;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void onChange(object sender, FileSystemEventArgs e)
        {
            StringBuilder fileInfo = file.GetText();
            Change(fileInfo);
        }

        private void ReadNewLines(StringBuilder newContent, int oldLen)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(newContent.ToString())))
            using (var reader = new StreamReader(stream))
            {
                stream.Seek(oldLen, SeekOrigin.Begin);
                string poruka = reader.ReadToEnd();
                oldInfo.Append(poruka);
                client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));
                Thread.Sleep(1000);
            }
        }

        private void ReadNewLines2(StringBuilder newContent, int oldLen)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(newContent.ToString())))
            using (var reader = new StreamReader(stream))
            {
                stream.Seek(oldLen, SeekOrigin.Begin);
                string poruka = reader.ReadToEnd();
                oldInfo = newContent;
                oldInfo.Append(poruka);
                client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));
                Thread.Sleep(1000);
            }
        }

    }
}
