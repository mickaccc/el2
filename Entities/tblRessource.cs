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
    
    public partial class tblRessource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblRessource()
        {
            this.tblArbeitsplatzZuteilung = new HashSet<tblArbeitsplatzZuteilung>();
            this.tblRessourceVorgang = new HashSet<tblRessourceVorgang>();
        }
    
        public int RID { get; set; }
        public string RessName { get; set; }
        public string Abteilung { get; set; }
        public string Info { get; set; }
        public string Inventarnummer { get; set; }
        public Nullable<int> Sort { get; set; }
        public Nullable<int> Type { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArbeitsplatzZuteilung> tblArbeitsplatzZuteilung { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblRessourceVorgang> tblRessourceVorgang { get; set; }
    }
}
