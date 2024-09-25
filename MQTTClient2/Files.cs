using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MQTTClient2
{
    internal class Files : IFiles
    {
        
        public StringBuilder GetText()
        {
            StringBuilder sb = new StringBuilder(); ;
            if (File.Exists(Configs.File))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(Configs.File))
                    {
                        string line = null;
                        while ((line = sr.ReadLine()) != null)
                        {
                            sb.AppendLine(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log4net.log.Error(ex.Message);
                }
            }
            else
            {
                Log4net.log.Error("Nije pronajden fajl");
            }

            return sb;
        }

        public void WriteText(string text, string imef)
        {
            using (StreamWriter sw = new StreamWriter(imef))
            {
                sw.WriteLine(text);
            }
        }

        public string Change(StringBuilder sb, StringBuilder oldInfo)
        {
            string oldContent = oldInfo.ToString();
            string newContent = sb.ToString();

            int oldLen = oldContent.Length;
            int newLen = newContent.Length;
            int razLen = oldLen - newLen;

            if (razLen < 0)
            {
                return ReadNewLines(sb, oldLen, oldInfo);
            }
            else
            {
                var oldStream = new MemoryStream(Encoding.UTF8.GetBytes(newContent));
                var newStream = new MemoryStream(Encoding.UTF8.GetBytes(oldContent));

                int firstDiference = CompareStreams(oldStream, newStream);
                return ReadNewLines(sb, firstDiference, oldInfo);
            }
        }

        private string ReadNewLines(StringBuilder newContent, int oldLen, StringBuilder oldInfo)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(newContent.ToString())))
            using (var reader = new StreamReader(stream))
            {
                stream.Seek(oldLen, SeekOrigin.Begin);
                string poruka = reader.ReadToEnd();
                oldInfo.Append(poruka);

                return poruka;
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

                //zavrsili su se streamovi, potpuno isti streamovi
                if (byte1 == -1 && byte2 == -1)
                {
                    return -1;
                }

                //jedan je kraci od drugoga, jedan je dosao do kraja
                if (byte1 == -1 || byte2 == -1)
                {
                    return index;
                }

                //prva razlika, nije dosao do kraja
                if (byte1 != byte2)
                {
                    return index;
                }

                index++;
            }
        }
    }
}
