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
    
    public partial class tmTransportes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tmTransportes()
        {
            this.tmvDEXEncabezado = new HashSet<tmvDEXEncabezado>();
            this.tmCompradores = new HashSet<tmCompradores>();
            this.tmTerceros = new HashSet<tmTerceros>();
            this.tmvFacturasExportacionEncabezado = new HashSet<tmvFacturasExportacionEncabezado>();
        }
    
        public int intIdTrnsprte { get; set; }
        public string varCdTrnsprte { get; set; }
        public string varDscrpcionTrnsprte { get; set; }
        public Nullable<int> intDiasPsbles { get; set; }
        public string varCmntrios { get; set; }
        public string varDscrpcionWeb { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmvDEXEncabezado> tmvDEXEncabezado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmCompradores> tmCompradores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmTerceros> tmTerceros { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmvFacturasExportacionEncabezado> tmvFacturasExportacionEncabezado { get; set; }
    }
}
