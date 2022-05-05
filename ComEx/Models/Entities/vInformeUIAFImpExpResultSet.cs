using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ComEx.Models.Entities
{
    public class vInformeUIAFImpExpResultSet
    {
        public DTResult<vInformeUIAFImpExp> GetResult(string search, string sortOrder, int start, int length, List<vInformeUIAFImpExp> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vInformeUIAFImpExp> resultados = FilterResult(search, dtResult, columnFilters);
            int cont = resultados.Count();
            List<vInformeUIAFImpExp> datos = FilterResult(search, dtResult, columnFilters).ToList();
            vInformeUIAFImpExp totales = new vInformeUIAFImpExp()
            {
                numVlrfob = datos.Sum(s => s.numVlrfob),
                numVlrfob2 = datos.Sum(s => s.numVlrfob2)
            };

            DTResult<vInformeUIAFImpExp> resultado = new DTResult<vInformeUIAFImpExp>
            {
                data = resultados.SortBy(sortOrder).Skip(start).Take(length).ToList(),
                recordsTotal = cont,
                totals = totales
            };

            return resultado;
        }

        private IQueryable<vInformeUIAFImpExp> FilterResult(string search, List<vInformeUIAFImpExp> dtResult, List<DTFilters> columnFilters)
        {
            IQueryable<vInformeUIAFImpExp> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (
                p.intConsec.ToString().ToLower().Contains(search.ToLower()) ||
                p.fecha != null && p.fecha.ToLower().Contains(search.ToLower()) ||
                p.numVlrfob.ToString().ToLower().Contains(search.ToLower()) ||
                p.numVlrfob2.ToString().ToLower().Contains(search.ToLower()) ||
                p.varTipomon != null && p.varTipomon.ToLower().Contains(search.ToLower()) ||
                p.varTipoDoc != null && p.varTipoDoc.ToLower().Contains(search.ToLower()) ||
                p.varNit != null && p.varNit.ToLower().Contains(search.ToLower()) ||
                p.varPrimerApellido != null && p.varPrimerApellido.ToLower().Contains(search.ToLower()) ||
                p.varSegundoApellido != null && p.varSegundoApellido.ToLower().Contains(search.ToLower()) ||
                p.varPrimerNombre != null && p.varPrimerNombre.ToLower().Contains(search.ToLower()) ||
                p.varSegundoNombre != null && p.varSegundoNombre.ToLower().Contains(search.ToLower()) ||
                p.varRazonSocial != null && p.varRazonSocial.ToLower().Contains(search.ToLower()) ||
                p.varCoddane != null && p.varCoddane.ToLower().Contains(search.ToLower()) ||
                p.intTipotra.ToString().ToLower().Contains(search.ToLower()) ||
                p.varNitdetalle != null && p.varNitdetalle.ToLower().Contains(search.ToLower()) ||
                p.varNit2 != null && p.varNit2.ToLower().Contains(search.ToLower()) ||
                p.varPrimerApellido1 != null && p.varPrimerApellido1.ToLower().Contains(search.ToLower()) ||
                p.varSegundoApellido1 != null && p.varSegundoApellido1.ToLower().Contains(search.ToLower()) ||
                p.varPrimerNombre1 != null && p.varPrimerNombre1.ToLower().Contains(search.ToLower()) ||
                p.varSegundoNombre1 != null && p.varSegundoNombre1.ToLower().Contains(search.ToLower()) ||
                p.varRazonSocial1 != null && p.varRazonSocial1.ToLower().Contains(search.ToLower()) ||
                p.varPais != null && p.varPais.ToLower().Contains(search.ToLower()) ||
                p.varCiudad != null && p.varCiudad.ToLower().Contains(search.ToLower()) ||
                p.varPartidaa != null && p.varPartidaa.ToLower().Contains(search.ToLower()) ||
                p.varDex != null && p.varDex.ToLower().Contains(search.ToLower()) ||
                p.intFormap.ToString().ToLower().Contains(search.ToLower()) ||
                p.varDetalle != null && p.varDetalle.ToLower().Contains(search.ToLower())))
                );

            results = Funciones.FiltrarListDataTables(results, columnFilters);

            return results;
        }
    }
}