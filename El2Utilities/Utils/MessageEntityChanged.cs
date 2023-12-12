using El2Core.Models;
using Prism.Events;

namespace El2Core.Utils
{
    public class MessageVorgangChanged : PubSubEvent<Vorgang>
    {
    }
    public class MessageOrderChanged : PubSubEvent<OrderRb>
    { }
}
