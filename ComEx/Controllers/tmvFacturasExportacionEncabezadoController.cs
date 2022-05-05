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
    public class tmvFacturasExportacionEncabezadoController : Controller
    {
        // GET: tmvFacturasExportacionEncabezado
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetFacturaExportacion(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vFacturasExportacion> lisFacturasExportacion = new List<vFacturasExportacion>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            lisFacturasExportacion = db.vFacturasExportacion.AsNoTracking().Where(x => x.fecFctra.Value.Year == ano).ToList();
                        }
                        else
                        {
                            lisFacturasExportacion = db.vFacturasExportacion.AsNoTracking().ToList();
                        }
                    }

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vFacturasExportacion> data;
                    DTResult<vFacturasExportacion> dtResult;
                    vFacturasExportacion totals;
                    int count;

                    if (param.Columns != null)
                    {
                        dtResult = new vFacturasExportacionResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, lisFacturasExportacion, columnSearch);

                        data = dtResult.data;
                        count = dtResult.recordsTotal;
                        totals = dtResult.totals;
                    }
                    else
                    {
                        IQueryable<vFacturasExportacion> results = lisFacturasExportacion.AsQueryable();

                        lisFacturasExportacion = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = lisFacturasExportacion;
                        count = lisFacturasExportacion.Count();

                        totals = new vFacturasExportacion()
                        {
                            numSbttal = lisFacturasExportacion.Sum(s => s.numSbttal),
                            numVlorFOB = lisFacturasExportacion.Sum(s => s.numVlorFOB),
                            numFltes = lisFacturasExportacion.Sum(s => s.numFltes),
                            numGstos = lisFacturasExportacion.Sum(s => s.numGstos),
                            numSgroUS = lisFacturasExportacion.Sum(s => s.numSgroUS),
                            numPsoBrto = lisFacturasExportacion.Sum(s => s.numPsoBrto),
                            numPsoNto = lisFacturasExportacion.Sum(s => s.numPsoNto)
                        };
                    }

                    DTResult<vFacturasExportacion> resultado = new DTResult<vFacturasExportacion>
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
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionEncabezadoController.GetFacturaExportacion", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmvFacturasExportacionEncabezado/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        ViewBag.listComprador = Funciones.GetListOfSelectListItem(db.tmCompradores.ToList(), "varNmbre", "intIdCmprdor");
                        ViewBag.listExportador = Funciones.GetListOfSelectListItem(db.tmImportadorExportador.ToList(), "varNmbre", "intIdImprtdorExprtdor");
                        ViewBag.listIncoterm = Funciones.GetListOfSelectListItem(db.tmIncoterms.ToList(), "varCdIncterms", "intIdIncterms");
                        ViewBag.listMoneda = Funciones.GetListOfSelectListItem(db.tmMonedas.ToList(), "varCdMnda", "intIdMnda");
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionEncabezadoController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmFacturaExportacion()
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmvFacturasExportacionEncabezado tmvFacturasExportacionEncabezado = new tmvFacturasExportacionEncabezado();

                    tmvFacturasExportacionEncabezado.varNmroAuxliar = Request["varNmroAuxliar"];
                    tmvFacturasExportacionEncabezado.varNmroFctra = Request["varNmroFctra"];
                    tmvFacturasExportacionEncabezado.varDcmntoTrnsprte = Request["varDcmntoTrnsprte"];
                    tmvFacturasExportacionEncabezado.varNmroExprtcion = Request["varNmroExprtcion"];
                    tmvFacturasExportacionEncabezado.fecFctra = Convert.ToDateTime(Request["fecFctra"]);
                    tmvFacturasExportacionEncabezado.fecVncmientoFctra = Convert.ToDateTime(Request["fecVncmientoFctra"]);
                    tmvFacturasExportacionEncabezado.intIdCmprdor = Convert.ToInt32(Request["intIdCmprdor"]);
                    tmvFacturasExportacionEncabezado.intIdImprtdorExprtdor = Convert.ToInt32(Request["intIdImprtdorExprtdor"]);
                    tmvFacturasExportacionEncabezado.bitAnlda = Convert.ToBoolean(Request["bitAnlda"]);
                    tmvFacturasExportacionEncabezado.bitVntaNcional = Convert.ToBoolean(Request["bitVntaNcional"]);
                    tmvFacturasExportacionEncabezado.bitCrtra = true;

                    if (!string.IsNullOrWhiteSpace(Request["intIdIncterms"]))
                    {
                        tmvFacturasExportacionEncabezado.intIdIncterms = Convert.ToInt32(Request["intIdIncterms"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdMnda"]))
                    {
                        tmvFacturasExportacionEncabezado.intIdMnda = Convert.ToInt32(Request["intIdMnda"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numTsaCmbio"]))
                    {
                        tmvFacturasExportacionEncabezado.numTsaCmbio = Convert.ToDecimal(Request["numTsaCmbio"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numSbttal"]))
                    {
                        tmvFacturasExportacionEncabezado.numSbttal = Convert.ToDecimal(Request["numSbttal"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorFOB"]))
                    {
                        tmvFacturasExportacionEncabezado.numVlorFOB = Convert.ToDecimal(Request["numVlorFOB"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numFltes"]))
                    {
                        tmvFacturasExportacionEncabezado.numFltes = Convert.ToDecimal(Request["numFltes"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numGstos"]))
                    {
                        tmvFacturasExportacionEncabezado.numGstos = Convert.ToDecimal(Request["numGstos"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numSgroUS"]))
                    {
                        tmvFacturasExportacionEncabezado.numSgroUS = Convert.ToDecimal(Request["numSgroUS"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numPsoBrto"]))
                    {
                        tmvFacturasExportacionEncabezado.numPsoBrto = Convert.ToDecimal(Request["numPsoBrto"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numPsoNto"]))
                    {
                        tmvFacturasExportacionEncabezado.numPsoNto = Convert.ToDecimal(Request["numPsoNto"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["fecExprtcion"]))
                    {
                        tmvFacturasExportacionEncabezado.fecExprtcion = Convert.ToDateTime(Request["fecExprtcion"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar Compra
                        db.tmvFacturasExportacionEncabezado.Add(tmvFacturasExportacionEncabezado);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación factura de exportación",
                            string.Format("Fac: {0} Export: {1} Lote: {2}", tmvFacturasExportacionEncabezado.varNmroFctra, tmvFacturasExportacionEncabezado.varDcmntoTrnsprte, tmvFacturasExportacionEncabezado.varNmroExprtcion),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvFacturasExportacionDetalle/Create/" + tmvFacturasExportacionEncabezado.intIdFctraExprtcion };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionEncabezadoController.SetTmFacturaExportacion", usr.varLgin);
                    ObjJson = new { Message = ex.Message + " " + ex.InnerException, Success = false };
                    return Json(ObjJson, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return Json(ObjJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    if (string.IsNullOrWhiteSpace(id.ToString()))
                    {
                        ObjJson = new { Message = "Asegúrese de seleccionar una factura", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmvFacturasExportacionEncabezado tmvFacturasExportacionEncabezado = db.tmvFacturasExportacionEncabezado.Where(i => i.intIdFctraExprtcion == id).FirstOrDefault();
                        //Eliminar 
                        db.tmvFacturasExportacionEncabezado.Remove(tmvFacturasExportacionEncabezado);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina factura de exportación",
                            string.Format("Fac: {0} Export: {1} Lote: {2}", tmvFacturasExportacionEncabezado.varNmroFctra, tmvFacturasExportacionEncabezado.varDcmntoTrnsprte, tmvFacturasExportacionEncabezado.varNmroExprtcion),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvFacturasExportacionEncabezado/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionEncabezadoController.Delete", usr.varLgin);
                    ObjJson = new { Message = ex.Message + " " + ex.InnerException, Success = false };
                    return Json(ObjJson, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return Json(ObjJson, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            //return View();
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    if (string.IsNullOrWhiteSpace(id.ToString()))
                    {
                        return RedirectToAction("Index");
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmvFacturasExportacionEncabezado tmvFacturasExportacionEncabezado = db.tmvFacturasExportacionEncabezado.Where(i => i.intIdFctraExprtcion == id).FirstOrDefault();

                        ViewBag.listComprador = Funciones.GetListOfSelectListItem(db.tmCompradores.ToList(), "varNmbre", "intIdCmprdor");
                        ViewBag.listExportador = Funciones.GetListOfSelectListItem(db.tmImportadorExportador.ToList(), "varNmbre", "intIdImprtdorExprtdor");
                        ViewBag.listIncoterm = Funciones.GetListOfSelectListItem(db.tmIncoterms.ToList(), "varCdIncterms", "intIdIncterms");
                        ViewBag.listMoneda = Funciones.GetListOfSelectListItem(db.tmMonedas.ToList(), "varCdMnda", "intIdMnda");

                        return View(tmvFacturasExportacionEncabezado);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionEncabezadoController.Edit", usr.varLgin);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("LogOut", "Home");
            }
        }

        public JsonResult Update()
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        int intIdFctraExprtcion = Convert.ToInt32(Request["intIdFctraExprtcion"]);
                        tmvFacturasExportacionEncabezado tmvFacturasExportacionEncabezado = db.tmvFacturasExportacionEncabezado.Where(i => i.intIdFctraExprtcion == intIdFctraExprtcion).FirstOrDefault();

                        tmvFacturasExportacionEncabezado.varNmroAuxliar = Request["varNmroAuxliar"];
                        tmvFacturasExportacionEncabezado.varNmroFctra = Request["varNmroFctra"];
                        tmvFacturasExportacionEncabezado.varDcmntoTrnsprte = Request["varDcmntoTrnsprte"];
                        tmvFacturasExportacionEncabezado.varNmroExprtcion = Request["varNmroExprtcion"];
                        tmvFacturasExportacionEncabezado.fecFctra = Convert.ToDateTime(Request["fecFctra"]);
                        tmvFacturasExportacionEncabezado.fecVncmientoFctra = Convert.ToDateTime(Request["fecVncmientoFctra"]);
                        tmvFacturasExportacionEncabezado.intIdCmprdor = Convert.ToInt32(Request["intIdCmprdor"]);
                        tmvFacturasExportacionEncabezado.intIdImprtdorExprtdor = Convert.ToInt32(Request["intIdImprtdorExprtdor"]);
                        tmvFacturasExportacionEncabezado.bitAnlda = Convert.ToBoolean(Request["bitAnlda"]);
                        tmvFacturasExportacionEncabezado.bitVntaNcional = Convert.ToBoolean(Request["bitVntaNcional"]);
                        tmvFacturasExportacionEncabezado.bitCrtra = true;

                        if (!string.IsNullOrWhiteSpace(Request["intIdIncterms"]))
                        {
                            tmvFacturasExportacionEncabezado.intIdIncterms = Convert.ToInt32(Request["intIdIncterms"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdMnda"]))
                        {
                            tmvFacturasExportacionEncabezado.intIdMnda = Convert.ToInt32(Request["intIdMnda"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numTsaCmbio"]))
                        {
                            tmvFacturasExportacionEncabezado.numTsaCmbio = Convert.ToDecimal(Request["numTsaCmbio"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numSbttal"]))
                        {
                            tmvFacturasExportacionEncabezado.numSbttal = Convert.ToDecimal(Request["numSbttal"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorFOB"]))
                        {
                            tmvFacturasExportacionEncabezado.numVlorFOB = Convert.ToDecimal(Request["numVlorFOB"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numFltes"]))
                        {
                            tmvFacturasExportacionEncabezado.numFltes = Convert.ToDecimal(Request["numFltes"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numGstos"]))
                        {
                            tmvFacturasExportacionEncabezado.numGstos = Convert.ToDecimal(Request["numGstos"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numSgroUS"]))
                        {
                            tmvFacturasExportacionEncabezado.numSgroUS = Convert.ToDecimal(Request["numSgroUS"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numPsoBrto"]))
                        {
                            tmvFacturasExportacionEncabezado.numPsoBrto = Convert.ToDecimal(Request["numPsoBrto"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numPsoNto"]))
                        {
                            tmvFacturasExportacionEncabezado.numPsoNto = Convert.ToDecimal(Request["numPsoNto"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecExprtcion"]))
                        {
                            tmvFacturasExportacionEncabezado.fecExprtcion = Convert.ToDateTime(Request["fecExprtcion"]);
                        }

                        //Actualizar
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de factura de exportación",
                            string.Format("Fac: {0} Export: {1} Lote: {2}", tmvFacturasExportacionEncabezado.varNmroFctra, tmvFacturasExportacionEncabezado.varDcmntoTrnsprte, tmvFacturasExportacionEncabezado.varNmroExprtcion),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmvFacturasExportacionEncabezado/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionEncabezadoController.Update", usr.varLgin);
                    ObjJson = new { Message = ex.Message + " " + ex.InnerException, Success = false };
                    return Json(ObjJson, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return Json(ObjJson, JsonRequestBehavior.AllowGet);
        }
    }
}