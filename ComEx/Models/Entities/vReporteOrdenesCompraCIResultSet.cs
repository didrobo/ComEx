using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vReporteOrdenesCompraCIResultSet
    {
        public DTResult<vReporteOrdenesCompraCI> GetResult(string search, string sortOrder, int start, int length, List<vReporteOrdenesCompraCI> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vReporteOrdenesCompraCI> resultados = FilterResult(search, dtResult, columnFilters);
            int cont = resultados.Count();
            List<vReporteOrdenesCompraCI> datos = FilterResult(search, dtResult, columnFilters).ToList();
            vReporteOrdenesCompraCI totales = new vReporteOrdenesCompraCI()
            {
                numCntdadFctra = datos.Sum(s => s.numCntdadFctra),
                numFnos = datos.Sum(s => s.numFnos),
                numFnosAG = datos.Sum(s => s.numFnosAG),
                numFnosPT = datos.Sum(s => s.numFnosPT),
                numRglias = datos.Sum(s => s.numRglias),
                numRgliasAG = datos.Sum(s => s.numRgliasAG),
                numRgliasPT = datos.Sum(s => s.numRgliasPT),
                numVlorRtncionFuente = datos.Sum(s => s.numVlorRtncionFuente),
                numValorPagar = datos.Sum(s => s.numValorPagar),
                numValorAntesRetencion = datos.Sum(s => s.numValorAntesRetencion),
                numSbttal = datos.Sum(s => s.numSbttal),
                numVlorFOB = datos.Sum(s => s.numVlorFOB)                
            };

            DTResult<vReporteOrdenesCompraCI> resultado = new DTResult<vReporteOrdenesCompraCI>
            {
                data = resultados.SortBy(sortOrder).Skip(start).Take(length).ToList(),
                recordsTotal = cont,
                totals = totales
            };

            return resultado;
        }

        private IQueryable<vReporteOrdenesCompraCI> FilterResult(string search, List<vReporteOrdenesCompraCI> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vReporteOrdenesCompraCI> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.varNumLte != null && p.varNumLte.ToLower().Contains(search.ToLower()) ||
                p.intCnsctvoCrgue != null && p.intCnsctvoCrgue.ToString().ToLower().Contains(search.ToLower()) ||
                p.varCdAuxliarCmpra != null && p.varCdAuxliarCmpra.ToLower().Contains(search.ToLower()) ||
                p.varNmroFctra != null && p.varNmroFctra.ToLower().Contains(search.ToLower()) ||
                p.fecCmpra != null && Convert.ToDateTime(p.fecCmpra).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNitPrvdor != null && p.varNitPrvdor.ToLower().Contains(search.ToLower()) ||
                p.varNmbrePrvdor != null && p.varNmbrePrvdor.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionInsmo != null && p.varDscrpcionInsmo.ToLower().Contains(search.ToLower()) ||
                p.numCntdadFctra != null && p.numCntdadFctra.ToString().ToLower().Contains(search.ToLower()) ||
                p.numFnos != null && p.numFnos.ToString().ToLower().Contains(search.ToLower()) ||
                p.numFnosAG.ToString().ToLower().Contains(search.ToLower()) ||
                p.numLey != null && p.numLey.ToString().ToLower().Contains(search.ToLower()) ||
                p.numLeyAG.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorUntrio != null && p.numVlorUntrio.ToString().ToLower().Contains(search.ToLower()) ||
                p.numRglias != null && p.numRglias.ToString().ToLower().Contains(search.ToLower()) ||
                p.numRgliasAG.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorRtncionFuente != null && p.numVlorRtncionFuente.ToString().ToLower().Contains(search.ToLower()) ||
                p.numValorPagar != null && p.numValorPagar.ToString().ToLower().Contains(search.ToLower()) ||
                p.numValorAntesRetencion != null && p.numValorAntesRetencion.ToString().ToLower().Contains(search.ToLower()) ||
                p.numSbttal != null && p.numSbttal.ToString().ToLower().Contains(search.ToLower()) ||
                p.bitAcgdosLey1429 != null && p.bitAcgdosLey1429.ToString().ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionCiudad != null && p.varDscrpcionCiudad.ToLower().Contains(search.ToLower()) ||
                p.varDrccion != null && p.varDrccion.ToLower().Contains(search.ToLower()) ||
                p.varNbreRgmen != null && p.varNbreRgmen.ToLower().Contains(search.ToLower()) ||
                p.numVlorPorGrmo != null && p.numVlorPorGrmo.ToString().ToLower().Contains(search.ToLower()) ||
                p.intAnoCmpra != null && p.intAnoCmpra.ToString().ToLower().Contains(search.ToLower()) ||
                p.intMesCmpra != null && p.intMesCmpra.ToString().ToLower().Contains(search.ToLower()) ||
                p.intDiaCmpra != null && p.intDiaCmpra.ToString().ToLower().Contains(search.ToLower()) ||
                p.varCiudadRglias != null && p.varCiudadRglias.ToLower().Contains(search.ToLower()) ||
                p.varDepartamentoRglias != null && p.varDepartamentoRglias.ToLower().Contains(search.ToLower()) ||
                p.varCdCP != null && p.varCdCP.ToLower().Contains(search.ToLower()) ||
                p.fecCp != null && Convert.ToDateTime(p.fecCp).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNmroCP != null && p.varNmroCP.ToLower().Contains(search.ToLower()) ||
                p.fecAprbcionCP != null && Convert.ToDateTime(p.fecAprbcionCP).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varCdAuxliarDEX != null && p.varCdAuxliarDEX.ToLower().Contains(search.ToLower()) ||
                p.fecAuxliar != null && Convert.ToDateTime(p.fecAuxliar).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNmroDEX != null && p.varNmroDEX.ToLower().Contains(search.ToLower()) ||
                p.fecAprbcionDEX != null && Convert.ToDateTime(p.fecAprbcionDEX).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.numVlorFOB != null && p.numVlorFOB.ToString().ToLower().Contains(search.ToLower()) ||
                p.varNmbreCmprdor != null && p.varNmbreCmprdor.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionPais != null && p.varDscrpcionPais.ToLower().Contains(search.ToLower()) ||
                p.varNmroExprtcion != null && p.varNmroExprtcion.ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}