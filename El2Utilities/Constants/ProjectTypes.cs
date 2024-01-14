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
            None,
            [Description("Entwicklungsmuster")]
            DevelopeSpecimen,
            [Description("Verkaufsmuster")]
            SaleSpecimen
        }
    }
}
