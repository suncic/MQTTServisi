using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MQTTClient2
{
    internal class FileChangesManual : IFileChanges
    {
        Thread t;
        IFiles file = new Files();

        public FileChangesManual()
        {
            t = new Thread(onChange); 
            t.Start();
        }

        public void onChange()
        {
            if (File.Exists(Configs.File))
            {
                FileInfo f = new FileInfo(Configs.File);

                DateTime lastModified = f.LastWriteTime;
                while (true)
                {
                    if (lastModified.Equals(DateTime.Now))
                    {
                        //pubolishovati poruku
                    }

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
