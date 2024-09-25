using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace MQTTClient2
{
    internal interface IFileChanges
    {
        void onChange(MqttClient client);
    }
}
