using Lieferliste_WPF.Entities;
using System.Collections.Generic;

namespace Lieferliste_WPF.Planning
{
    internal class MachineContainer : List<IMachine>
    {
        private ObservableList<Process> _orderPool = new ObservableList<Process>();

        public int BID { get; set; }

        public ObservableList<Process> OrderPool { get { return _orderPool; } set { _orderPool = value; } }

    }
}
