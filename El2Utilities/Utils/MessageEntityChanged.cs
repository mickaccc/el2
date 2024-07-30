using El2Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;

namespace El2Core.Utils
{
    public class MessageVorgangChanged : PubSubEvent<List<(string, string)?>>
    { }
    public class MessageOrderChanged : PubSubEvent<List<(string, string)?>>
    { }
    public class MessageOrderArchivated : PubSubEvent<OrderRb>
    { }
    public class ContextPlanMachineChanged : PubSubEvent<int>
    { }
    public class SearchTextFilter : PubSubEvent<string> { }
    public class MessagePlanmachineChanged : PubSubEvent<Vorgang> { }
    public class MessageReportFilterWorkAreaChanged : PubSubEvent<(string, bool)> { }
    public class MessageReportFilterDateChanged : PubSubEvent<DateTime> { }

}
