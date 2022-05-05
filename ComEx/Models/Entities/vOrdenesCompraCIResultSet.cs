using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vOrdenesCompraCIResultSet
    {
        public DTResult<vOrdenesCompraCI> GetResult(string search, string sortOrder, int start, int length, List<vOrdenesCompraCI> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vOrdenesCompraCI> resultados = FilterResult(search, dtResult, columnFilters);
            int cont = resultados.Count();
            List<vOrdenesCompraCI> datos = FilterResult(search, dtResult, columnFilters).ToList();
            vOrdenesCompraCI totales = new vOrdenesCompraCI()
            {
                numVlorTtal = datos.Sum(s => s.numVlorTtal),
                numVlorIva = datos.Sum(s => s.numVlorIva),
                numVlorRglias = datos.Sum(s => s.numVlorRglias),
                numBrtos = datos.Sum(s => s.numBrtos),
                numFnos = datos.Sum(s => s.numFnos)
            };

            DTResult<vOrdenesCompraCI> resultado = new DTResult<vOrdenesCompraCI>
            {
                data = resultados.SortBy(sortOrder).Skip(start).Take(length).ToList(),
                recordsTotal = cont,
                totals = totales
            };

            return resultado;
        }


        private IQueryable<vOrdenesCompraCI> FilterResult(string search, List<vOrdenesCompraCI> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vOrdenesCompraCI> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.varNumLte != null && p.varNumLte.ToLower().Contains(search.ToLower()) || 
                p.intCnsctvoCrgue != null && p.intCnsctvoCrgue.ToString().ToLower().Contains(search.ToLower()) || 
                p.varCdAuxliar != null && p.varCdAuxliar.ToLower().Contains(search.ToLower()) || 
                p.varNmroFctra != null && p.varNmroFctra.ToLower().Contains(search.ToLower()) ||
                p.fecCmpra != null && Convert.ToDateTime(p.fecCmpra).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varCdCP != null && p.varCdCP.ToLower().Contains(search.ToLower()) ||
                p.fecCp != null && Convert.ToDateTime(p.fecCp).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNmroCP != null && p.varNmroCP.ToLower().Contains(search.ToLower()) ||
                p.fecAprbcionCP != null && Convert.ToDateTime(p.fecAprbcionCP).ToString(GlobalConfig.get().dateFormat).Contains(search.ToLower()) ||
                p.varNmbre != null && p.varNmbre.ToLower().Contains(search.ToLower()) ||
                p.numVlorTtal != null && p.numVlorTtal.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorIva != null && p.numVlorIva.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorRglias != null && p.numVlorRglias.ToString().ToLower().Contains(search.ToLower()) ||
                p.varRegalias != null && p.varRegalias.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionCiudadRglias != null && p.varDscrpcionCiudadRglias.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionDprtmntoRglias != null && p.varDscrpcionDprtmntoRglias.ToLower().Contains(search.ToLower()) ||
                p.bitAnldo != null && p.bitAnldo.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlorRtncionFuente != null && p.numVlorRtncionFuente.ToString().ToLower().Contains(search.ToLower()) ||
                p.numPrcntjeRtncionFte != null && p.numPrcntjeRtncionFte.ToString().ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionAdquirdaTpoCmpra != null && p.varDscrpcionAdquirdaTpoCmpra.ToLower().Contains(search.ToLower()) ||
                p.NmroRslcion != null && p.NmroRslcion.ToLower().Contains(search.ToLower()) ||
                p.varPlcaTtloMnro != null && p.varPlcaTtloMnro.ToLower().Contains(search.ToLower()) ||
                p.bitAcgdosLey1429 != null && p.bitAcgdosLey1429.ToString().ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionTpoCmpra != null && p.varDscrpcionTpoCmpra.ToLower().Contains(search.ToLower())))
                
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