using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using uPLibrary.Networking.M2Mqtt;

namespace MQTTClient2
{
    internal class FileChangesManual : IFileChanges
    {
        private Thread t;
        private IFiles file;
        private Action<string> action;
        private DateTime lastWriteTime;

        public FileChangesManual(IFiles f)
        {
            this.file = f;
            t = new Thread(StartMonitoring);
            t.Start();
        }

        public void onChange(Action<string> action)
        {
            this.action = action;
        }
        public void StartMonitoring() { 

            if (File.Exists(Configs.File))
            {
                lastWriteTime = File.GetLastWriteTime(Configs.File);
                StringBuilder oldInfo = file.GetText();

                MonitorFile(oldInfo);
            }
        }

        private void MonitorFile(StringBuilder oldInfo)
        {
            while (true)
            {
                try
                {
                    DateTime currentWriteTime = File.GetLastWriteTime(Configs.File);
                    StringBuilder newInfo = file.GetText();
                    string poruka = null;

                    if (currentWriteTime != lastWriteTime)
                    {
                        lastWriteTime = currentWriteTime;
                        poruka = file.Change(newInfo, oldInfo);
                        action.Invoke(poruka);
                    }

                    oldInfo = newInfo;

                    Thread.Sleep(1000);
                }
                catch(Exception e)
                {
                    Log4net.log.Error(e.Message);
                }
            }
        }
    }
}
