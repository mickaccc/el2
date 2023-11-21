using System;

namespace Lieferliste_WPF.Interfaces
{
    internal interface ICloseable
    {
        event EventHandler<EventArgs> RequestClose;
    }
}
