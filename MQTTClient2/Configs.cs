using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTClient2
{
    internal static class Configs
    {

        public static string Broker {  get; } 

        public static int Port {  get; }

        public static  string Topic1 { get; }

        public static string Topic2 { get; }

        public static string Username { get; }

        public static string Password { get; }

        public static string File { get; }

        public static string RootFile { get; }

        static Configs()
        {

            try
            {
                Broker = ConfigurationManager.AppSettings["Mqtt broker"];
                Port = int.Parse(ConfigurationManager.AppSettings["port"]);
                Topic1 = ConfigurationManager.AppSettings["topic1"];
                Topic2 = ConfigurationManager.AppSettings["topic2"];
                Username = ConfigurationManager.AppSettings["username"];
                Password = ConfigurationManager.AppSettings["passwd"];
                Password = ConfigurationManager.AppSettings["passwd"];
                File = ConfigurationManager.AppSettings["file"];
                RootFile = ConfigurationManager.AppSettings["root"];
            }
            catch(FormatException fe)
            {
                log.Error("Number in wrong format: " + fe.StackTrace);
            }
            catch (ConfigurationErrorsException ex)
            {
                Log4net.log.Error("Configuration error: " + ex.Message);
            }
            catch (KeyNotFoundException knfe)
            {
                Log4net.log.Error("Key not found: " + knfe.StackTrace);
            }
        }
        
    }
}
