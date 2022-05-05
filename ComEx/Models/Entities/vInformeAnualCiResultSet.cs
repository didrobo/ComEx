using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vInformeAnualCiResultSet
    {
        public List<vInformeAnualCi> GetResult(string search, string sortOrder, int start, int length, List<vInformeAnualCi> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<vInformeAnualCi> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<vInformeAnualCi> FilterResult(string search, List<vInformeAnualCi> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vInformeAnualCi> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.intNum.ToString().ToLower().Contains(search.ToLower()) ||
                p.varCdCP != null && p.varCdCP.ToLower().Contains(search.ToLower()) ||
                p.fecBmstre != null && Convert.ToDateTime(p.fecBmstre).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.fecCP != null && Convert.ToDateTime(p.fecCP).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.intNumItem.ToString().ToLower().Contains(search.ToLower()) ||
                p.varNmroFctra != null && p.varNmroFctra.ToLower().Contains(search.ToLower()) ||
                p.fecCmpra != null && Convert.ToDateTime(p.fecCmpra).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.numVlorCp.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorTtalAnual.ToString().ToLower().Contains(search.ToLower()) ||
                p.varPrdctoSinTrnsfrmar != null && p.varPrdctoSinTrnsfrmar.ToLower().Contains(search.ToLower()) ||
                p.varMtriaPrima != null && p.varMtriaPrima.ToLower().Contains(search.ToLower()) ||
                p.varSrvcioIntrmdioPrdccion != null && p.varSrvcioIntrmdioPrdccion.ToLower().Contains(search.ToLower()) ||
                p.fecEnvioCP != null && Convert.ToDateTime(p.fecEnvioCP).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNmroDEX != null && p.varNmroDEX.ToLower().Contains(search.ToLower()) ||
                p.fecAprbcionDEX != null && Convert.ToDateTime(p.fecAprbcionDEX).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varCdAduana != null && p.varCdAduana.ToLower().Contains(search.ToLower()) ||
                p.fecEmbrque != null && Convert.ToDateTime(p.fecEmbrque).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.numVlrTtalFobUSD.ToString().ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}