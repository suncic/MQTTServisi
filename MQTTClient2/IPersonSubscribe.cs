﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTClient2
{
    internal interface IPersonSubscribe
    {
        int AddInDatabase(string poruka);
    }
}
