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
    using System.ComponentModel.DataAnnotations;

    public partial class tmLineas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tmLineas()
        {
            this.tmRegalias = new HashSet<tmRegalias>();
            this.tmInsumos = new HashSet<tmInsumos>();
            this.tmvProductosFacturacion = new HashSet<tmvProductosFacturacion>();
        }

        public int intIdLnea { get; set; }
        [Display(Name = "C�digo")]
        public string varCdLnea { get; set; }
        [Display(Name = "Descripci�n")]
        public string varDscrpcionLnea { get; set; }
        [Display(Name = "Descripci�n Dian")]
        public string varDscrpcionDian { get; set; }
        [Display(Name = "% Base Regal�as")]
        public Nullable<decimal> numPrcntjeBseRglias { get; set; }
        [Display(Name = "% Rte Fuente")]
        public Nullable<int> intIdRtncionfuente { get; set; }
        [Display(Name = "Factor")]
        public Nullable<decimal> numFctor { get; set; }
        public string varDscrpcionInglesLnea { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmRegalias> tmRegalias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmInsumos> tmInsumos { get; set; }
        public virtual tmRetefuente tmRetefuente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmvProductosFacturacion> tmvProductosFacturacion { get; set; }
    }
}
