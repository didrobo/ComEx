using ComEx.Context;
using ComEx.Helpers;
using ComEx.Models;
using ComEx.Models.Entities;
using System;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using ComEx.Hubs;
using Microsoft.AspNet.SignalR;
using System.Data.Entity;

namespace ComEx.Controllers
{
    public class tmComprasController : Controller
    {
        // GET: tmCompras
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Reporte()
        {
            return View();
        }

        public ActionResult FormatoDocEquivalente()
        {
            return View();
        }

        public ActionResult FormatoDocEquivalenteNuevo()
        {
            return View();
        }

        public ActionResult FormatoRemisionCP()
        {
            return View();
        }

        public JsonResult GetCompras(DTParameters param) {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try {
                    List<vOrdenesCompraCI> lisCompras = new List<vOrdenesCompraCI>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            lisCompras = db.vOrdenesCompraCI.AsNoTracking().Where(x => x.fecCmpra.Value.Year == ano).ToList();
                        }
                        else
                        {
                            lisCompras = db.vOrdenesCompraCI.AsNoTracking().ToList();
                        }
                    }

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vOrdenesCompraCI> data;
                    DTResult<vOrdenesCompraCI> dtResult;
                    vOrdenesCompraCI totals;
                    int count;

                    if (param.Columns != null)
                    {
                        dtResult = new vOrdenesCompraCIResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, lisCompras, columnSearch);

                        data = dtResult.data;
                        count = dtResult.recordsTotal;
                        totals = dtResult.totals;
                    }
                    else {
                        IQueryable<vOrdenesCompraCI> results = lisCompras.AsQueryable();

                        lisCompras = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = lisCompras;
                        count = lisCompras.Count();

                        totals = new vOrdenesCompraCI()
                        {
                            numVlorTtal = lisCompras.Sum(s => s.numVlorTtal),
                            numVlorIva = lisCompras.Sum(s => s.numVlorIva),
                            numVlorRglias = lisCompras.Sum(s => s.numVlorRglias),
                            numBrtos = lisCompras.Sum(s => s.numBrtos),
                            numFnos = lisCompras.Sum(s => s.numFnos)
                        };
                    }

