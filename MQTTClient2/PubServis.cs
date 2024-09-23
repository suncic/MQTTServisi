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
            Change(fileInfo);
        }

        private void Change(StringBuilder sb)
        {
            string oldContent = oldInfo.ToString();
            string newContent = sb.ToString();
            using (var stream1 = new MemoryStream(Encoding.UTF8.GetBytes(oldContent)))
            using (var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(newContent)))
            using(var reader = new StreamReader(stream2))
            {
                int firstDiference = CompareStreams(stream1, stream2);
                if (firstDiference == -1)
                {
                    Log4net.log.Info("Sadrzaj je isti, nista ne pablisujemo");
                }
                else
                {
                    //streamovi su razliciti od indexa firstDiference
                    //radim samo nad streamom2 zato sto je to izvrsena promena
                    stream2.Seek(firstDiference, SeekOrigin.Begin);
                    string poruka = reader.ReadToEnd();
                    client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));
                    Thread.Sleep(1000);
                }
            }
        }

        private int CompareStreams(Stream stream1, Stream stream2)
        {
            int index = 0;
            int byte1, byte2;

            while (true)
            {
                byte1 = stream1.ReadByte();
                byte2 = stream2.ReadByte();


                if (byte1 == -1 && byte2 == -1) //zavrsili su se streamovi, potpuno isti streamovi
                {
                    return -1;
                }

                if(byte1 == -1 || byte2 == -1) //jedan je kraci od drugoga, jedan je dosao do kraja
                {
                    return index;
                }

                if (byte1 != byte2) //prva razlika, nije dosao do kraja
                {
                    return index;
                }

                index++;
            }
        }
       
    }
}
