using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTClient2
{
    internal interface IFiles
    {
        StringBuilder GetText();
        void WriteText(string text, string imef);
        void Change(StringBuilder sb, StringBuilder oldInfo);
    }
}
