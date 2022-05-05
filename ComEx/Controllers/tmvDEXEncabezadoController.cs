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
    public class tmvDEXEncabezadoController : Controller
    {
        // GET: tmvDEXEncabezado
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDex(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vDEX> lisDex = new List<vDEX>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            lisDex = db.vDEX.AsNoTracking().Where(x => x.fecAuxliar.Value.Year == ano).ToList();
                        }
                        else
                        {
                            lisDex = db.vDEX.AsNoTracking().ToList();
                        }
                    }

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vDEX> data;
                    DTResult<vDEX> dtResult;
                    vDEX totals;
                    int count;

                    if (param.Columns != null)
                    {
                        dtResult = new vDEXResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, lisDex, columnSearch);

                        data = dtResult.data;
                        count = dtResult.recordsTotal;
                        totals = dtResult.totals;
                    }
                    else
                    {
                        IQueryable<vDEX> results = lisDex.AsQueryable();

                        lisDex = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = lisDex;
                        count = lisDex.Count();

                        totals = new vDEX()
                        {
                            numVlorFOB = lisDex.Sum(s => s.numVlorFOB),
                            numFltes = lisDex.Sum(s => s.numFltes),
                            numSgro = lisDex.Sum(s => s.numSgro),
                            numOtrosGstos = lisDex.Sum(s => s.numOtrosGstos),
                            numTtalPsoBrto = lisDex.Sum(s => s.numTtalPsoBrto),
                            numTtalPsoNto = lisDex.Sum(s => s.numTtalPsoNto),
                            numTtalSries = lisDex.Sum(s => s.numTtalSries),
                            numBltos = lisDex.Sum(s => s.numBltos)
                        };
                    }

                    DTResult<vDEX> resultado = new DTResult<vDEX>
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
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXEncabezadoController.GetDex", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmvDEXEncabezado/Create
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
                        ViewBag.listAduana = Funciones.GetListOfSelectListItem(db.tmAduanas.ToList(), "varDscrpcionAduana", "intIdAduana");
                        ViewBag.listDeclarante = Funciones.GetListOfSelectListItem(db.tmTerceros.ToList(), "varNmbre", "intIdTrcro");
                        ViewBag.listLugarEmbarque = Funciones.GetListOfSelectListItem(db.tmLugaresEmbarque.ToList(), "varNmbreLgarEmbrque", "intIdLgarEmbrque");
                        ViewBag.listTransporte = Funciones.GetListOfSelectListItem(db.tmTransportes.ToList(), "varDscrpcionTrnsprte", "intIdTrnsprte");
                        ViewBag.listPlanCI = Funciones.GetListOfSelectListItem(db.tmPlanesCI.ToList(), "varDscrpcionPlanCI", "intIdPlanCI");
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXEncabezadoController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmvDEXEncabezado()
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmvDEXEncabezado tmvDEXEncabezado = new tmvDEXEncabezado();

                    tmvDEXEncabezado.varCdAuxliar = Request["varCdAuxliar"];
                    tmvDEXEncabezado.varNmroDEX = Request["varNmroDEX"];
                    tmvDEXEncabezado.fecAuxliar = Convert.ToDateTime(Request["fecAuxliar"]);
                    tmvDEXEncabezado.intIdCmprdor = Convert.ToInt32(Request["intIdCmprdor"]);
                    tmvDEXEncabezado.intIdImprtdorExprtdor = Convert.ToInt32(Request["intIdImprtdorExprtdor"]);
                    tmvDEXEncabezado.numNumroAcptcion = Request["numNumroAcptcion"];
                    tmvDEXEncabezado.varSlctudAtrzcionEmbrque = Request["varSlctudAtrzcionEmbrque"];
                    tmvDEXEncabezado.varMnfiestoCrga = Request["varMnfiestoCrga"];
                    tmvDEXEncabezado.varNmroFrmlrioAntrior = Request["varNmroFrmlrioAntrior"];
                    tmvDEXEncabezado.varNmroDcmntoTrnsprte = Request["varNmroDcmntoTrnsprte"];
                    tmvDEXEncabezado.varCmntrio = Request["varCmntrio"];

                    if (!string.IsNullOrWhiteSpace(Request["fecAprbcionDEX"]))
                    {
                        tmvDEXEncabezado.fecAprbcionDEX = Convert.ToDateTime(Request["fecAprbcionDEX"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["fecEmbrque"]))
                    {
                        tmvDEXEncabezado.fecEmbrque = Convert.ToDateTime(Request["fecEmbrque"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdAduana"]))
                    {
                        tmvDEXEncabezado.intIdAduana = Convert.ToInt32(Request["intIdAduana"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdLgarEmbrque"]))
                    {
                        tmvDEXEncabezado.intIdLgarEmbrque = Convert.ToInt32(Request["intIdLgarEmbrque"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdDclrnte"]))
                    {
                        tmvDEXEncabezado.intIdDclrnte = Convert.ToInt32(Request["intIdDclrnte"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numExprtcionesUSD"]))
                    {
                        tmvDEXEncabezado.numExprtcionesUSD = Convert.ToDecimal(Request["numExprtcionesUSD"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorFOB"]))
                    {
                        tmvDEXEncabezado.numVlorFOB = Convert.ToDecimal(Request["numVlorFOB"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numFltes"]))
                    {
                        tmvDEXEncabezado.numFltes = Convert.ToDecimal(Request["numFltes"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numOtrosGstos"]))
                    {
                        tmvDEXEncabezado.numOtrosGstos = Convert.ToDecimal(Request["numOtrosGstos"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numSgro"]))
                    {
                        tmvDEXEncabezado.numSgro = Convert.ToDecimal(Request["numSgro"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorReintgrarUSD"]))
                    {
                        tmvDEXEncabezado.numVlorReintgrarUSD = Convert.ToDecimal(Request["numVlorReintgrarUSD"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorAgrgdoNcional"]))
                    {
                        tmvDEXEncabezado.numVlorAgrgdoNcional = Convert.ToDecimal(Request["numVlorAgrgdoNcional"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numTtalSries"]))
                    {
                        tmvDEXEncabezado.numTtalSries = Convert.ToDecimal(Request["numTtalSries"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numBltos"]))
                    {
                        tmvDEXEncabezado.numBltos = Convert.ToDecimal(Request["numBltos"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numTtalPsoBrto"]))
                    {
                        tmvDEXEncabezado.numTtalPsoBrto = Convert.ToDecimal(Request["numTtalPsoBrto"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numTtalPsoNto"]))
                    {
                        tmvDEXEncabezado.numTtalPsoNto = Convert.ToDecimal(Request["numTtalPsoNto"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["fecAcptcion"]))
                    {
                        tmvDEXEncabezado.fecAcptcion = Convert.ToDateTime(Request["fecAcptcion"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["fecAtrzcionEmbrque"]))
                    {
                        tmvDEXEncabezado.fecAtrzcionEmbrque = Convert.ToDateTime(Request["fecAtrzcionEmbrque"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["fecMnfiestoCrga"]))
                    {
                        tmvDEXEncabezado.fecMnfiestoCrga = Convert.ToDateTime(Request["fecMnfiestoCrga"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdTrnsprte"]))
                    {
                        tmvDEXEncabezado.intIdTrnsprte = Convert.ToInt32(Request["intIdTrnsprte"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdPlanCI"]))
                    {
                        tmvDEXEncabezado.intIdPlanCI = Convert.ToInt32(Request["intIdPlanCI"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["fecDcmntoTrnsprte"]))
                    {
                        tmvDEXEncabezado.fecDcmntoTrnsprte = Convert.ToDateTime(Request["fecDcmntoTrnsprte"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar
                        db.tmvDEXEncabezado.Add(tmvDEXEncabezado);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación DEX",
                            string.Format("Aux: {0} ", tmvDEXEncabezado.varCdAuxliar),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvDEXDetalle/Create/" + tmvDEXEncabezado.intIdDEX };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXEncabezadoController.SetTmvDEXEncabezado", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar un DEX", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmvDEXEncabezado tmvDEXEncabezado = db.tmvDEXEncabezado.Where(i => i.intIdDEX == id).FirstOrDefault();
                        //Eliminar 
                        db.tmvDEXEncabezado.Remove(tmvDEXEncabezado);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina DEX",
                            string.Format("Aux: {0} ", tmvDEXEncabezado.varCdAuxliar),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvDEXEncabezado/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXEncabezadoController.Delete", usr.varLgin);
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
                        tmvDEXEncabezado tmvDEXEncabezado = db.tmvDEXEncabezado.Where(i => i.intIdDEX == id).FirstOrDefault();

                        ViewBag.listComprador = Funciones.GetListOfSelectListItem(db.tmCompradores.ToList(), "varNmbre", "intIdCmprdor");
                        ViewBag.listExportador = Funciones.GetListOfSelectListItem(db.tmImportadorExportador.ToList(), "varNmbre", "intIdImprtdorExprtdor");
                        ViewBag.listAduana = Funciones.GetListOfSelectListItem(db.tmAduanas.ToList(), "varDscrpcionAduana", "intIdAduana");
                        ViewBag.listDeclarante = Funciones.GetListOfSelectListItem(db.tmTerceros.ToList(), "varNmbre", "intIdTrcro");
                        ViewBag.listLugarEmbarque = Funciones.GetListOfSelectListItem(db.tmLugaresEmbarque.ToList(), "varNmbreLgarEmbrque", "intIdLgarEmbrque");
                        ViewBag.listTransporte = Funciones.GetListOfSelectListItem(db.tmTransportes.ToList(), "varDscrpcionTrnsprte", "intIdTrnsprte");
                        ViewBag.listPlanCI = Funciones.GetListOfSelectListItem(db.tmPlanesCI.ToList(), "varDscrpcionPlanCI", "intIdPlanCI");

                        return View(tmvDEXEncabezado);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXEncabezadoController.Edit", usr.varLgin);
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
                        int intIdDEX = Convert.ToInt32(Request["intIdDEX"]);
                        tmvDEXEncabezado tmvDEXEncabezado = db.tmvDEXEncabezado.Where(i => i.intIdDEX == intIdDEX).FirstOrDefault();

                        tmvDEXEncabezado.varCdAuxliar = Request["varCdAuxliar"];
                        tmvDEXEncabezado.varNmroDEX = Request["varNmroDEX"];
                        tmvDEXEncabezado.fecAuxliar = Convert.ToDateTime(Request["fecAuxliar"]);
                        tmvDEXEncabezado.intIdCmprdor = Convert.ToInt32(Request["intIdCmprdor"]);
                        tmvDEXEncabezado.intIdImprtdorExprtdor = Convert.ToInt32(Request["intIdImprtdorExprtdor"]);
                        tmvDEXEncabezado.numNumroAcptcion = Request["numNumroAcptcion"];
                        tmvDEXEncabezado.varSlctudAtrzcionEmbrque = Request["varSlctudAtrzcionEmbrque"];
                        tmvDEXEncabezado.varMnfiestoCrga = Request["varMnfiestoCrga"];
                        tmvDEXEncabezado.varNmroFrmlrioAntrior = Request["varNmroFrmlrioAntrior"];
                        tmvDEXEncabezado.varNmroDcmntoTrnsprte = Request["varNmroDcmntoTrnsprte"];
                        tmvDEXEncabezado.varCmntrio = Request["varCmntrio"];

                        if (!string.IsNullOrWhiteSpace(Request["fecAprbcionDEX"]))
                        {
                            tmvDEXEncabezado.fecAprbcionDEX = Convert.ToDateTime(Request["fecAprbcionDEX"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecEmbrque"]))
                        {
                            tmvDEXEncabezado.fecEmbrque = Convert.ToDateTime(Request["fecEmbrque"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdAduana"]))
                        {
                            tmvDEXEncabezado.intIdAduana = Convert.ToInt32(Request["intIdAduana"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdLgarEmbrque"]))
                        {
                            tmvDEXEncabezado.intIdLgarEmbrque = Convert.ToInt32(Request["intIdLgarEmbrque"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdDclrnte"]))
                        {
                            tmvDEXEncabezado.intIdDclrnte = Convert.ToInt32(Request["intIdDclrnte"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numExprtcionesUSD"]))
                        {
                            tmvDEXEncabezado.numExprtcionesUSD = Convert.ToDecimal(Request["numExprtcionesUSD"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorFOB"]))
                        {
                            tmvDEXEncabezado.numVlorFOB = Convert.ToDecimal(Request["numVlorFOB"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numFltes"]))
                        {
                            tmvDEXEncabezado.numFltes = Convert.ToDecimal(Request["numFltes"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numOtrosGstos"]))
                        {
                            tmvDEXEncabezado.numOtrosGstos = Convert.ToDecimal(Request["numOtrosGstos"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numSgro"]))
                        {
                            tmvDEXEncabezado.numSgro = Convert.ToDecimal(Request["numSgro"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorReintgrarUSD"]))
                        {
                            tmvDEXEncabezado.numVlorReintgrarUSD = Convert.ToDecimal(Request["numVlorReintgrarUSD"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorAgrgdoNcional"]))
                        {
                            tmvDEXEncabezado.numVlorAgrgdoNcional = Convert.ToDecimal(Request["numVlorAgrgdoNcional"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numTtalSries"]))
                        {
                            tmvDEXEncabezado.numTtalSries = Convert.ToDecimal(Request["numTtalSries"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numBltos"]))
                        {
                            tmvDEXEncabezado.numBltos = Convert.ToDecimal(Request["numBltos"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numTtalPsoBrto"]))
                        {
                            tmvDEXEncabezado.numTtalPsoBrto = Convert.ToDecimal(Request["numTtalPsoBrto"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numTtalPsoNto"]))
                        {
                            tmvDEXEncabezado.numTtalPsoNto = Convert.ToDecimal(Request["numTtalPsoNto"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecAcptcion"]))
                        {
                            tmvDEXEncabezado.fecAcptcion = Convert.ToDateTime(Request["fecAcptcion"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecAtrzcionEmbrque"]))
                        {
                            tmvDEXEncabezado.fecAtrzcionEmbrque = Convert.ToDateTime(Request["fecAtrzcionEmbrque"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecMnfiestoCrga"]))
                        {
                            tmvDEXEncabezado.fecMnfiestoCrga = Convert.ToDateTime(Request["fecMnfiestoCrga"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdTrnsprte"]))
                        {
                            tmvDEXEncabezado.intIdTrnsprte = Convert.ToInt32(Request["intIdTrnsprte"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdPlanCI"]))
                        {
                            tmvDEXEncabezado.intIdPlanCI = Convert.ToInt32(Request["intIdPlanCI"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecDcmntoTrnsprte"]))
                        {
                            tmvDEXEncabezado.fecDcmntoTrnsprte = Convert.ToDateTime(Request["fecDcmntoTrnsprte"]);
                        }

                        //Actualizar
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de DEX",
                            string.Format("Aux: {0} ", tmvDEXEncabezado.varCdAuxliar),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmvDEXEncabezado/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXEncabezadoController.Update", usr.varLgin);
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