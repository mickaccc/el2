//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lieferliste_WPF.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblMaterial
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblMaterial()
        {
            this.tblAuftrag = new HashSet<tblAuftrag>();
        }
    
        public string TTNR { get; set; }
        public string Bezeichng { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAuftrag> tblAuftrag { get; set; }
    }
}
