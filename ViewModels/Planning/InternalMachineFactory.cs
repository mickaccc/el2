using Lieferliste_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lieferliste_WPF.Planning
{

    class InternalMachineFactory:IMachineFactory
    {

        public IMachine createMachine()
        {
            IMachine mach = new InternalMachine();
            return mach;
        }
    }
}
