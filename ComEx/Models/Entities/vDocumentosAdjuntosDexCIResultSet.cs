using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
namespace ComEx.Models.Entities
{
    public class vDocumentosAdjuntosDexCIResultSet
    {
        public List<vDocumentosAdjuntosDexCIComex> GetResult(string search, string sortOrder, int start, int length, List<vDocumentosAdjuntosDexCIComex> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<vDocumentosAdjuntosDexCIComex> dtResult, List<DTFilters> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<vDocumentosAdjuntosDexCIComex> FilterResult(string search, List<vDocumentosAdjuntosDexCIComex> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vDocumentosAdjuntosDexCIComex> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.intId.ToString().ToLower().Contains(search.ToLower()) ||
                p.intIdDEX.ToString().ToLower().Contains(search.ToLower()) ||
                p.varNumDoc != null && p.varNumDoc.ToLower().Contains(search.ToLower()) ||
                p.varRtaArchvoAdjnto != null && p.varRtaArchvoAdjnto.ToLower().Contains(search.ToLower()) ||
                p.varTipoDocumento != null && p.varTipoDocumento.ToLower().Contains(search.ToLower()) ||
                p.intIdCP.ToString().ToLower().Contains(search.ToLower()) ||
                p.varExiste != null && p.varExiste.ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}