using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblRessKappa
    {

        /// <summary>
        /// Identity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Resource ID
        /// </summary>
        public int Rid { get; set; }
        public DateTime Datum { get; set; }

        /// <summary>
        /// Start Schicht 1
        /// </summary>
        public int? Start1 { get; set; }

        /// <summary>
        /// Ende Schicht 1
        /// </summary>
        public int? End1 { get; set; }
        public string Comment1 { get; set; }

        /// <summary>
        /// Start Schicht 2
        /// </summary>
        public int? Start2 { get; set; }

        /// <summary>
        /// Ende Schicht
        /// </summary>
        public int? End2 { get; set; }
        public string Comment2 { get; set; }

        /// <summary>
        /// Start Schicht 3
        /// </summary>
        public int? Start3 { get; set; }

        /// <summary>
        /// Ende Schicht 3
        /// </summary>
        public int? End3 { get; set; }
        public string Comment3 { get; set; }

        /// <summary>
        /// Timestamp to create the Datarow
        /// </summary>
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
