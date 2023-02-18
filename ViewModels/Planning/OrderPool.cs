using Lieferliste_WPF.Entities;
using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Planning
{
    class OrderPool : IOrderPool
    {
        public List<Process> Orders
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        List<string> IOrderPool.Orders
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
