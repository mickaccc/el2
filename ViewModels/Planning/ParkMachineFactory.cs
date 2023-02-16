using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lieferliste_WPF.Planning
{
    class ParkMachineFactory:IMachineFactory
    {
        public IMachine createMachine()
        {
            IMachine m = new ParkMachine();
            return m;
        }
    }
}
