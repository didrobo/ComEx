using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vInformeUIAFCompVentResultSet
    {
        public DTResult<vInformeUIAFCompVent> GetResult(string search, string sortOrder, int start, int length, List<vInformeUIAFCompVent> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vInformeUIAFCompVent> resultados = FilterResult(search, dtResult, columnFilters);
            int cont = resultados.Count();
            List<vInformeUIAFCompVent> datos = FilterResult(search, dtResult, columnFilters).ToList();
            vInformeUIAFCompVent totales = new vInformeUIAFCompVent()
            {
                Valor = datos.Sum(s => s.Valor),
                numCntdadFctra = datos.Sum(s => s.numCntdadFctra)
            };

            DTResult<vInformeUIAFCompVent> resultado = new DTResult<vInformeUIAFCompVent>
            {
                data = resultados.SortBy(sortOrder).Skip(start).Take(length).ToList(),
                recordsTotal = cont,
                totals = totales
            };

            return resultado;
        }

        private IQueryable<vInformeUIAFCompVent> FilterResult(string search, List<vInformeUIAFCompVent> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vInformeUIAFCompVent> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.intIdFctra.ToString().ToLower().Contains(search.ToLower()) ||
                p.FecTransaccion != null && p.FecTransaccion.ToLower().Contains(search.ToLower()) ||
                p.Valor.ToString().ToLower().Contains(search.ToLower()) ||
                p.TipoOperacion != null && p.TipoOperacion.ToLower().Contains(search.ToLower()) ||
                p.TipoDocumento != null && p.TipoDocumento.ToLower().Contains(search.ToLower()) ||
                p.Identificacion != null && p.Identificacion.ToLower().Contains(search.ToLower()) ||
                p.PrimerApellido != null && p.PrimerApellido.ToLower().Contains(search.ToLower()) ||
                p.SegundoApellido != null && p.SegundoApellido.ToLower().Contains(search.ToLower()) ||
                p.PrimerNombre != null && p.PrimerNombre.ToLower().Contains(search.ToLower()) ||
                p.SegundoNombre != null && p.SegundoNombre.ToLower().Contains(search.ToLower()) ||
                p.RazonSocial != null && p.RazonSocial.ToLower().Contains(search.ToLower()) ||
                p.varCd != null && p.varCd.ToLower().Contains(search.ToLower()) ||
                p.numCntdadFctra.ToString().ToLower().Contains(search.ToLower()) ||
                p.FormaPago.ToString().ToLower().Contains(search.ToLower()) ||
                p.Detalles != null && p.Detalles.ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}