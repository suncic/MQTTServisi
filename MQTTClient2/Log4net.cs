using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTClient2
{
    internal static class Log4net
    {
        public static  ILog log { get; }

        static Log4net()
        {
            log = LogManager.GetLogger(typeof(Program));

            var log4netConfigFile = "log4netXmlConfig.config";
            if (File.Exists(log4netConfigFile))
            {
                XmlConfigurator.Configure(new FileInfo(log4netConfigFile));
                log.Info("LogConfig file is found");
            }
            else
            {
                log.Warn($"Configuration file '{log4netConfigFile}' not found.");
                return;
            }
        }
    }
}
