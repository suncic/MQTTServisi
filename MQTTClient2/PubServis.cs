using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using MySql.Data.MySqlClient;
using System.Runtime.Remoting.Messaging;

namespace MQTTClient2
{
    internal class PubServis : IPubService
    {
        private MqttClient client;
        private Thread t1 = null;
        private IFiles file;
        private IFileChanges fileChanges;
        private MySqlConnection conn;
        private DBChanges dbChanges;
        private MySqlDataReader reader;

        private static StringBuilder oldInfo = new StringBuilder();

        public PubServis(MqttClient client, IFiles f, MySqlConnection con, MySqlDataReader reader)
        {
            this.client = client;
            this.file = f;
            this.conn = con;
            this.reader = reader;
            this.fileChanges = new FileChangesManual(file);
            //this.fileChanges = new FileChangesEvent(file);
            dbChanges = new DBChanges(con, client);
            this.t1 = new Thread(Publish);
            t1.Start();
        }

        public void Publish()
        {
            /*StringBuilder sb = file.GetText();
            client.Publish(Configs.Topic1, Encoding.UTF8.GetBytes(sb.ToString()));
            Thread.Sleep(1000);*/

            //fileChanges.onChange(client);
            dbChanges.onChange(client, reader);
        }
    }
}
