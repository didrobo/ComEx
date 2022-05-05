using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vDEXResultSet
    {
        public DTResult<vDEX> GetResult(string search, string sortOrder, int start, int length, List<vDEX> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vDEX> resultados = FilterResult(search, dtResult, columnFilters);
            int cont = resultados.Count();
            List<vDEX> datos = FilterResult(search, dtResult, columnFilters).ToList();
            vDEX totales = new vDEX()
            {
                numVlorFOB = datos.Sum(s => s.numVlorFOB),
                numFltes = datos.Sum(s => s.numFltes),
                numSgro = datos.Sum(s => s.numSgro),
                numOtrosGstos = datos.Sum(s => s.numOtrosGstos),
                numTtalPsoBrto = datos.Sum(s => s.numTtalPsoBrto),
                numTtalPsoNto = datos.Sum(s => s.numTtalPsoNto),
                numTtalSries = datos.Sum(s => s.numTtalSries),
                numBltos = datos.Sum(s => s.numBltos)
            };

            DTResult<vDEX> resultado = new DTResult<vDEX>
            {
                data = resultados.SortBy(sortOrder).Skip(start).Take(length).ToList(),
                recordsTotal = cont,
                totals = totales
            };

            return resultado;
        }

        private IQueryable<vDEX> FilterResult(string search, List<vDEX> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vDEX> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.varCdAuxliar != null && p.varCdAuxliar.ToLower().Contains(search.ToLower()) ||                
                p.fecAuxliar != null && Convert.ToDateTime(p.fecAuxliar).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNmroDEX != null && p.varNmroDEX.ToLower().Contains(search.ToLower()) ||
                p.fecAprbcionDEX != null && Convert.ToDateTime(p.fecAprbcionDEX).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.fecEmbrque != null && Convert.ToDateTime(p.fecEmbrque).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varCmprdor != null && p.varCmprdor.ToLower().Contains(search.ToLower()) ||
                p.varExprtdor != null && p.varExprtdor.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionAduana != null && p.varDscrpcionAduana.ToLower().Contains(search.ToLower()) ||
                p.varCmntrio != null && p.varCmntrio.ToLower().Contains(search.ToLower()) ||
                p.numFltes != null && p.numFltes.ToString().ToLower().Contains(search.ToLower()) ||
                p.numSgro != null && p.numSgro.ToString().ToLower().Contains(search.ToLower()) ||
                p.numOtrosGstos != null && p.numOtrosGstos.ToString().ToLower().Contains(search.ToLower()) ||
                p.varRtaArchvoAdjnto != null && p.varRtaArchvoAdjnto.ToLower().Contains(search.ToLower()) ||
                p.varDclrnte != null && p.varDclrnte.ToLower().Contains(search.ToLower()) ||
                p.varDtnoFinal != null && p.varDtnoFinal.ToLower().Contains(search.ToLower()) ||
                p.numVlorFOB != null && p.numVlorFOB.ToString().ToLower().Contains(search.ToLower()) ||
                p.numExprtcionesUSD != null && p.numExprtcionesUSD.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorReintgrarUSD != null && p.numVlorReintgrarUSD.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorAgrgdoNcional != null && p.numVlorAgrgdoNcional.ToString().ToLower().Contains(search.ToLower()) ||
                p.numTtalSries != null && p.numTtalSries.ToString().ToLower().Contains(search.ToLower()) ||
                p.numBltos != null && p.numBltos.ToString().ToLower().Contains(search.ToLower()) ||
                p.numTtalPsoNto != null && p.numTtalPsoNto.ToString().ToLower().Contains(search.ToLower()) ||
                p.numTtalPsoBrto != null && p.numTtalPsoBrto.ToString().ToLower().Contains(search.ToLower()) ||
                p.numNumroAcptcion != null && p.numNumroAcptcion.ToString().ToLower().Contains(search.ToLower()) ||
                p.fecAcptcion != null && Convert.ToDateTime(p.fecAcptcion).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varSlctudAtrzcionEmbrque != null && p.varSlctudAtrzcionEmbrque.ToLower().Contains(search.ToLower()) ||
                p.fecAtrzcionEmbrque != null && Convert.ToDateTime(p.fecAtrzcionEmbrque).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varMnfiestoCrga != null && p.varMnfiestoCrga.ToLower().Contains(search.ToLower()) ||
                p.fecMnfiestoCrga != null && Convert.ToDateTime(p.fecMnfiestoCrga).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varDscrpcionTrnsprte != null && p.varDscrpcionTrnsprte.ToLower().Contains(search.ToLower()) ||
                p.numCntdadEmblje != null && p.numCntdadEmblje.ToString().ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionPlanCI != null && p.varDscrpcionPlanCI.ToLower().Contains(search.ToLower()) ||
                p.varNmroFrmlrioAntrior != null && p.varNmroFrmlrioAntrior.ToLower().Contains(search.ToLower()) ||
                p.varNmroDcmntoTrnsprte != null && p.varNmroDcmntoTrnsprte.ToLower().Contains(search.ToLower()) ||
                p.fecDcmntoTrnsprte != null && Convert.ToDateTime(p.fecDcmntoTrnsprte).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}