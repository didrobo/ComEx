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

    public partial class tmProveedores
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tmProveedores()
        {
            this.tmCPEncabezado = new HashSet<tmCPEncabezado>();
            this.tmResolucionesProveedor = new HashSet<tmResolucionesProveedor>();
            this.tmCompras = new HashSet<tmCompras>();
            this.tmTituloMineroProveedor = new HashSet<tmTituloMineroProveedor>();
            this.tmTituloMineroProveedor1 = new HashSet<tmTituloMineroProveedor>();
            this.tmRemisionDocumentos = new HashSet<tmRemisionDocumentos>();
        }

        public int intIdPrvdor { get; set; }
        [Display(Name = "C�digo")]
        public string varCdPrvdor { get; set; }
        [Display(Name = "NIT")]
        public string varNitPrvdor { get; set; }
        [Display(Name = "Raz�n Social")]
        public string varNmbre { get; set; }
        [Display(Name = "Direcci�n")]
        public string varDrccion { get; set; }
        [Display(Name = "Tel�fono")]
        public string varTlfno { get; set; }
        [Display(Name = "Pa�s")]
        public int intIdPais { get; set; }
        public Nullable<bool> bitPrveedorNcional { get; set; }
        [Display(Name = "Segundo Apellido")]
        public string varAplldos { get; set; }
        [Display(Name = "Ciudad")]
        public Nullable<int> intIdCiudad { get; set; }
        [Display(Name = "Email")]
        public string varEmail { get; set; }
        [Display(Name = "Primer Nombre")]
        public string varNmbre1 { get; set; }
        [Display(Name = "Primer Apellido")]
        public string varAplldos1 { get; set; }
        [Display(Name = "Otros Nombres")]
        public string varNmbre2 { get; set; }
        [Display(Name = "R�gimen")]
        public Nullable<int> intIdRgmen { get; set; }
        [Display(Name = "Bloqueado")]
        public Nullable<bool> bitBlqueado { get; set; }
        public string varAbrviatraArchvos { get; set; }
        [Display(Name = "Ley 1429")]
        public Nullable<bool> bitAcgdosLey1429 { get; set; }
        [Display(Name = "Exportador")]
        public Nullable<int> intIdCmpnia { get; set; }
        [Display(Name = "Fairmined Id")]
        public string varFairminedId { get; set; }
        
        public string varCiudad { get; set; }
        public string varFax { get; set; }
        public Nullable<int> intIdBnco { get; set; }
        public Nullable<int> intIdMnda { get; set; }
        public Nullable<int> intIdIncterms { get; set; }
        public string varCuenta { get; set; }
        public Nullable<int> intIdFrmaPgo { get; set; }
        public Nullable<decimal> numdiaspgo { get; set; }
        public Nullable<int> intIdCndcionPrvdor { get; set; }
        public string varOtraCndcion { get; set; }
        public Nullable<decimal> numrtncionfnte { get; set; }
        public Nullable<int> intIdTpoPrvdor { get; set; }
        public string varCmpniaInter { get; set; }
        public Nullable<int> intIdClsfccionPrvdor { get; set; }
        public Nullable<decimal> numprcntjeantcpo { get; set; }
        public string varCdPdrePrvdor { get; set; }
        public Nullable<int> intIdLgarEmbrque { get; set; }
        public Nullable<bool> bitMrcar { get; set; }
        public string varClveWeb { get; set; }
        public string varAbrvtraPrvdor { get; set; }
        public Nullable<int> intIdTpoDcmntoIdntfccion { get; set; }
        public string varCdPstal { get; set; }
        public string varCdgoSwift { get; set; }
        public string varCdgoBIC { get; set; }
        public string varCdgoIBAN { get; set; }
        public string varCdgoABA { get; set; }
        public string varCdgoCAB { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmCPEncabezado> tmCPEncabezado { get; set; }
        public virtual tmCiudades tmCiudades { get; set; }
        public virtual tmPaises tmPaises { get; set; }
        public virtual tmRegimenes tmRegimenes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmResolucionesProveedor> tmResolucionesProveedor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmCompras> tmCompras { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmTituloMineroProveedor> tmTituloMineroProveedor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmTituloMineroProveedor> tmTituloMineroProveedor1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmRemisionDocumentos> tmRemisionDocumentos { get; set; }
        public virtual tmIncoterms tmIncoterms { get; set; }
        public virtual tmMonedas tmMonedas { get; set; }
        public virtual tmLugaresEmbarque tmLugaresEmbarque { get; set; }
    }
}
