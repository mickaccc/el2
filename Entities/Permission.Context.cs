﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EntitiesPermiss : DbContext
    {
        public EntitiesPermiss()
            : base("name=EntitiesPermiss")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<PermissionRoles> PermissionRoles { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<tblBerechtigung> tblBerechtigung { get; set; }
        public virtual DbSet<tblUserListe> tblUserListe { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
    }
}
