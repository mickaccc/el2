using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblHoliMask
    {
        public int Hid { get; set; }
        public string? HoliName { get; set; }
        public DateTime? HoliDate { get; set; }
        public string? HoliDescript { get; set; }
        public string? HoliFormula { get; set; }
    }
}
