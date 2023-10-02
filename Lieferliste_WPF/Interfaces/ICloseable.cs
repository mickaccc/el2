using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lieferliste_WPF.Interfaces
{
    internal interface ICloseable
    {
        event EventHandler<EventArgs> RequestClose;
    }
}
