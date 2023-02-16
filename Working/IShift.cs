using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lieferliste_WPF.Working
{
    public interface IShift
    {
        void buildShift(int start,int end, int type, String comment);


    }
}
