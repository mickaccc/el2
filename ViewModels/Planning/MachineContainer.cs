using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lieferliste_WPF.Entities;

namespace Lieferliste_WPF.Planning
{
    internal class MachineContainer:List<IMachine>
    {
        private ObservableList<Process> _orderPool = new ObservableList<Process>();
    
        public int BID { get; set; }

        public ObservableList<Process> OrderPool { get { return _orderPool; } set { _orderPool = value; } }
        
    }
}
