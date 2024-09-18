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
        static ILog log = LogManager.GetLogger(typeof(Configs));

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
                File = ConfigurationManager.AppSettings["file"];
                RootFile = ConfigurationManager.AppSettings["root"];
            }
            catch (FormatException fe)
            {
                Log4net.log.Error("Number in wrong format: " + fe.StackTrace);
            }
            catch (ConfigurationErrorsException ex)
            {
                Log4net.log.Error("Configuration error: " + ex.Message);
            }
            catch (KeyNotFoundException knfe)
            {
                Log4net.log.Error("Key not found: " + knfe.StackTrace);
            }

            validateConfiguration();
        }
        
       private static bool validateConfiguration()
        {
            var broker = Broker;
            var port = ConfigurationManager.AppSettings["port"];
            var topic1 = Topic1;
            var topic2 = Topic2;
            var username = Username;
            var password = Password;
            var file = File;
            var root = RootFile;

            if (string.IsNullOrEmpty(broker))
            {
                log.Error("broker should not be empty");
                return false;
            }

            if (int.Parse(port, out _))
            {
                log.Error("port should be a valid integer");
                return false;
            }

            if (string.IsNullOrEmpty(topic1))
            {
                log.Error("topic1 should not be empty");
                return false;
            }

            if (string.IsNullOrEmpty(topic2))
            {
                log.Error("topic2 should not be empty");
                return false;
            }

            if (string.IsNullOrEmpty(username))
            {
                log.Error("username should not be empty");
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                log.Error("password should not be empty");
                return false;
            }

            if (string.IsNullOrEmpty(file))
            {
                log.Error("file should not be empty");
                return false;
            }

            if (string.IsNullOrEmpty(root))
            {
                log.Error("root file should not be empty");
                return false;
            }


            return true;
        }
    }
}
