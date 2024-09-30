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

        public static string Broker { get; } = "localhost";

        public static int Port { get; } = 1883;

        public static string Topic1 { get; } = "suncica";

        public static string Topic2 { get; } = "suncica";

        public static string Username { get; } = "suncica";

        public static string Password { get; } = "suncica";

        public static string File { get; } = "E:";

        public static string RootFile { get; } = "E:";

        public static string ConnString { get; } = "";

        public static string Col { get; } = ""; 
        
        public static string Col1 { get; } = "";

        public static string Col2 { get; } = "";

        public static string Col3 { get; } = "";

        public static string Pattern { get; } = "";

        public static string FileChangeDetMethod { get; } = "";

        static Configs()
        {
            try
            {
               Broker = ConfigurationManager.AppSettings["Mqtt broker"];
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Port = int.Parse(ConfigurationManager.AppSettings["port"]);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Topic1 = ConfigurationManager.AppSettings["topic1"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Topic2 = ConfigurationManager.AppSettings["topic2"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Username = ConfigurationManager.AppSettings["username"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Password = ConfigurationManager.AppSettings["passwd"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                File = ConfigurationManager.AppSettings["file"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                RootFile = ConfigurationManager.AppSettings["root"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                ConnString = ConfigurationManager.AppSettings["connectionString"];
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Col = ConfigurationManager.AppSettings["col"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Col1 = ConfigurationManager.AppSettings["col1"];
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            } 
            
            try
            {
                Col2 = ConfigurationManager.AppSettings["col2"];
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Col3 = ConfigurationManager.AppSettings["col3"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {
                Pattern = ConfigurationManager.AppSettings["pattern"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            try
            {

                FileChangeDetMethod = ConfigurationManager.AppSettings["FileChangeDetMethod"];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }
    }
}
