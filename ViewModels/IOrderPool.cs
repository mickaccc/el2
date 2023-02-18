using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Planning
{
    interface IOrderPool
    {
        List<String> Orders { get; set; }
    }
}