                    DTResult<vOrdenesCompraCI> resultado = new DTResult<vOrdenesCompraCI>
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
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.GetCompras", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmCompras/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
                try {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        var lstRegalia = (from a in db.tmRegalias
                                          join b in db.tmLineas
                                          on a.intIdLnea equals b.intIdLnea
                                          orderby a.intAno descending, a.intMes descending
                                          select new
                                          {
                                              intIdRglia = a.intIdRglia,
                                              varCdLnea = b.varCdLnea,
                                              intAno = a.intAno,
                                              intMes = a.intMes
                                          }
                                          ).AsEnumerable().Select(x => new SelectListItem
                                          {
                                              Text = string.Format("{0}-{1}/{2}", x.varCdLnea.ToString(), x.intAno.ToString(), x.intMes.ToString().PadLeft(2, '0')),
                                              Value = x.intIdRglia.ToString()
                                          }).ToList();

                        lstRegalia.Insert(0, new SelectListItem() { Selected = true, Text = "Seleccionar...", Value = "" });

                        //ViewBag.listProveedores = Funciones.GetListOfSelectListItem(db.vProveedores.ToList(), "varNmbre", "intIdPrvdor");
                        ViewBag.listReteFuente = Funciones.GetListOfSelectListItem(db.tmRetefuente.ToList(), "varDscrpcionRtncionfuente", "intIdRtncionfuente");
                        ViewBag.listCiudad = Funciones.GetListOfSelectListItem(db.tmCiudades.ToList(), "varDscrpcionCiudad", "intIdCiudad");
                        ViewBag.listAdquirida = Funciones.GetListOfSelectListItem(db.tmAdquiridasComprasTipo.ToList(), "varDscrpcionAdquirdaTpoCmpra", "intIdAdquirdaTpoCmpra");
                        ViewBag.listRegalia = lstRegalia;
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmCompras()
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmCompras tmCompra = new tmCompras();
                    //tmComprasDetalle tmCompraDetalle = new tmComprasDetalle();

                    tmCompra.varNumLte = Request["varNumLte"];
                    tmCompra.intCnsctvoCrgue = Convert.ToInt32(Request["intCnsctvoCrgue"]); //intCnsctvoCrgue
                    tmCompra.varCdAuxliar = Request["varCdAuxliar"];
                    tmCompra.varNmroFctra = Request["varNmroFctra"];
                    tmCompra.fecCmpra = Convert.ToDateTime(Request["fecCmpra"]);
                    tmCompra.intIdPrvdor = Convert.ToInt32(Request["intIdPrvdor"]);
                    tmCompra.bitAcgdosLey1429 = Convert.ToBoolean(Request["bitAcgdosLey1429"]);
                    tmCompra.bitAnldo = Convert.ToBoolean(Request["bitAnldo"]);
                    tmCompra.intIdRglia = Convert.ToInt32(Request["intIdRglia"]);
                    tmCompra.intIdCiudadRglias = Convert.ToInt32(Request["intIdCiudadRglias"]);
                    tmCompra.varDscrpcionTpoCmpra = Request["varDscrpcionTpoCmpra"];
                    tmCompra.varPrycto = Request["varPrycto"];
                    tmCompra.varLlve = Request["varLlve"];

                    if (!string.IsNullOrWhiteSpace(Request["intIdRslcion"]))
                    {
                        tmCompra.intIdRslcion = Convert.ToInt32(Request["intIdRslcion"]);
                    }

                    if(!string.IsNullOrWhiteSpace(Request["intIdTtloMnro"]))
                    {
                        tmCompra.intIdTtloMnro = Convert.ToInt32(Request["intIdTtloMnro"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdRtncionfuente"]))
                    {
                        tmCompra.intIdRtncionfuente = Convert.ToInt32(Request["intIdRtncionfuente"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numPrcntjeRtncionFte"]))
                    {
                        tmCompra.numPrcntjeRtncionFte = Convert.ToDecimal(Request["numPrcntjeRtncionFte"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorRtncionFuente"]))
                    {
                        tmCompra.numVlorRtncionFuente = Convert.ToDecimal(Request["numVlorRtncionFuente"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorIva"]))
                    {
                        tmCompra.numVlorIva = Convert.ToDecimal(Request["numVlorIva"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorTtal"]))
                    {
                        tmCompra.numVlorTtal = Convert.ToDecimal(Request["numVlorTtal"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdAdquirdaTpoCmpra"]))
                    {
                        tmCompra.intIdAdquirdaTpoCmpra = Convert.ToInt32(Request["intIdAdquirdaTpoCmpra"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorRglias"]))
                    {
                        tmCompra.numVlorRglias = Convert.ToDecimal(Request["numVlorRglias"]);
                    }
                    
                    if(!string.IsNullOrWhiteSpace(Request["fecPgoRglia"]))
                    {
                        tmCompra.fecPgoRglia = Convert.ToDateTime(Request["fecPgoRglia"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar Compra
                        db.tmCompras.Add(tmCompra);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación orden de compra",
                            string.Format("Aux: {0} Fac: {1} Cargue: {2} Lote: {3}", tmCompra.varCdAuxliar, tmCompra.varNmroFctra, tmCompra.intCnsctvoCrgue, tmCompra.varNumLte),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmComprasDetalle/Create/" + tmCompra.intIdCmpra };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.SetTmCompras", usr.varLgin);
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

        public JsonResult GetDatosProveedor(string IdProveedor)
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    if(string.IsNullOrWhiteSpace(IdProveedor))
                    {
                        ObjJson = new { Message = "Por favor selecciona un proveedor.", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        int intProveedor = Convert.ToInt32(IdProveedor);
                        var lstResolucionProveedor = db.vResolucionFactActualProveedor.Where(i => i.intIdPrvdor == intProveedor).ToList();
                        var lstTituloMinero = (from a in db.tmTituloMineroProveedor.ToList()
                                              where a.intIdPrvdor == intProveedor
                                              select new tmTituloMineroProveedor
                                              {
                                                  intIdTtloMnro = a.intIdTtloMnro,
                                                  intIdPrvdor = a.intIdPrvdor,
                                                  varPlca = a.varPlca
                                              }).ToList();
                        ObjJson = new
                        {
                            Message = "Resoluciones recuperadas correctamente.",
                            Success = true,
                            lstResolucionProveedor = lstResolucionProveedor,
                            lstTituloMinero = lstTituloMinero
                        };
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.GetDatosProveedor", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar una compra", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmCompras tmCompra = db.tmCompras.Where(i => i.intIdCmpra == id).FirstOrDefault();
                        //Eliminar Compra
                        db.tmCompras.Remove(tmCompra);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina orden de compra",
                            string.Format("Aux: {0} Fac: {1} Cargue: {2} Lote: {3}", tmCompra.varCdAuxliar, tmCompra.varNmroFctra, tmCompra.intCnsctvoCrgue, tmCompra.varNumLte),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmCompras/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.Delete", usr.varLgin);
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
                        tmCompras tmCompra = db.tmCompras.Where(i => i.intIdCmpra == id).FirstOrDefault();

                        var lstRegalia = (from a in db.tmRegalias
                                          join b in db.tmLineas
                                          on a.intIdLnea equals b.intIdLnea
                                          orderby a.intAno descending, a.intMes descending
                                          select new
                                          {
                                              intIdRglia = a.intIdRglia,
                                              varCdLnea = b.varCdLnea,
                                              intAno = a.intAno,
                                              intMes = a.intMes
                                          }
                                          ).AsEnumerable().Select(x => new SelectListItem
                                          {
                                              Text = string.Format("{0}-{1}/{2}", x.varCdLnea.ToString(), x.intAno.ToString(), x.intMes.ToString().PadLeft(2, '0')),
                                              Value = x.intIdRglia.ToString()
                                          }).ToList();

                        lstRegalia.Insert(0, new SelectListItem() { Selected = true, Text = "Seleccionar...", Value = "" });

                        ViewBag.listProveedores = Funciones.GetListOfSelectListItem(db.vProveedores.Where(p => p.intIdPrvdor == tmCompra.intIdPrvdor).ToList(), "varNmbre", "intIdPrvdor");
                        ViewBag.listReteFuente = Funciones.GetListOfSelectListItem(db.tmRetefuente.ToList(), "varDscrpcionRtncionfuente", "intIdRtncionfuente");
                        ViewBag.listCiudad = Funciones.GetListOfSelectListItem(db.tmCiudades.ToList(), "varDscrpcionCiudad", "intIdCiudad");
                        ViewBag.listAdquirida = Funciones.GetListOfSelectListItem(db.tmAdquiridasComprasTipo.ToList(), "varDscrpcionAdquirdaTpoCmpra", "intIdAdquirdaTpoCmpra");
                        ViewBag.listRegalia = lstRegalia;
                        return View(tmCompra);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.Edit", usr.varLgin);
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
                        int intIdCmpra = Convert.ToInt32(Request["intIdCmpra"]);
                        tmCompras tmCompra = db.tmCompras.Where(i => i.intIdCmpra == intIdCmpra).FirstOrDefault();

                        tmCompra.varNumLte = Request["varNumLte"];
                        tmCompra.intCnsctvoCrgue = Convert.ToInt32(Request["intCnsctvoCrgue"]);
                        tmCompra.varCdAuxliar = Request["varCdAuxliar"];
                        tmCompra.varNmroFctra = Request["varNmroFctra"];
                        tmCompra.fecCmpra = string.IsNullOrWhiteSpace(Request["fecCmpra"]) ? Convert.ToDateTime(Request["fecCmpra.Value"]) : Convert.ToDateTime(Request["fecCmpra"]);
                        tmCompra.intIdPrvdor = Convert.ToInt32(Request["intIdPrvdor"]);
                        tmCompra.bitAcgdosLey1429 = Convert.ToBoolean(Request["bitAcgdosLey1429"]);
                        tmCompra.bitAnldo = Convert.ToBoolean(Request["bitAnldo"]);
                        tmCompra.intIdRglia = Convert.ToInt32(Request["intIdRglia"]);
                        tmCompra.intIdCiudadRglias = Convert.ToInt32(Request["intIdCiudadRglias"]);
                        tmCompra.varDscrpcionTpoCmpra = Request["varDscrpcionTpoCmpra"];
                        tmCompra.varPrycto = Request["varPrycto"];
                        tmCompra.varLlve = Request["varLlve"];

                        if (!string.IsNullOrWhiteSpace(Request["intIdRslcion"]))
                        {
                            tmCompra.intIdRslcion = Convert.ToInt32(Request["intIdRslcion"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdTtloMnro"]))
                        {
                            tmCompra.intIdTtloMnro = Convert.ToInt32(Request["intIdTtloMnro"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdRtncionfuente"]))
                        {
                            tmCompra.intIdRtncionfuente = Convert.ToInt32(Request["intIdRtncionfuente"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numPrcntjeRtncionFte"]))
                        {
                            tmCompra.numPrcntjeRtncionFte = Convert.ToDecimal(Request["numPrcntjeRtncionFte"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorRtncionFuente"]))
                        {
                            tmCompra.numVlorRtncionFuente = Convert.ToDecimal(Request["numVlorRtncionFuente"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorIva"]))
                        {
                            tmCompra.numVlorIva = Convert.ToDecimal(Request["numVlorIva"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorTtal"]))
                        {
                            tmCompra.numVlorTtal = Convert.ToDecimal(Request["numVlorTtal"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdAdquirdaTpoCmpra"]))
                        {
                            tmCompra.intIdAdquirdaTpoCmpra = Convert.ToInt32(Request["intIdAdquirdaTpoCmpra"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorRglias"]))
                        {
                            tmCompra.numVlorRglias = Convert.ToDecimal(Request["numVlorRglias"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecPgoRglia"]))
                        {
                            tmCompra.fecPgoRglia = Convert.ToDateTime(Request["fecPgoRglia"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecPgoRglia.Value"]))
                        {
                            tmCompra.fecPgoRglia = Convert.ToDateTime(Request["fecPgoRglia.Value"]);
                        }

                        //Actualizar Compras
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de orden de compra",
                            string.Format("Aux: {0} Fac: {1} Cargue: {2} Lote: {3}", tmCompra.varCdAuxliar, tmCompra.varNmroFctra, tmCompra.intCnsctvoCrgue, tmCompra.varNumLte),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmCompras/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.Update", usr.varLgin);
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

        public JsonResult GetListCargues(string paramSearch)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    object listCargues = null;

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        List<tmCompras> listTmCompras = db.tmCompras.Where(c => c.intCnsctvoCrgue.ToString().Contains(paramSearch)).ToList();

                        var cargues = listTmCompras.GroupBy(c => c.intCnsctvoCrgue).Select(group => new { 
                            cargue = group.Key,
                            cantCmpras = group.Count()
                        }).OrderBy(or => or.cargue);

                        listCargues = cargues;
                    }

                    result = Json(listCargues, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.GetListCargues", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public JsonResult BorrarCargue(int intCnsctvoCrgue)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Database.CommandTimeout = 180;
                        db.spEliminarCargueAnexpo(intCnsctvoCrgue);

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Borrar cargue",
                            string.Format("Se eliminó el cargue {0}", intCnsctvoCrgue),
                            "Masivo",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    result = Json(new { success = true, mensaje = string.Format("Se eliminó el cargue {0}", intCnsctvoCrgue) });
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.BorrarCargue", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GetListLotes(string paramSearch)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    object listLotes = null;

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        List<tmCompras> listTmCompras = db.tmCompras.Where(c => c.varNumLte.ToString().Contains(paramSearch)).ToList();

                        var lotes = listTmCompras.GroupBy(c => c.varNumLte).Select(group => new
                        {
                            lote = group.Key,
                            cantCmpras = group.Count()
                        }).OrderBy(or => or.lote);

                        listLotes = lotes;
                    }

                    result = Json(listLotes, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.GetListLotes", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public JsonResult GenerarOrdenCompra()
        {
            //Variables
            JsonResult result = new JsonResult();
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
                List<DTFilters> listFiltros = (List<DTFilters>)Session["listFiltros"];
                bool ChkFiltroPorAno = GlobalConfig.get().ChkFiltroPorAno;
                int AppAnoFiltro = GlobalConfig.get().AppAnoFiltro;

                HostingEnvironment.QueueBackgroundWorkItem(c => Generar(usr, listFiltros, ChkFiltroPorAno, AppAnoFiltro));
                result = Json(new { success = true });
            }
            else
            {
                result = Json(new { success = false });
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GenerarDocEquivalente()
        {
            //Variables
            JsonResult result = new JsonResult();
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
                List<DTFilters> listFiltros = (List<DTFilters>)Session["listFiltros"];
                bool ChkFiltroPorAno = GlobalConfig.get().ChkFiltroPorAno;
                int AppAnoFiltro = GlobalConfig.get().AppAnoFiltro;

                HostingEnvironment.QueueBackgroundWorkItem(c => GenerarDocEquivalente(usr, listFiltros, ChkFiltroPorAno, AppAnoFiltro));
                result = Json(new { success = true });
            }
            else
            {
                result = Json(new { success = false });
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GenerarDocEquivalenteNuevo()
        {
            //Variables
            JsonResult result = new JsonResult();
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
                List<DTFilters> listFiltros = (List<DTFilters>)Session["listFiltros"];
                bool ChkFiltroPorAno = GlobalConfig.get().ChkFiltroPorAno;
                int AppAnoFiltro = GlobalConfig.get().AppAnoFiltro;

                HostingEnvironment.QueueBackgroundWorkItem(c => GenerarDocEquivalenteNuevo(usr, listFiltros, ChkFiltroPorAno, AppAnoFiltro));
                result = Json(new { success = true });
            }
            else
            {
                result = Json(new { success = false });
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        private void Generar(tcUsuarios objUsuario, List<DTFilters> lstFiltros, bool ChkFiltroPorAno, int AppAnoFiltro)
        {

            //Variables
            List<vFormatoOrdenesCompraCI> lstFormatoOrdenCompra = new List<vFormatoOrdenesCompraCI>();
            List<vOrdenesCompraCI> listOrdenesCompraCI = new List<vOrdenesCompraCI>();
            List<tmDocumentosAdjuntos> listDocumentosAdjuntos = new List<tmDocumentosAdjuntos>();
            string strUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/");

            List<DTFilters> listFiltros = null;
            int intIdTpoDcmntoArchvosAdjntos;

            try
            {
                //Iniciar la creación del reporte
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "Mostrar", "Generanado ordenes de compras...");

                if (lstFiltros != null)
                {
                    listFiltros = lstFiltros;
                }

                using (db_comexEntities db = new db_comexEntities())
                {
                    //Si la tabla fue filtrada se aplican los filtros
                    if (listFiltros != null)
                    {
                        //Si esta activo el filtro por año
                        if (ChkFiltroPorAno)
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                        }
                        else
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.ToList();
                        }
                        
                        IQueryable<vOrdenesCompraCI> results = listOrdenesCompraCI.AsQueryable();
                        listOrdenesCompraCI = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                        if (listOrdenesCompraCI.Count > 0)
                        {
                            //Si esta activo el filtro por año
                            if (ChkFiltroPorAno)
                            {
                                lstFormatoOrdenCompra = db.vFormatoOrdenesCompraCI.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                            }
                            else
                            {
                                lstFormatoOrdenCompra = db.vFormatoOrdenesCompraCI.ToList();
                            }                            

                            lstFormatoOrdenCompra = lstFormatoOrdenCompra.Where(
                                x => listOrdenesCompraCI.Select(
                                    s => s.intIdCmpra
                                ).Contains(x.intIdCmpra)
                            ).ToList();
                        }
                    }
                    else
                    {
                        //Si esta activo el filtro por año
                        if (ChkFiltroPorAno)
                        {
                            lstFormatoOrdenCompra = db.vFormatoOrdenesCompraCI.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                        }
                        else
                        {
                            lstFormatoOrdenCompra = db.vFormatoOrdenesCompraCI.ToList();
                        }                        
                    }

                    //Selecciona el tipo de Documento
                    intIdTpoDcmntoArchvosAdjntos = db.tipoDocumentosArchivosAdjuntos.Where(
                        x => x.varCdTpoDcmnto == "OC"
                        ).FirstOrDefault().intIdTpoDcmntoArchvosAdjntos;
                }

                List<tmDocumentosAdjuntos> listDocumentos = new List<tmDocumentosAdjuntos>();
                tmDocumentosAdjuntos documento;
                //Generar PDF uno por uno
                foreach (vFormatoOrdenesCompraCI objOrdenCompra in lstFormatoOrdenCompra)
                {
                    //Variables
                    string strReporte = Server.MapPath("~/Report/OrdenCompra.rdlc");
                    string strFirma = Server.MapPath("~/img/Reportes/OrdenCompra/firma.png");
                    string strNombreArchivo = string.Format("OrdenCompra-{0}-Fact-{1}.pdf", objOrdenCompra.varNmroFctra, objOrdenCompra.varFctra);
                    string strRutaArchivo = Server.MapPath("~/Archivos/OrdenCompra/" + objOrdenCompra.fecCmpra.Value.Year.ToString());
                    string strRuta_Archivo = string.Format(@"{0}/{1}", strRutaArchivo, strNombreArchivo);
                    List<vFormatoOrdenesCompraCI> lstOrdenCompra = new List<vFormatoOrdenesCompraCI>();

                    //Validar si existe la ruta.
                    if (!Directory.Exists(strRutaArchivo))
                    {
                        Directory.CreateDirectory(strRutaArchivo);
                    }

                    ReportViewer rptvPrincipal = null;

                    //Crear el registro para asociar el archivo a la orden de compra
                    documento = new tmDocumentosAdjuntos()
                    {
                        intIdTpoDcmntoArchvosAdjntos = intIdTpoDcmntoArchvosAdjntos,
                        varRta = string.Format(@"{0}/{1}", "/Archivos/OrdenCompra/" + objOrdenCompra.fecCmpra.Value.Year.ToString(), strNombreArchivo),
                        varObsrvciones = string.Format("Documento creado el {0}", DateTime.Now.ToString("yyyy-MM-dd")),
                        varFrmlrio = "frmCompras",
                        intIdDcmnto = objOrdenCompra.intIdCmpra,
                        intidUsuario = objUsuario.intIdUsuario
                    };
                    listDocumentos.Add(documento);

                    rptvPrincipal = new ReportViewer();
                    rptvPrincipal.LocalReport.ReportPath = strReporte;
                    rptvPrincipal.LocalReport.DataSources.Clear();
                    //ReportDataSource dtReporte = new ReportDataSource("dtOrdenCompra", lstFormatoOrdenCompra);
                    lstOrdenCompra.Add(objOrdenCompra);
                    rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtOrdenCompra", lstOrdenCompra));
                    //Agregar firma
                    rptvPrincipal.LocalReport.EnableExternalImages = true;
                    ReportParameter prmImagen = new ReportParameter();
                    prmImagen.Name = "Firma";
                    prmImagen.Values.Add(strFirma);
                    rptvPrincipal.LocalReport.SetParameters(prmImagen);
                    rptvPrincipal.LocalReport.Refresh();

                    try { 

                        //Verifica si el archivo existe
                        if (System.IO.File.Exists(strRuta_Archivo))
                        {
                            System.IO.File.Delete(strRuta_Archivo);
                        }
                        //Crear PDF
                        byte[] byteArchivo = rptvPrincipal.LocalReport.Render("PDF");
                        FileStream OrdenCompraPdf = System.IO.File.Create(strRuta_Archivo, byteArchivo.Length, FileOptions.WriteThrough);
                        OrdenCompraPdf.Write(byteArchivo, 0, byteArchivo.Length);
                        OrdenCompraPdf.Flush();
                        OrdenCompraPdf.Close();
                    }
                    catch (Exception) { }
            }

                //Llenar tabla
                using (db_comexEntities db = new db_comexEntities())
                {

                    //Selecciona los Adjuntos que ya existen
                    listDocumentosAdjuntos = db.tmDocumentosAdjuntos.Where(x => x.varFrmlrio == "frmCompras" && x.intIdTpoDcmntoArchvosAdjntos == intIdTpoDcmntoArchvosAdjntos).ToList();

                    listDocumentosAdjuntos = listDocumentosAdjuntos.Where(
                        x => listDocumentos.Select(
                            s => s.intIdDcmnto
                            ).Contains((int)x.intIdDcmnto)
                    ).ToList();

                    db.tmDocumentosAdjuntos.RemoveRange(listDocumentosAdjuntos);

                    db.tmDocumentosAdjuntos.AddRange(listDocumentos);
                    db.SaveChanges();
                }

                //Temrinar de crear los reportes
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", "Ordenes de compras creadas correctamente.");
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.GenerarOrdenCompra", objUsuario.varLgin);
                //Temrinar de crear los reportes
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", ex.Message);
            }
        }

        private void GenerarDocEquivalente(tcUsuarios objUsuario, List<DTFilters> lstFiltros, bool ChkFiltroPorAno, int AppAnoFiltro)
        {
            //Variables
            List<vFormatoDocEquivalenteCI> lstFormatoDocEquivalente = new List<vFormatoDocEquivalenteCI>();
            List<vOrdenesCompraCI> listOrdenesCompraCI = new List<vOrdenesCompraCI>();
            List<tmDocumentosAdjuntos> listDocumentosAdjuntos = new List<tmDocumentosAdjuntos>();
            string strUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/");

            List<DTFilters> listFiltros = null;
            int intIdTpoDcmntoArchvosAdjntos;

            try
            {
                //Iniciar la creación del reporte
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "Mostrar", "Generanado documentos equivalentes...");

                if (lstFiltros != null)
                {
                    listFiltros = lstFiltros;
                }

                using (db_comexEntities db = new db_comexEntities())
                {
                    //Si la tabla fue filtrada se aplican los filtros
                    if (listFiltros != null)
                    {
                        //Si esta activo el filtro por año
                        if (ChkFiltroPorAno)
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                        }
                        else
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.ToList();
                        }

                        IQueryable<vOrdenesCompraCI> results = listOrdenesCompraCI.AsQueryable();
                        listOrdenesCompraCI = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                        if (listOrdenesCompraCI.Count > 0)
                        {
                            //Si esta activo el filtro por año
                            if (ChkFiltroPorAno)
                            {
                                lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCI.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                            }
                            else
                            {
                                lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCI.ToList();
                            }

                            lstFormatoDocEquivalente = lstFormatoDocEquivalente.Where(
                                x => listOrdenesCompraCI.Select(
                                    s => s.intIdCmpra
                                ).Contains(x.intIdCmpra)
                            ).ToList();
                        }
                    }
                    else
                    {
                        //Si esta activo el filtro por año
                        if (ChkFiltroPorAno)
                        {
                            lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCI.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                        }
                        else
                        {
                            lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCI.ToList();
                        }
                    }

                    //Selecciona el tipo de Documento
                    intIdTpoDcmntoArchvosAdjntos = db.tipoDocumentosArchivosAdjuntos.Where(
                        x => x.varCdTpoDcmnto == "DE"
                        ).FirstOrDefault().intIdTpoDcmntoArchvosAdjntos;
                }

                List<tmDocumentosAdjuntos> listDocumentos = new List<tmDocumentosAdjuntos>();
                tmDocumentosAdjuntos documento;
                //Generar PDF uno por uno
                foreach (vFormatoDocEquivalenteCI objDocEquivalente in lstFormatoDocEquivalente)
                {
                    //Variables
                    string strReporte = Server.MapPath("~/Report/DocumentoEquivalente.rdlc");
                    string strLogo = Server.MapPath("~/img/Logo_Anexpo.png");
                    string strNombreArchivo = string.Format("Equivalente-{0}-Fact-{1}.pdf", objDocEquivalente.varNumLte, objDocEquivalente.varNmroFctra);
                    string strRutaArchivo = Server.MapPath("~/Archivos/DocEquivalente/" + objDocEquivalente.fecCmpra.Value.Year.ToString());
                    string strRuta_Archivo = string.Format(@"{0}/{1}", strRutaArchivo, strNombreArchivo);
                    List<vFormatoDocEquivalenteCI> lstDocEquivalente = new List<vFormatoDocEquivalenteCI>();

                    //Validar si existe la ruta.
                    if (!Directory.Exists(strRutaArchivo))
                    {
                        Directory.CreateDirectory(strRutaArchivo);
                    }

                    ReportViewer rptvPrincipal = null;

                    //Crear el registro para asociar el archivo a la orden de compra
                    documento = new tmDocumentosAdjuntos()
                    {
                        intIdTpoDcmntoArchvosAdjntos = intIdTpoDcmntoArchvosAdjntos,
                        varRta = string.Format(@"{0}/{1}", "/Archivos/DocEquivalente/" + objDocEquivalente.fecCmpra.Value.Year.ToString(), strNombreArchivo),
                        varObsrvciones = string.Format("Documento creado el {0}", DateTime.Now.ToString("yyyy-MM-dd")),
                        varFrmlrio = "frmCompras",
                        intIdDcmnto = objDocEquivalente.intIdCmpra,
                        intidUsuario = objUsuario.intIdUsuario
                    };
                    listDocumentos.Add(documento);

                    rptvPrincipal = new ReportViewer();
                    rptvPrincipal.LocalReport.ReportPath = strReporte;
                    rptvPrincipal.LocalReport.DataSources.Clear();

                    lstDocEquivalente.Add(objDocEquivalente);
                    rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtDocEquivalente", lstDocEquivalente));

                    //Agregar Logo
                    rptvPrincipal.LocalReport.EnableExternalImages = true;
                    ReportParameter prmLogo = new ReportParameter();
                    prmLogo.Name = "Logo";
                    prmLogo.Values.Add(strLogo);
                    rptvPrincipal.LocalReport.SetParameters(prmLogo);

                    rptvPrincipal.LocalReport.Refresh();
                    try
                    {
                        //Verifica si el archivo existe
                        if (System.IO.File.Exists(strRuta_Archivo))
                        {
                             System.IO.File.Delete(strRuta_Archivo);
                        }

                        //Crear PDF
                        byte[] byteArchivo = rptvPrincipal.LocalReport.Render("PDF");
                        FileStream DocEquivalentePdf = System.IO.File.Create(strRuta_Archivo, byteArchivo.Length, FileOptions.WriteThrough);
                        DocEquivalentePdf.Write(byteArchivo, 0, byteArchivo.Length);
                        DocEquivalentePdf.Flush();
                        DocEquivalentePdf.Close();
                    }
                    catch (Exception) { }
                }

                //Llenar tabla
                using (db_comexEntities db = new db_comexEntities())
                {

                    //Selecciona los Adjuntos que ya existen
                    listDocumentosAdjuntos = db.tmDocumentosAdjuntos.Where(x => x.varFrmlrio == "frmCompras" && x.intIdTpoDcmntoArchvosAdjntos == intIdTpoDcmntoArchvosAdjntos).ToList();

                    listDocumentosAdjuntos = listDocumentosAdjuntos.Where(
                        x => listDocumentos.Select(
                            s => s.intIdDcmnto
                            ).Contains((int)x.intIdDcmnto)
                    ).ToList();

                    db.tmDocumentosAdjuntos.RemoveRange(listDocumentosAdjuntos);

                    db.tmDocumentosAdjuntos.AddRange(listDocumentos);
                    db.SaveChanges();
                }

                //Temrinar de crear los reportes
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", "Documentos equivalentes creados correctamente.");
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.GenerarDocEquivalente", objUsuario.varLgin);
                //Temrinar de crear los reportes
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", ex.Message);
            }
        }

        private void GenerarDocEquivalenteNuevo(tcUsuarios objUsuario, List<DTFilters> lstFiltros, bool ChkFiltroPorAno, int AppAnoFiltro)
        {
            //Variables
            List<vFormatoDocEquivalenteCINuevo> lstFormatoDocEquivalente = new List<vFormatoDocEquivalenteCINuevo>();
            List<vOrdenesCompraCI> listOrdenesCompraCI = new List<vOrdenesCompraCI>();
            List<tmDocumentosAdjuntos> listDocumentosAdjuntos = new List<tmDocumentosAdjuntos>();
            string strUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/");

            List<DTFilters> listFiltros = null;
            int intIdTpoDcmntoArchvosAdjntos;

            try
            {
                //Iniciar la creación del reporte
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "Mostrar", "Generanado documentos equivalentes...");

                if (lstFiltros != null)
                {
                    listFiltros = lstFiltros;
                }

                using (db_comexEntities db = new db_comexEntities())
                {
                    //Si la tabla fue filtrada se aplican los filtros
                    if (listFiltros != null)
                    {
                        //Si esta activo el filtro por año
                        if (ChkFiltroPorAno)
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                        }
                        else
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.ToList();
                        }

                        IQueryable<vOrdenesCompraCI> results = listOrdenesCompraCI.AsQueryable();
                        listOrdenesCompraCI = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                        if (listOrdenesCompraCI.Count > 0)
                        {
                            //Si esta activo el filtro por año
                            if (ChkFiltroPorAno)
                            {
                                lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCINuevo.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                            }
                            else
                            {
                                lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCINuevo.ToList();
                            }

                            lstFormatoDocEquivalente = lstFormatoDocEquivalente.Where(
                                x => listOrdenesCompraCI.Select(
                                    s => s.intIdCmpra
                                ).Contains(x.intIdCmpra)
                            ).ToList();
                        }
                    }
                    else
                    {
                        //Si esta activo el filtro por año
                        if (ChkFiltroPorAno)
                        {
                            lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCINuevo.Where(x => x.fecCmpra.Value.Year == AppAnoFiltro).ToList();
                        }
                        else
                        {
                            lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCINuevo.ToList();
                        }
                    }

                    //Selecciona el tipo de Documento
                    intIdTpoDcmntoArchvosAdjntos = db.tipoDocumentosArchivosAdjuntos.Where(
                        x => x.varCdTpoDcmnto == "DE"
                        ).FirstOrDefault().intIdTpoDcmntoArchvosAdjntos;
                }

                List<tmDocumentosAdjuntos> listDocumentos = new List<tmDocumentosAdjuntos>();
                tmDocumentosAdjuntos documento;
                //Generar PDF uno por uno
                foreach (vFormatoDocEquivalenteCINuevo objDocEquivalente in lstFormatoDocEquivalente)
                {
                    //Variables
                    string strReporte = Server.MapPath("~/Report/DocumentoEquivalenteNuevo.rdlc");
                    string strLogo = Server.MapPath("~/img/Logo_Anexpo.png");
                    string strNombreArchivo = string.Format("Equivalente-{0}-Fact-{1}.pdf", objDocEquivalente.varNumLte, objDocEquivalente.varNmroFctra);
                    string strRutaArchivo = Server.MapPath("~/Archivos/DocEquivalente/" + objDocEquivalente.fecCmpra.Value.Year.ToString());
                    string strRuta_Archivo = string.Format(@"{0}/{1}", strRutaArchivo, strNombreArchivo);
                    List<vFormatoDocEquivalenteCINuevo> lstDocEquivalente = new List<vFormatoDocEquivalenteCINuevo>();

                    //Validar si existe la ruta.
                    if (!Directory.Exists(strRutaArchivo))
                    {
                        Directory.CreateDirectory(strRutaArchivo);
                    }

                    ReportViewer rptvPrincipal = null;

                    //Crear el registro para asociar el archivo a la orden de compra
                    documento = new tmDocumentosAdjuntos()
                    {
                        intIdTpoDcmntoArchvosAdjntos = intIdTpoDcmntoArchvosAdjntos,
                        varRta = string.Format(@"{0}/{1}", "/Archivos/DocEquivalente/" + objDocEquivalente.fecCmpra.Value.Year.ToString(), strNombreArchivo),
                        varObsrvciones = string.Format("Documento creado el {0}", DateTime.Now.ToString("yyyy-MM-dd")),
                        varFrmlrio = "frmCompras",
                        intIdDcmnto = objDocEquivalente.intIdCmpra,
                        intidUsuario = objUsuario.intIdUsuario
                    };
                    listDocumentos.Add(documento);

                    rptvPrincipal = new ReportViewer();
                    rptvPrincipal.LocalReport.ReportPath = strReporte;
                    rptvPrincipal.LocalReport.DataSources.Clear();

                    lstDocEquivalente.Add(objDocEquivalente);
                    rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtDocEquivalenteNuevo", lstDocEquivalente));

                    //Agregar Logo
                    rptvPrincipal.LocalReport.EnableExternalImages = true;
                    ReportParameter prmLogo = new ReportParameter();
                    prmLogo.Name = "Logo";
                    prmLogo.Values.Add(strLogo);
                    rptvPrincipal.LocalReport.SetParameters(prmLogo);

                    rptvPrincipal.LocalReport.Refresh();
                    try
                    {
                        //Verifica si el archivo existe
                        if (System.IO.File.Exists(strRuta_Archivo))
                        {
                            System.IO.File.Delete(strRuta_Archivo);
                        }

                        //Crear PDF
                        byte[] byteArchivo = rptvPrincipal.LocalReport.Render("PDF");
                        FileStream DocEquivalentePdf = System.IO.File.Create(strRuta_Archivo, byteArchivo.Length, FileOptions.WriteThrough);
                        DocEquivalentePdf.Write(byteArchivo, 0, byteArchivo.Length);
                        DocEquivalentePdf.Flush();
                        DocEquivalentePdf.Close();
                    }
                    catch (Exception) { }
                }

                //Llenar tabla
                using (db_comexEntities db = new db_comexEntities())
                {

                    //Selecciona los Adjuntos que ya existen
                    listDocumentosAdjuntos = db.tmDocumentosAdjuntos.Where(x => x.varFrmlrio == "frmCompras" && x.intIdTpoDcmntoArchvosAdjntos == intIdTpoDcmntoArchvosAdjntos).ToList();

                    listDocumentosAdjuntos = listDocumentosAdjuntos.Where(
                        x => listDocumentos.Select(
                            s => s.intIdDcmnto
                            ).Contains((int)x.intIdDcmnto)
                    ).ToList();

                    db.tmDocumentosAdjuntos.RemoveRange(listDocumentosAdjuntos);

                    db.tmDocumentosAdjuntos.AddRange(listDocumentos);
                    db.SaveChanges();
                }

                //Temrinar de crear los reportes
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", "Documentos equivalentes creados correctamente.");
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.GenerarDocEquivalenteNuevo", objUsuario.varLgin);
                //Temrinar de crear los reportes
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", ex.Message);
            }
        }

        public JsonResult CambiarLote(string varNumLte, string varNumLte_nuevo)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Database.CommandTimeout = 180;
                        List<tmCompras> listCompras = db.tmCompras.Where(x => x.varNumLte == varNumLte).ToList();

                        listCompras.ForEach(compra => compra.varNumLte = varNumLte_nuevo);

                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Cambiar lote",
                            string.Format("Se cambió el lote {0} por el lote {1}", varNumLte, varNumLte_nuevo),
                            "Masivo",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    result = Json(new { success = true, mensaje = string.Format("Se cambió el lote {0} por el lote {1}", varNumLte, varNumLte_nuevo) });
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.CambiarLote", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult CambiarFecAuxCargue(int intIdCrgue, DateTime fecAux)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Database.CommandTimeout = 180;

                        List<vOrdenesCompraCI> listOrdenesCompraCI = db.vOrdenesCompraCI.Where(x => x.intCnsctvoCrgue == intIdCrgue).ToList();


                        if (listOrdenesCompraCI.Where(x => !String.IsNullOrEmpty(x.varNmroCP)).Count() == 0)
                        {

                            using (DbContextTransaction transaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    List<tmCompras> listCompras = db.tmCompras.Where(x => x.intCnsctvoCrgue == intIdCrgue).ToList();
                                    List<tmCPEncabezado> listCPs = db.tmCPEncabezado
                                        .Join(db.tmCompras.Where(x => x.intCnsctvoCrgue == intIdCrgue),
                                        cp => cp.intIdCP,
                                        compra => compra.intIdCP,
                                        (cp, compra) => new { cp, compra })
                                        .Select(s => s.cp).ToList();
                                                                                                                      

                                    listCompras = listCompras.Where(
                                        x => listOrdenesCompraCI.Select(
                                            s => s.intIdCmpra
                                        ).Contains(x.intIdCmpra)
                                    ).ToList();

                                    listCPs = listCPs.Where(
                                        x => listOrdenesCompraCI.Select(
                                            s => s.intIdCP
                                        ).Contains(x.intIdCP)
                                    ).ToList();


                                    listCompras.ForEach(compra => compra.fecCmpra = fecAux);
                                    listCPs.ForEach(cp => cp.fecCP = fecAux);

                                    db.SaveChanges();

                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    throw ex;
                                }
                            }

                            //Guarda el Log
                            tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Cambiar fecha aux cargue",
                                string.Format("Se cambió la fecha del cargue {0} por {1}", intIdCrgue, fecAux.ToString("yyyy/MM/dd")),
                                "Masivo",
                                usr.varLgin);

                            db.tcLogOperaciones.Add(mytcLogOperaciones);
                            db.SaveChanges();

                            result = Json(new { success = true, mensaje = string.Format("Se cambió la fecha del cargue {0} por {1}", intIdCrgue, fecAux.ToString("yyyy/MM/dd")) });
                        }
                        else
                        {
                            result = Json(new { success = false, error = "No se puede cambiar la fecha auxiliar del cargue por que tiene CPs DIAN asociados" });
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasController.CambiarFecAuxCargue", usr.varLgin);
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