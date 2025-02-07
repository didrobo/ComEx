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

    public partial class tmvFacturasExportacionEncabezado
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tmvFacturasExportacionEncabezado()
        {
            this.tmvFacturasExportacionDetalle = new HashSet<tmvFacturasExportacionDetalle>();
            this.tmvDEXDetalle = new HashSet<tmvDEXDetalle>();
            this.tmvFacturasExportacionNotasCredito = new HashSet<tmvFacturasExportacionNotasCredito>();
        }

        public int intIdFctraExprtcion { get; set; }
        [Display(Name = "Auxilar")]
        public string varNmroAuxliar { get; set; }
        [Display(Name = "Factura")]
        public string varNmroFctra { get; set; }
        public Nullable<System.DateTime> fecAuxliar { get; set; }
        [Display(Name = "F. Factura")]
        public Nullable<System.DateTime> fecFctra { get; set; }
        [Display(Name = "F. Vencimiento")]
        public Nullable<System.DateTime> fecVncmientoFctra { get; set; }
        [Display(Name = "Comprador")]
        public int intIdCmprdor { get; set; }
        [Display(Name = "Exportador")]
        public int intIdImprtdorExprtdor { get; set; }
        [Display(Name = "Tasa Cambio")]
        public Nullable<decimal> numTsaCmbio { get; set; }
        [Display(Name = "Subtotal")]
        public Nullable<decimal> numSbttal { get; set; }
        [Display(Name = "Incoterm")]
        public Nullable<int> intIdIncterms { get; set; }
        [Display(Name = "Moneda")]
        public Nullable<int> intIdMnda { get; set; }
        [Display(Name = "Vr. FOB")]
        public Nullable<decimal> numVlorFOB { get; set; }
        [Display(Name = "Fletes")]
        public Nullable<decimal> numFltes { get; set; }
        [Display(Name = "Iva")]
        public Nullable<decimal> numGstos { get; set; }
        [Display(Name = "Seguro")]
        public Nullable<decimal> numSgroUS { get; set; }
        [Display(Name = "Lote")]
        public string varNmroExprtcion { get; set; }
        [Display(Name = "Peso Bruto")]
        public Nullable<decimal> numPsoBrto { get; set; }
        [Display(Name = "Peso Neto")]
        public Nullable<decimal> numPsoNto { get; set; }
        public bool bitCrtra { get; set; }
        [Display(Name = "Anulada")]
        public bool bitAnlda { get; set; }
        [Display(Name = "Exportación")]
        public string varDcmntoTrnsprte { get; set; }
        [Display(Name = "Venta Ncnal")]
        public bool bitVntaNcional { get; set; }
        [Display(Name = "F. Exportación")]
        public Nullable<System.DateTime> fecExprtcion { get; set; }
        public string varPrfjo { get; set; }
        public string varPrfrma { get; set; }
        public Nullable<decimal> numGstosFob { get; set; }
        public Nullable<decimal> numVlorImpuestos { get; set; }
        public Nullable<int> intIdTpoPlan { get; set; }
        public Nullable<int> intIdTpoDcmntoSprte { get; set; }
        public Nullable<int> intIdPuertoOrgen { get; set; }
        public Nullable<int> intIdPuertoDstno { get; set; }
        public Nullable<decimal> numBltos { get; set; }
        public Nullable<int> intIdTrnsprte { get; set; }
        public string varDscrpcionEmblje { get; set; }
        public string varPddoCliente { get; set; }
        public string varRmsion { get; set; }
        public string varNtfca { get; set; }
        public string varCnsgna { get; set; }
        public Nullable<int> intIdFrmaPgo { get; set; }
        public Nullable<bool> bitMrcar { get; set; }
        public Nullable<bool> bitEnvioUrgnte { get; set; }
        public Nullable<System.DateTime> fecRqurdaPlnta { get; set; }
        public Nullable<bool> bitDsctvarDcmnto { get; set; }
        public string varCmntrios { get; set; }
        public Nullable<bool> bitDEX { get; set; }
        public string varFobOprcionAltrdoPor { get; set; }
        public string varCiudadIncoterm { get; set; }
        public string varNmroPddo { get; set; }
        public Nullable<int> intIdBdga { get; set; }
        public Nullable<System.DateTime> fecLmteRcpcionPddo { get; set; }
        public string varNmroDex { get; set; }
        public Nullable<System.DateTime> fecAprbcionDEX { get; set; }
        public Nullable<System.DateTime> fecDcmntoTrnsprte { get; set; }
        public string varCrtfcdoOrgen { get; set; }
        public string varAutoDian { get; set; }
        public Nullable<System.DateTime> fecAutoDian { get; set; }
        public string varGuia { get; set; }
        public Nullable<int> intIdNviera { get; set; }
        public Nullable<int> intIdAgnteCrga { get; set; }
        public string varObsrvciones { get; set; }
        public Nullable<int> intIdCmprdorSlctaOrden { get; set; }
        public Nullable<int> intIdCmprdorDstnoOrden { get; set; }
        public Nullable<bool> bitMuestraSVC { get; set; }
        public Nullable<bool> bitImprsionfctraalico { get; set; }
        public Nullable<bool> bitImprsionprfrmaalico { get; set; }
        public Nullable<int> intIdUsuario { get; set; }
        public Nullable<int> intCjas { get; set; }
        public Nullable<bool> bitAdnaProrrtada { get; set; }
        public Nullable<bool> bitDDPCliente { get; set; }
        public Nullable<bool> bitDDUAduana { get; set; }
        public Nullable<bool> bitFrmto2002 { get; set; }
        public Nullable<bool> bitPrfrmaPscionArncel { get; set; }
        public Nullable<bool> bitTxtilPscionArncel { get; set; }
        public Nullable<int> intIdPddoExprtcionEncbzdo { get; set; }
        public Nullable<bool> bitMQ { get; set; }
        public Nullable<decimal> numCjas { get; set; }
        public Nullable<bool> bitNptno { get; set; }
        public Nullable<int> intIdOpcion { get; set; }
        public string varTpoNgcio { get; set; }
        public Nullable<int> intCnsctvoMstra { get; set; }
        public Nullable<int> intRslcion { get; set; }
        public Nullable<bool> bitBrrdoLgco { get; set; }
        public Nullable<int> intIdFrmaEmbrque { get; set; }
        public Nullable<int> intIdTpoEmblje { get; set; }
        public Nullable<decimal> numVlorEXW { get; set; }
        public Nullable<int> intIdAnlstaExprtcion { get; set; }
        public string varNmroCrtaReintgro { get; set; }
        public Nullable<System.DateTime> fecCrtaReintgro { get; set; }
        public Nullable<int> intidUsuarioCrtaReintgro { get; set; }
        public Nullable<bool> bitInctvo { get; set; }
        public Nullable<bool> bitFctraMstraLeonisa { get; set; }
        public Nullable<int> intIdAduana { get; set; }
        public Nullable<int> intIdDclrnte { get; set; }
        public Nullable<int> intIdUsuarioMrcar { get; set; }
        public Nullable<int> intIdTrnsprtdorLcal { get; set; }
        
    
        public virtual tmCompradores tmCompradores { get; set; }
        public virtual tmImportadorExportador tmImportadorExportador { get; set; }
        public virtual tmMonedas tmMonedas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmvFacturasExportacionDetalle> tmvFacturasExportacionDetalle { get; set; }
        public virtual tmIncoterms tmIncoterms { get; set; }
        public virtual tmCompradores tmCompradores1 { get; set; }
        public virtual tmCompradores tmCompradores2 { get; set; }
        public virtual tmLugaresEmbarque tmLugaresEmbarque { get; set; }
        public virtual tmLugaresEmbarque tmLugaresEmbarque1 { get; set; }
        public virtual tmTerceros tmTerceros { get; set; }
        public virtual tmTerceros tmTerceros1 { get; set; }
        public virtual tmTransportes tmTransportes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmvDEXDetalle> tmvDEXDetalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tmvFacturasExportacionNotasCredito> tmvFacturasExportacionNotasCredito { get; set; }
    }
}
