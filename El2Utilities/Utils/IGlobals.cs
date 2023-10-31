using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public interface IGlobals
    {
        static string PC { get; }
        static User User { get; }
    }
}
