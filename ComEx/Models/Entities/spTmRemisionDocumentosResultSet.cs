using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class spTmRemisionDocumentosResultSet
    {
        public List<spTmRemisionDocumentos_Result> GetResult(string search, string sortOrder, int start, int length, List<spTmRemisionDocumentos_Result> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<spTmRemisionDocumentos_Result> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<spTmRemisionDocumentos_Result> FilterResult(string search, List<spTmRemisionDocumentos_Result> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<spTmRemisionDocumentos_Result> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.intIdRmsion.ToString().ToLower().Contains(search.ToLower()) ||
                p.intIdPrvdor.ToString().ToLower().Contains(search.ToLower()) ||
                p.varCdPrvdor != null && p.varCdPrvdor.ToLower().Contains(search.ToLower()) ||
                p.varNmbre != null && p.varNmbre.ToLower().Contains(search.ToLower()) ||
                p.intMes.ToString().ToLower().Contains(search.ToLower()) ||
                p.fecEnvio != null && Convert.ToDateTime(p.fecEnvio).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNumGuiaEnvio != null && p.varNumGuiaEnvio.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionCiudad != null && p.varDscrpcionCiudad.ToLower().Contains(search.ToLower()) ||
                p.fecRcbdo != null && Convert.ToDateTime(p.fecRcbdo).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.fecEntrgaCliente != null && Convert.ToDateTime(p.fecEntrgaCliente).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varQuienRcbe != null && p.varQuienRcbe.ToLower().Contains(search.ToLower()) ||
                p.fecDspcho != null && Convert.ToDateTime(p.fecDspcho).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNumGuiaDspacho != null && p.varNumGuiaDspacho.ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}