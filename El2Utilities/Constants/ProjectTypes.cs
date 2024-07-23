using System;
using System.ComponentModel;

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
