using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vInventarioCpResultSet
    {
        public DTResult<vInventarioCp> GetResult(string search, string sortOrder, int start, int length, List<vInventarioCp> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vInventarioCp> resultados = FilterResult(search, dtResult, columnFilters);
            int cont = resultados.Count();
            List<vInventarioCp> datos = FilterResult(search, dtResult, columnFilters).ToList();
            vInventarioCp totales = new vInventarioCp()
            {
                numCntdadCp = datos.Sum(s => s.numCntdadCp),
                numCntdadCnsmda = datos.Sum(s => s.numCntdadCnsmda),
                numCntCnsmdaDEX = datos.Sum(s => s.numCntCnsmdaDEX),
                numCntdadPndienteCP = datos.Sum(s => s.numCntdadPndienteCP),
                numVlorCpPorCI = datos.Sum(s => s.numVlorCpPorCI),
                numVlorCnsmdoCpPorCI = datos.Sum(s => s.numVlorCnsmdoCpPorCI),
                numVlorPndienteCpPorCI = datos.Sum(s => s.numVlorPndienteCpPorCI)
            };

            DTResult<vInventarioCp> resultado = new DTResult<vInventarioCp>
            {
                data = resultados.SortBy(sortOrder).Skip(start).Take(length).ToList(),
                recordsTotal = cont,
                totals = totales
            };

            return resultado;
        }

        private IQueryable<vInventarioCp> FilterResult(string search, List<vInventarioCp> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vInventarioCp> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.intIdCP.ToString().ToLower().Contains(search.ToLower()) ||
                p.varCdCP != null && p.varCdCP.ToLower().Contains(search.ToLower()) ||
                p.fecCP != null && Convert.ToDateTime(p.fecCP).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNmroCP != null && p.varNmroCP.ToLower().Contains(search.ToLower()) ||
                p.fecAprbcionCP != null && Convert.ToDateTime(p.fecAprbcionCP).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varProveedor != null && p.varProveedor.ToLower().Contains(search.ToLower()) ||
                p.varCI != null && p.varCI.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionInsmo != null && p.varDscrpcionInsmo.ToLower().Contains(search.ToLower()) ||
                p.varUndadCP != null && p.varUndadCP.ToLower().Contains(search.ToLower()) ||
                p.numCntdadCp.ToString().ToLower().Contains(search.ToLower()) ||
                p.numCntdadCnsmda.ToString().ToLower().Contains(search.ToLower()) ||
                p.numCntCnsmdaDEX.ToString().ToLower().Contains(search.ToLower()) ||
                p.numCntdadPndienteCP.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorCpPorCI.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorCnsmdoCpPorCI.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorPndienteCpPorCI.ToString().ToLower().Contains(search.ToLower()) ||
                p.varSemaforo != null && p.varSemaforo.ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}