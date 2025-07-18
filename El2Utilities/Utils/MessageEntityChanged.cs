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
    public class MessageReportFilterWorkAreaChanged : PubSubEvent<(int, bool)> { }
    public class MessageReportFilterDateChanged : PubSubEvent<List<DateTime>> { }
    public class MessageReportChangeSource : PubSubEvent<int> { }
    public class MessageReportTextSearch : PubSubEvent<string> { }
    public class EnableAutoSave : PubSubEvent<bool> { }

}
