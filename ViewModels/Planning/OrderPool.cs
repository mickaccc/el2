using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lieferliste_WPF.Entities;

namespace Lieferliste_WPF.Planning
{
    class OrderPool:IOrderPool
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
