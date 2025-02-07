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

    public partial class tmCompras
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tmCompras()
        {
            this.tmComprasDetalle = new HashSet<tmComprasDetalle>();
            this.tmCPEncabezado1 = new HashSet<tmCPEncabezado>();
        }

        public int intIdCmpra { get; set; }
        [Display(Name = "Auxilar")]
        public string varCdAuxliar { get; set; }
        [Display(Name = "No. Factura")]
        public string varNmroFctra { get; set; }
        [Display(Name = "Fecha Compra")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public Nullable<System.DateTime> fecCmpra { get; set; }
        [Display(Name = "Proveedor")]
        public int intIdPrvdor { get; set; }
        [Display(Name = "Rete Fte")]
        public Nullable<int> intIdRtncionfuente { get; set; }
        [Display(Name = "Vr. Rete Fte")]
        public Nullable<decimal> numVlorRtncionFuente { get; set; }
        [Display(Name = "Valor Iva")]
        public Nullable<decimal> numVlorIva { get; set; }
        [Display(Name = "Anulada")]
        public Nullable<bool> bitAnldo { get; set; }
        public Nullable<int> intIdCP { get; set; }
        public Nullable<int> intIdCmpnia { get; set; }
        public Nullable<bool> bitMrcar { get; set; }
        [Display(Name = "% Rete Fte")]
        public Nullable<decimal> numPrcntjeRtncionFte { get; set; }
        [Display(Name = "Valor Total")]
        public Nullable<decimal> numVlorTtal { get; set; }
        [Display(Name = "Adquirida Tipo Compra")]
        public Nullable<int> intIdAdquirdaTpoCmpra { get; set; }
        [Display(Name = "Resoluci�n")]
        public Nullable<int> intIdRslcion { get; set; }
        [Display(Name = "Descripci�n Tipo Compra")]
        public string varDscrpcionTpoCmpra { get; set; }
        [Display(Name = "Ciudad Regal�as")]
        public Nullable<int> intIdCiudadRglias { get; set; }
        [Display(Name = "Valor Regal�as")]
        public Nullable<decimal> numVlorRglias { get; set; }
        [Display(Name = "Cargue")]
        public Nullable<int> intCnsctvoCrgue { get; set; }
        [Display(Name = "Regal�as")]
        public Nullable<int> intIdRglia { get; set; }
        [Display(Name = "T�tulo Minero")]
        public Nullable<int> intIdTtloMnro { get; set; }
        public Nullable<int> intIdUsuario { get; set; }
        [Display(Name = "Ley 1429")]
        public Nullable<bool> bitAcgdosLey1429 { get; set; }
        [Display(Name = "Lote")]
        public string varNumLte { get; set; }
        [Display(Name = "Fecha pago Regal�as")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public Nullable<System.DateTime> fecPgoRglia { get; set; }
        [Display(Name = "Nmero Regal�as")]
        public string varNumRglia { get; set; }
        public string varNmbreCntcto { get; set; }
        public Nullable<decimal> numPrctjeDscuento { get; set; }
        public Nullable<decimal> numVlorDscuento { get; set; }
        public Nullable<int> intIdPriodo { get; set; }
        public Nullable<int> intIdLgarEmbrque { get; set; }
        public Nullable<decimal> numTsaCmbio { get; set; }
        public Nullable<int> numSmna { get; set; }
        public Nullable<System.DateTime> fecPgoVuce { get; set; }
        public Nullable<int> intIdRslcionCmpnia { get; set; }
        public string varNumDocEquvlnte { get; set; }
        [Display(Name = "Proyecto")]
        public string varPrycto { get; set; }
        [Display(Name = "Llave")]
        public string varLlve { get; set; }

        public virtual tmAdquiridasComprasTipo tmAdquiridasComprasTipo { get; set; }
        public virtual tmCiudades tmCiudades { get; set; }
        public virtual tmCPEncabezado tmCPEncabezado { get; set; }
        public virtual tmProveedores tmProveedores { get; set; }
        public virtual tmRegalias tmRegalias { get; set; }
        public virtual tmResolucionesProveedor tmResolucionesProveedor { get; set; }
        public virtual tmRetefuente tmRetefuente { get; set; }
        public virtual tmTituloMineroProveedor tmTituloMineroProveedor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmComprasDetalle> tmComprasDetalle { get; set; }
        public virtual tmImportadorExportador tmImportadorExportador { get; set; }
        public virtual tmImportadorExportador tmImportadorExportador1 { get; set; }
        public virtual tmLugaresEmbarque tmLugaresEmbarque { get; set; }
        public virtual tmResolucionesImportadorExportador tmResolucionesImportadorExportador { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmCPEncabezado> tmCPEncabezado1 { get; set; }
    }
}
