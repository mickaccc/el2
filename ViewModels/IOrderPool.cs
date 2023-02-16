using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lieferliste_WPF.Planning
{
    interface IOrderPool
    {
        List<String> Orders { get; set; }
    }
}
