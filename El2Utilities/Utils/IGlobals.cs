using El2Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Utilities.Utils
{
    public interface IGlobals
    {
        static string PC { get; }
        static User User { get; }
    }
}
