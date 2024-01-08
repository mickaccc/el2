﻿using El2Core.Models;
using Prism.Events;
using System.Collections.Generic;

namespace El2Core.Utils
{
    public class MessageVorgangChanged : PubSubEvent<List<string>>
    {
    }
    public class MessageOrderChanged : PubSubEvent<List<string>>
    { }
}
