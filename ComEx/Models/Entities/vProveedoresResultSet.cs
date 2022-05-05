using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vProveedoresResultSet
    {
        public List<vProveedores> GetResult(string search, string sortOrder, int start, int length, List<vProveedores> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<vProveedores> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<vProveedores> FilterResult(string search, List<vProveedores> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vProveedores> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.varCdPrvdor != null && p.varCdPrvdor.ToLower().Contains(search.ToLower()) ||                
                p.varNmbre != null && p.varNmbre.ToLower().Contains(search.ToLower()) ||
                p.varDrccion != null && p.varDrccion.ToLower().Contains(search.ToLower()) ||
                p.varTlfno != null && p.varTlfno.ToLower().Contains(search.ToLower()) ||
                p.varCdPais != null && p.varCdPais.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionPais != null && p.varDscrpcionPais.ToLower().Contains(search.ToLower()) ||
                p.bitPrveedorNcional != null && p.bitPrveedorNcional.ToString().ToLower().Contains(search.ToLower()) ||
                p.varCdCiudad != null && p.varCdCiudad.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionCiudad != null && p.varDscrpcionCiudad.ToLower().Contains(search.ToLower()) ||
                p.varCdDprtmnto != null && p.varCdDprtmnto.ToLower().Contains(search.ToLower()) ||
                p.varDscrpcionDprtmnto != null && p.varDscrpcionDprtmnto.ToLower().Contains(search.ToLower()) ||
                p.varEmail != null && p.varEmail.ToLower().Contains(search.ToLower()) ||
                p.varNitPrvdor != null && p.varNitPrvdor.ToLower().Contains(search.ToLower()) ||
                p.varNbreRgmen != null && p.varNbreRgmen.ToLower().Contains(search.ToLower()) ||
                p.bitAcgdosLey1429 != null && p.bitAcgdosLey1429.ToString().ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}