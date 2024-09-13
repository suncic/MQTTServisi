using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MQTTClient2
{
    internal class Files : FilesInterface
    {
        public StringBuilder GetText()
        {
            StringBuilder sb = new StringBuilder(); ;
            if (File.Exists(Configs.File))
            {
                using (StreamReader sr = new StreamReader(Configs.File))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.Append(line.Trim());
                    }
                }
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
    }
}
