using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lieferliste_WPF.Planning
{
    interface IMachineFactory
    {
        IMachine createMachine();
    }
}
