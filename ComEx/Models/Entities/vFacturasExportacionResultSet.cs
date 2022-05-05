using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vFacturasExportacionResultSet
    {
        public DTResult<vFacturasExportacion> GetResult(string search, string sortOrder, int start, int length, List<vFacturasExportacion> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vFacturasExportacion> resultados = FilterResult(search, dtResult, columnFilters);
            int cont = resultados.Count();
            List<vFacturasExportacion> datos = FilterResult(search, dtResult, columnFilters).ToList();
            vFacturasExportacion totales = new vFacturasExportacion()
            {
                numSbttal = datos.Sum(s => s.numSbttal),
                numVlorFOB = datos.Sum(s => s.numVlorFOB),
                numFltes = datos.Sum(s => s.numFltes),
                numGstos = datos.Sum(s => s.numGstos),
                numSgroUS = datos.Sum(s => s.numSgroUS),
                numPsoBrto = datos.Sum(s => s.numPsoBrto),
                numPsoNto = datos.Sum(s => s.numPsoNto)
            };

            DTResult<vFacturasExportacion> resultado = new DTResult<vFacturasExportacion>
            {
                data = resultados.SortBy(sortOrder).Skip(start).Take(length).ToList(),
                recordsTotal = cont,
                totals = totales
            };

            return resultado;
        }


        private IQueryable<vFacturasExportacion> FilterResult(string search, List<vFacturasExportacion> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vFacturasExportacion> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.varNmroAuxliar != null && p.varNmroAuxliar.ToLower().Contains(search.ToLower()) ||
                p.varNmroFctra != null && p.varNmroFctra.ToLower().Contains(search.ToLower()) ||
                p.fecAuxliar != null && Convert.ToDateTime(p.fecAuxliar).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.fecFctra != null && Convert.ToDateTime(p.fecFctra).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.fecVncmientoFctra != null && Convert.ToDateTime(p.fecVncmientoFctra).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||

                p.varCmprdor != null && p.varCmprdor.ToLower().Contains(search.ToLower()) ||
                p.varExprtdor != null && p.varExprtdor.ToLower().Contains(search.ToLower()) ||
                p.numTsaCmbio != null && p.numTsaCmbio.ToString().ToLower().Contains(search.ToLower()) ||
                p.numSbttal != null && p.numSbttal.ToString().ToLower().Contains(search.ToLower()) ||
                p.varCdIncterms != null && p.varCdIncterms.ToLower().Contains(search.ToLower()) ||
                p.varCdMnda != null && p.varCdMnda.ToLower().Contains(search.ToLower()) ||
                p.numVlorFOB != null && p.numVlorFOB.ToString().ToLower().Contains(search.ToLower()) ||
                p.numFltes != null && p.numFltes.ToString().ToLower().Contains(search.ToLower()) ||
                p.numGstos != null && p.numGstos.ToString().ToLower().Contains(search.ToLower()) ||
                p.numSgroUS != null && p.numSgroUS.ToString().ToLower().Contains(search.ToLower()) ||
                p.varNmroExprtcion != null && p.varNmroExprtcion.ToLower().Contains(search.ToLower()) ||
                p.numPsoBrto != null && p.numPsoBrto.ToString().ToLower().Contains(search.ToLower()) ||
                p.numPsoNto != null && p.numPsoNto.ToString().ToLower().Contains(search.ToLower()) ||
                p.bitAnlda.ToString().ToLower().Contains(search.ToLower()) ||
                p.varDcmntoTrnsprte != null && p.varDcmntoTrnsprte.ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}