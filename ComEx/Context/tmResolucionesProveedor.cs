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
    
    public partial class tmResolucionesProveedor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tmResolucionesProveedor()
        {
            this.tmCompras = new HashSet<tmCompras>();
        }
    
        public int intIdRslcion { get; set; }
        public Nullable<int> intIdPrvdor { get; set; }
        public string NmroRslcion { get; set; }
        public string NmroRslcionDsde { get; set; }
        public string NmroRslcionHsta { get; set; }
        public Nullable<System.DateTime> FecVncmientoRslcion { get; set; }
        public string varPrfjo { get; set; }
        public Nullable<bool> bitActva { get; set; }
        public Nullable<System.DateTime> fecRslcion { get; set; }
        public Nullable<int> intVgnciaMses { get; set; }
    
        public virtual tmProveedores tmProveedores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmCompras> tmCompras { get; set; }
    }
}
