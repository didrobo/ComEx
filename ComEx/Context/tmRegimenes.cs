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
    
    public partial class tmRegimenes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tmRegimenes()
        {
            this.tmProveedores = new HashSet<tmProveedores>();
        }
    
        public int intIdRgmen { get; set; }
        public string varCdRgmen { get; set; }
        public string varNbreRgmen { get; set; }
        public string varDscrpcionRgmen { get; set; }
        public Nullable<decimal> numVlorMxmoVntas { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmProveedores> tmProveedores { get; set; }
    }
}
