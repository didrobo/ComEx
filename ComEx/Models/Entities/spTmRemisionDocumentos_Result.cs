namespace ComEx.Context
{
    using System;

    public partial class spTmRemisionDocumentos_Result
    {
        public bool ShouldSerializeintIdPrvdor()
        {
            return intIdPrvdor.HasValue;
        }

        public bool ShouldSerializeintMes()
        {
            return intMes.HasValue;
        }

        public bool ShouldSerializefecEnvio()
        {
            return fecEnvio.HasValue;
        }

        public bool ShouldSerializefecRcbdo()
        {
            return fecRcbdo.HasValue;
        }

        public bool ShouldSerializefecEntrgaCliente()
        {
            return fecEntrgaCliente.HasValue;
        }

        public bool ShouldSerializefecDspcho()
        {
            return fecDspcho.HasValue;
        }
    }
}