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

        private void Change(StringBuilder sb)
        {
            string oldContent = oldInfo.ToString();
            string newContent = sb.ToString();

            int oldLen = oldContent.Length;
            int newLen = newContent.Length;
            int razLen = oldLen - newLen;

            if (razLen < 0)
            {
                ReadNewLines(sb, oldLen);
            }
            else if(razLen >= 0)
            {
                var oldStream = new MemoryStream(Encoding.UTF8.GetBytes(newContent));
                var newStream = new MemoryStream(Encoding.UTF8.GetBytes(oldContent));
                
                int firstDiference = CompareStreams(oldStream, newStream);
                ReadNewLines2(sb, firstDiference);  
            }

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
