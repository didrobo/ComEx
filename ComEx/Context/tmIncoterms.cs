//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComEx.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class tmIncoterms
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tmIncoterms()
        {
            this.tmvFacturasExportacionEncabezado = new HashSet<tmvFacturasExportacionEncabezado>();
            this.tmCompradores = new HashSet<tmCompradores>();
            this.tmProveedores = new HashSet<tmProveedores>();
        }
    
        public int intIdIncterms { get; set; }
        public string varCdIncterms { get; set; }
        public string varDsIncterms { get; set; }
        public string varAbrvtra { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmvFacturasExportacionEncabezado> tmvFacturasExportacionEncabezado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmCompradores> tmCompradores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmProveedores> tmProveedores { get; set; }
    }
}
