using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.ViewModels
{
    interface IOrderPool
    {
        List<string> Orders { get; set; }
    }
}
