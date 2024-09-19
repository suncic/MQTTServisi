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
    internal class PubServis : PubServiceInterface, IFileChanges
    {
        private MqttClient client;
        private Thread t1 = null;
        private Files file;

        private static int lastLenFile = 0;
        private static StringBuilder oldInfo = new StringBuilder();

        public PubServis(MqttClient client, FilesInterface f)
        {
            this.client = client;
            this.file =(Files) f;
            file = new Files();
            this.t1 = new Thread(Publish);
            t1.Start();
        }

        public void Publish()
        {
            StringBuilder sb = file.GetText();
            lastLenFile = sb.Length;

            if (!sb.ToString().Equals(""))
            {
                client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(sb.ToString()));
                Thread.Sleep(1000);
            }

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
            int newLen = fileInfo.Length;

            if (newLen > lastLenFile) //dodata nova poruka, ispisi je
            {
                FirstChange(newLen);
            }
            else if (newLen < lastLenFile) //obrisan neki sadrzaj, da li je dodato nesto novo
            {
                SecondChange(newLen, fileInfo);
            }
            else // ista duzina mozda drugaciji sadrzaj
            {

            }
            
        }

        private void SecondChange(int newLen, StringBuilder sb)
        {
            string oldContent = oldInfo.ToString();
            string newContent = sb.ToString();
            using (var stream1 = new MemoryStream(Encoding.UTF8.GetBytes(oldContent)))
            using (var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(newContent)))
            using (var reader1 = new StreamReader(stream1))
            using (var reader2 = new StreamReader(stream2))
            {

            }
        }

        private void FirstChange(int newLen)
        {
            using (var stream = new FileStream(Configs.File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                stream.Seek(lastLenFile + 1, SeekOrigin.Begin);
                string poruka = reader.ReadToEnd();
                lastLenFile = newLen;

                client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));
                Thread.Sleep(1000);
            }
        }
       
    }
}
