using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using El2Utilities;

namespace Lieferliste_WPF
{
    public class Configurations : IConfigurations
    {
        public string ConnectionString => Properties.Settings.Default.ConnectionString;
    }
}
