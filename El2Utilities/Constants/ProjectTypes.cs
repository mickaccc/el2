using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Constants
{
    public class ProjectTypes
    {
        public enum ProjectType
        {
            [Description("ohne")]
            None = 0,
            [Description("Entwicklungsmuster")]
            DevelopeSpecimen = 1,
            [Description("Verkaufsmuster")]
            SaleSpecimen = 2
        }
    }
}
