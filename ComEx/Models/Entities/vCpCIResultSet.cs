using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vCpCIResultSet
    {
        public DTResult<vCpCI> GetResult(string search, string sortOrder, int start, int length, List<vCpCI> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vCpCI> resultados = FilterResult(search, dtResult, columnFilters);
            int cont = resultados.Count();
            List<vCpCI> datos = FilterResult(search, dtResult, columnFilters).ToList();

            vCpCI totales = new vCpCI()
            {
                numBrtos = datos.Sum(s => s.numBrtos),
                numFnos = datos.Sum(s => s.numFnos)
            };

            DTResult<vCpCI> resultado = new DTResult<vCpCI>
            {
                data = resultados.SortBy(sortOrder).Skip(start).Take(length).ToList(),
                recordsTotal = cont,
                totals = totales
            };

            return resultado;
        }

        public int Count(string search, List<vCpCI> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<vCpCI> FilterResult(string search, List<vCpCI> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vCpCI> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.varNumLte != null && p.varNumLte.ToLower().Contains(search.ToLower()) ||
                p.intCnsctvoCrgue != null && p.intCnsctvoCrgue.ToString().ToLower().Contains(search.ToLower()) ||
                p.fecCP != null && Convert.ToDateTime(p.fecCP).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varCdCP != null && p.varCdCP.ToLower().Contains(search.ToLower()) || 
                p.varNmroCP != null && p.varNmroCP.ToLower().Contains(search.ToLower()) ||
                p.fecAprbcionCP != null && Convert.ToDateTime(p.fecAprbcionCP).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNmbre != null && p.varNmbre.ToLower().Contains(search.ToLower()) ||
                p.fecBmstre != null && Convert.ToDateTime(p.fecBmstre).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.fecMdfccion != null && Convert.ToDateTime(p.fecMdfccion).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varMtvoMdfccion != null && p.varMtvoMdfccion.ToLower().Contains(search.ToLower()) ||
                p.varRtaArchvoAdjnto != null && p.varRtaArchvoAdjnto.ToLower().Contains(search.ToLower()) ||
                p.varCdPlanCI != null && p.varCdPlanCI.ToLower().Contains(search.ToLower()) ||
                p.bitAnldo != null && p.bitAnldo.ToString().ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionPlanCI != null && p.varDscrpcionPlanCI.ToLower().Contains(search.ToLower()) ||
                p.varCPMdfccion != null && p.varCPMdfccion.ToLower().Contains(search.ToLower())))

               /* && (columnFilters[0] == null || (p.varNumLte != null && p.varNumLte.ToLower().Contains(columnFilters[0].ToLower())))
                && (columnFilters[1] == null || (p.City != null && p.City.ToLower().Contains(columnFilters[1].ToLower())))
                && (columnFilters[2] == null || (p.Postal != null && p.Postal.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.Email != null && p.Email.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.Company != null && p.Company.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.Account != null && p.Account.ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.CreditCard != null && p.CreditCard.ToLower().Contains(columnFilters[6].ToLower())))*/
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);
            
            return results;
        }
    }
}