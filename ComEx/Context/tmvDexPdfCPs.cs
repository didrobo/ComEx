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
    
    public partial class tmvDexPdfCPs
    {
        public int intIdDexPdfCPs { get; set; }
        public int intIdDEX { get; set; }
        public string varNmroCP { get; set; }
        public Nullable<System.DateTime> fecCP { get; set; }
        public Nullable<System.DateTime> fecVncmientoCP { get; set; }
        public Nullable<decimal> numSbttal { get; set; }
        public string varCdMndaCP { get; set; }
        public string varCdUndadCP { get; set; }
        public string varUndadCP { get; set; }
        public Nullable<decimal> numCntdadUndadCP { get; set; }
        public Nullable<int> intIdCP { get; set; }
        public string varErrorDexInicial { get; set; }
        public string varErrorDexCorrecion { get; set; }
    
        public virtual tmCPEncabezado tmCPEncabezado { get; set; }
        public virtual tmvDEXEncabezado tmvDEXEncabezado { get; set; }
    }
}
