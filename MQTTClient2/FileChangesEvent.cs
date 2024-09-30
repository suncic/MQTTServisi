using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace MQTTClient2
{
    internal class FileChangesEvent : IFileChanges
    {
        private FileSystemWatcher fileSystemWatcher;
        private IFiles file;
        private StringBuilder oldInfo;

        public FileChangesEvent(IFiles f)
        {
            fileSystemWatcher = new FileSystemWatcher();
            this.file = f;
        }

        public void onChange(Action<string> action)
        {
            fileSystemWatcher.Path = Path.GetDirectoryName(Configs.File);
            fileSystemWatcher.Filter = Path.GetFileName(Configs.File);
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            oldInfo = file.GetText();

            fileSystemWatcher.Changed += (sender, e) =>
            {
                StringBuilder fileInfo = file.GetText();
                
                string poruka = file.Change(fileInfo, oldInfo);
                action.Invoke(poruka);
                /*client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(poruka));*/
            };
            fileSystemWatcher.EnableRaisingEvents = true;
        }
    }
}
