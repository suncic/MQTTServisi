﻿using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    }
}
