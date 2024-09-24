using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTClient2
{
    internal class FileChangesEvent : IFileChanges
    {
        FileSystemWatcher fileSystemWatcher;
        IFiles file = new Files();
        StringBuilder oldInfo;

        public FileChangesEvent()
        {
            fileSystemWatcher = new FileSystemWatcher();
        }

        public void onChange()
        {
            fileSystemWatcher.Path = Path.GetDirectoryName(Configs.File);
            fileSystemWatcher.Filter = Path.GetFileName(Configs.File);
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            oldInfo = file.GetText();

            fileSystemWatcher.Changed += OnChange;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            StringBuilder fileInfo = file.GetText();
            file.Change(fileInfo, oldInfo);
        }
    
    }
}
