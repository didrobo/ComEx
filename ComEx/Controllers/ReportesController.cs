using ComEx.Context;
using ComEx.Helpers;
using ComEx.Models;
using ComEx.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class ReportesController : Controller
    {
        // GET: Reportes
        public ActionResult Compras()
        {
            return View();
        }

        public ActionResult InformeAnualCI()
        {
            return View();
        }

        public ActionResult InformeUIAFCompVent()
        {
            return View();
        }

        public ActionResult InformeUIAFImpExp()
        {
            return View();
        }

        public ActionResult InventarioCPs()
        {
            return View();
        }

        public JsonResult GetComprasReporte(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vReporteOrdenesCompraCI> lisComprasReporte = new List<vReporteOrdenesCompraCI>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            lisComprasReporte = db.vReporteOrdenesCompraCI.AsNoTracking().Where(x => x.fecCmpra.Value.Year == ano).ToList();
                        }
                        else
                        {
                            lisComprasReporte = db.vReporteOrdenesCompraCI.AsNoTracking().ToList();
                        }                        
                    }

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vReporteOrdenesCompraCI> data;
                    DTResult<vReporteOrdenesCompraCI> dtResult;
                    vReporteOrdenesCompraCI totals;
                    int count;

                    if (param.Columns != null)
                    {
                        dtResult = new vReporteOrdenesCompraCIResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, lisComprasReporte, columnSearch);

                        data = dtResult.data;
                        count = dtResult.recordsTotal;
                        totals = dtResult.totals;
                    }
                    else
                    {
                        IQueryable<vReporteOrdenesCompraCI> results = lisComprasReporte.AsQueryable();

                        lisComprasReporte = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = lisComprasReporte;
                        count = lisComprasReporte.Count();

                        totals = new vReporteOrdenesCompraCI()
                        {
                            numCntdadFctra = lisComprasReporte.Sum(s => s.numCntdadFctra),
                            numFnos = lisComprasReporte.Sum(s => s.numFnos),
                            numFnosAG = lisComprasReporte.Sum(s => s.numFnosAG),
                            numFnosPT = lisComprasReporte.Sum(s => s.numFnosPT),
                            numRglias = lisComprasReporte.Sum(s => s.numRglias),
                            numRgliasAG = lisComprasReporte.Sum(s => s.numRgliasAG),
                            numRgliasPT = lisComprasReporte.Sum(s => s.numRgliasPT),
                            numVlorRtncionFuente = lisComprasReporte.Sum(s => s.numVlorRtncionFuente),
                            numValorPagar = lisComprasReporte.Sum(s => s.numValorPagar),
                            numValorAntesRetencion = lisComprasReporte.Sum(s => s.numValorAntesRetencion),
                            numSbttal = lisComprasReporte.Sum(s => s.numSbttal),
                            numVlorFOB = lisComprasReporte.Sum(s => s.numVlorFOB)
                        };
                    }

                    DTResult<vReporteOrdenesCompraCI> resultado = new DTResult<vReporteOrdenesCompraCI>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count,
                        totals = totals
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = 500000000;
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ReportesController.GetComprasReporte", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GetInformeAnualCIReporte(DTParameters param)
        {
            JsonResult result = new JsonResult();
            
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vInformeAnualCi> lisInformeAnualCI = new List<vInformeAnualCi>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        int ano = Convert.ToInt32(param.parametros[0]);

                        lisInformeAnualCI = db.vInformeAnualCi.AsNoTracking().Where(x => x.fecCP.Value.Year == ano).ToList();
                    }

                    //Obtiene valor total FOB
                    decimal valorTotalFOB = lisInformeAnualCI.Select(s => new
                    {
                        Key = s.varNmroDEX,
                        Value = s.numVlrTtalFobUSD
                    }).Distinct().Sum(r => r.Value); 


                    //Se duplican los datos con valores negativos (CPs Anulados)
                    lisInformeAnualCI.AddRange(
                        lisInformeAnualCI.Where(x => x.numVlorCp < 0).Select(s => new vInformeAnualCi
                        {
                            intNum = s.intNum + 10000,
                            varCdCP = s.varCdCP,
                            fecBmstre = s.fecBmstre,
                            fecCP = s.fecCP,
                            intNumItem = s.intNumItem,
                            varNmroFctra = s.varNmroFctra,
                            fecCmpra = s.fecCmpra,
                            numVlorCp = s.numVlorCp * (-1),
                            numVlorTtalAnual = s.numVlorTtalAnual,
                            varPrdctoSinTrnsfrmar = s.varPrdctoSinTrnsfrmar,
                            varMtriaPrima = s.varMtriaPrima,
                            varSrvcioIntrmdioPrdccion = s.varSrvcioIntrmdioPrdccion,
                            fecEnvioCP = s.fecEnvioCP,
                            varNmroDEX = s.varNmroDEX,
                            fecAprbcionDEX = s.fecAprbcionDEX,
                            varCdAduana = s.varCdAduana,
                            fecEmbrque = s.fecEmbrque,
                            numVlrTtalFobUSD = s.numVlrTtalFobUSD,
                            numVlrFobUSD = s.numVlrFobUSD
                        }).ToList()
                    );

                    //Se reasigna el número de ítem
                    int item = 0;
                    lisInformeAnualCI = lisInformeAnualCI.OrderBy(x => x.fecCP).ThenBy(t => t.varCdCP).Select(s => new vInformeAnualCi
                    {
                        intNum = ++item,
                        varCdCP = s.varCdCP,
                        fecBmstre = s.fecBmstre,
                        fecCP = s.fecCP,
                        intNumItem = s.intNumItem,
                        varNmroFctra = s.varNmroFctra,
                        fecCmpra = s.fecCmpra,
                        numVlorCp = s.numVlorCp,
                        numVlorTtalAnual = s.numVlorTtalAnual,
                        varPrdctoSinTrnsfrmar = s.varPrdctoSinTrnsfrmar,
                        varMtriaPrima = s.varMtriaPrima,
                        varSrvcioIntrmdioPrdccion = s.varSrvcioIntrmdioPrdccion,
                        fecEnvioCP = s.fecEnvioCP,
                        varNmroDEX = s.varNmroDEX,
                        fecAprbcionDEX = s.fecAprbcionDEX,
                        varCdAduana = s.varCdAduana,
                        fecEmbrque = s.fecEmbrque,
                        numVlrTtalFobUSD = 0,
                        numVlrFobUSD = s.numVlrFobUSD
                    }).ToList();

                    //Se llena el valor total FOB
                    lisInformeAnualCI.Where(x => x.intNum == 1).FirstOrDefault().numVlrTtalFobUSD = valorTotalFOB;

                    //Obtiene el valor total CP
                    decimal valorTotalCP = lisInformeAnualCI.Sum(s => s.numVlorCp);

                    //Se llena el valor total CP
                    lisInformeAnualCI.Where(x => x.intNum == 1).FirstOrDefault().numVlorTtalAnual = valorTotalCP;

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vInformeAnualCi> data;
                    int count;

                    if (param.Columns != null)
                    {
                        data = new vInformeAnualCiResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, lisInformeAnualCI, columnSearch);
                        count = new vInformeAnualCiResultSet().Count(param.Search.Value, lisInformeAnualCI, columnSearch);
                    }
                    else
                    {
                        IQueryable<vInformeAnualCi> results = lisInformeAnualCI.AsQueryable();

                        lisInformeAnualCI = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = lisInformeAnualCI;
                        count = lisInformeAnualCI.Count();
                    }

                    //Se ordena la información
                    data = data.OrderBy(x => x.intNum).ToList();

                    DTResult<vInformeAnualCi> resultado = new DTResult<vInformeAnualCi>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = 500000000;
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ReportesController.GetInformeAnualCIReporte", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GetInformeUIAFCompVent(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vInformeUIAFCompVent> listUIAFCompVent = new List<vInformeUIAFCompVent>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        int ano = Convert.ToInt32(param.parametros[0]);
                        int cuatrimestre = Convert.ToInt32(param.parametros[1]);

                        listUIAFCompVent = db.vInformeUIAFCompVent.AsNoTracking().Where(x => x.fecha.Value.Year == ano && Math.Ceiling((x.fecha.Value.Month) / 4.0) == cuatrimestre).ToList();
                    }

                    //Se reasigna el número de ítem
                    int item = 0;
                    listUIAFCompVent = listUIAFCompVent.OrderBy(x => x.FormaPago).ThenBy(t => t.Cargue).ThenBy(t => t.varCdAuxliar).Select(s => new vInformeUIAFCompVent
                    {
                        intIdFctra = ++item,
                        FecTransaccion = s.FecTransaccion,
                        Valor = s.Valor,
                        TipoOperacion = s.TipoOperacion,
                        TipoDocumento = s.TipoDocumento,
                        Identificacion = s.Identificacion,
                        PrimerApellido = s.PrimerApellido,
                        SegundoApellido = s.SegundoApellido,
                        PrimerNombre = s.PrimerNombre,
                        SegundoNombre = s.SegundoNombre,
                        RazonSocial = s.RazonSocial,
                        varCd = s.varCd,
                        numCntdadFctra = s.numCntdadFctra,
                        FormaPago = s.FormaPago,
                        Detalles = s.Detalles,
                        Lote = s.Lote,
                        varCdAuxliar = s.varCdAuxliar,
                        Cargue = s.Cargue,
                        fecha = s.fecha
                    }).ToList();

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vInformeUIAFCompVent> data;
                    DTResult<vInformeUIAFCompVent> dtResult;
                    vInformeUIAFCompVent totals;
                    int count;

                    if (param.Columns != null)
                    {
                        dtResult = new vInformeUIAFCompVentResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, listUIAFCompVent, columnSearch);

                        data = dtResult.data;
                        count = dtResult.recordsTotal;
                        totals = dtResult.totals;
                    }
                    else
                    {
                        IQueryable<vInformeUIAFCompVent> results = listUIAFCompVent.AsQueryable();

                        listUIAFCompVent = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = listUIAFCompVent;
                        count = listUIAFCompVent.Count();

                        totals = new vInformeUIAFCompVent()
                        {
                            Valor = listUIAFCompVent.Sum(s => s.Valor),
                            numCntdadFctra = listUIAFCompVent.Sum(s => s.numCntdadFctra)
                        };
                    }

                    //Se ordena la información
                    data = data.OrderBy(x => x.intIdFctra).ToList();

                    DTResult<vInformeUIAFCompVent> resultado = new DTResult<vInformeUIAFCompVent>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count,
                        totals = totals
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = 500000000;
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ReportesController.GetInformeUIAFCompVent", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GetInformeUIAFImpExp(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vInformeUIAFImpExp> listUIAFImpExp = new List<vInformeUIAFImpExp>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        int ano = Convert.ToInt32(param.parametros[0]);
                        int cuatrimestre = Convert.ToInt32(param.parametros[1]);

                        listUIAFImpExp = db.vInformeUIAFImpExp.AsNoTracking().Where(x => x.fecFctra.Value.Year == ano && Math.Ceiling((x.fecFctra.Value.Month) / 4.0) == cuatrimestre).ToList();
                    }

                    //Se reasigna el número de ítem
                    int item = 0;
                    listUIAFImpExp = listUIAFImpExp.OrderBy(x => x.intConsec).Select(s => new vInformeUIAFImpExp
                    {
                        intConsec = ++item,
                        fecha = s.fecha,
                        numVlrfob = s.numVlrfob,
                        numVlrfob2 = s.numVlrfob2,
                        varTipomon = s.varTipomon,
                        varTipoDoc = s.varTipoDoc,
                        varNit = s.varNit,
                        varPrimerApellido = s.varPrimerApellido,
                        varSegundoApellido = s.varSegundoApellido,
                        varPrimerNombre = s.varPrimerNombre,
                        varSegundoNombre = s.varSegundoNombre,
                        varRazonSocial = s.varRazonSocial,
                        varCoddane = s.varCoddane,
                        intTipotra = s.intTipotra,
                        varNitdetalle = s.varNitdetalle,
                        varNit2 = s.varNit2,
                        varPrimerApellido1 = s.varPrimerApellido1,
                        varSegundoApellido1 = s.varSegundoApellido1,
                        varPrimerNombre1 = s.varPrimerNombre1,
                        varSegundoNombre1 = s.varSegundoNombre1,
                        varRazonSocial1 = s.varRazonSocial1,
                        varPais = s.varPais,
                        varCiudad = s.varCiudad,
                        varPartidaa = s.varPartidaa,
                        varDex = s.varDex,
                        intFormap = s.intFormap,
                        varDetalle = s.varDetalle,
                        fecFctra = s.fecFctra
                    }).ToList();

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vInformeUIAFImpExp> data;
                    DTResult<vInformeUIAFImpExp> dtResult;
                    vInformeUIAFImpExp totals;
                    int count;

                    if (param.Columns != null)
                    {
                        dtResult = new vInformeUIAFImpExpResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, listUIAFImpExp, columnSearch);

                        data = dtResult.data;
                        count = dtResult.recordsTotal;
                        totals = dtResult.totals;
                    }
                    else
                    {
                        IQueryable<vInformeUIAFImpExp> results = listUIAFImpExp.AsQueryable();

                        listUIAFImpExp = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = listUIAFImpExp;
                        count = listUIAFImpExp.Count();

                        totals = new vInformeUIAFImpExp()
                        {
                            numVlrfob = listUIAFImpExp.Sum(s => s.numVlrfob),
                            numVlrfob2 = listUIAFImpExp.Sum(s => s.numVlrfob2)
                        };
                    }

                    DTResult<vInformeUIAFImpExp> resultado = new DTResult<vInformeUIAFImpExp>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count,
                        totals = totals
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = 500000000;
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ReportesController.GetInformeUIAFImpExp", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GetInventarioCPs(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vInventarioCp> listInventarioCPs = new List<vInventarioCp>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        DateTime desde = Convert.ToDateTime(param.parametros[0]);
                        DateTime hasta = Convert.ToDateTime(param.parametros[1]);

                        listInventarioCPs = db.vInventarioCp.AsNoTracking().Where(x => x.fecCP.Value >= desde && x.fecCP.Value <= hasta).ToList();
                    }

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vInventarioCp> data;
                    DTResult<vInventarioCp> dtResult;
                    vInventarioCp totals;
                    int count;

                    if (param.Columns != null)
                    {
                        dtResult = new vInventarioCpResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, listInventarioCPs, columnSearch);

                        data = dtResult.data;
                        count = dtResult.recordsTotal;
                        totals = dtResult.totals;
                    }
                    else
                    {
                        IQueryable<vInventarioCp> results = listInventarioCPs.AsQueryable();

                        listInventarioCPs = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = listInventarioCPs;
                        count = listInventarioCPs.Count();

                        totals = new vInventarioCp()
                        {
                            numCntdadCp = listInventarioCPs.Sum(s => s.numCntdadCp),
                            numCntdadCnsmda = listInventarioCPs.Sum(s => s.numCntdadCnsmda),
                            numCntCnsmdaDEX = listInventarioCPs.Sum(s => s.numCntCnsmdaDEX),
                            numCntdadPndienteCP = listInventarioCPs.Sum(s => s.numCntdadPndienteCP),
                            numVlorCpPorCI = listInventarioCPs.Sum(s => s.numVlorCpPorCI),
                            numVlorCnsmdoCpPorCI = listInventarioCPs.Sum(s => s.numVlorCnsmdoCpPorCI),
                            numVlorPndienteCpPorCI = listInventarioCPs.Sum(s => s.numVlorPndienteCpPorCI)
                        };
                    }

                    DTResult<vInventarioCp> resultado = new DTResult<vInventarioCp>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count,
                        totals = totals
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = 500000000;
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ReportesController.GetInventarioCPs", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }
    }
}