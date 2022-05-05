using ComEx.Context;
using ComEx.Helpers;
using ComEx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class tmInsumosController : Controller
    {
        // GET: tmInsumos
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetInsumos(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmInsumos> listInsumos = new List<tmInsumos>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listInsumos = db.tmInsumos.Include("tmComprasTipo").Include("tmLineas").Include("tmPosicionesArancelarias").Include("tmUnidades").Include("tmUnidades1").Include("tmUnidades2").ToList();
                    }


                    listInsumos.ForEach(x => {
                        if (x.tmComprasTipo != null) x.tmComprasTipo.tmInsumos = null; else x.tmComprasTipo = new tmComprasTipo();
                        if (x.tmLineas != null) x.tmLineas.tmInsumos = null; else x.tmLineas = new tmLineas();

                        //if (x.tmPosicionesArancelarias != null) x.tmPosicionesArancelarias.tmInsumos = null; else x.tmPosicionesArancelarias = new tmPosicionesArancelarias();
                        if (x.tmPosicionesArancelarias != null)
                        {
                            x.tmPosicionesArancelarias.tmInsumos = null;
                            x.tmPosicionesArancelarias.tmPosicionesArancelarias1 = null;
                            x.tmPosicionesArancelarias.tmPosicionesArancelarias2 = null;
                            x.tmPosicionesArancelarias.tmCuadrosInsumoProducto = null;
                            x.tmPosicionesArancelarias.tmCuadrosInsumoProducto1 = null;
                            x.tmPosicionesArancelarias.tmUnidades = null;
                        }
                        else
                        {
                            x.tmPosicionesArancelarias = new tmPosicionesArancelarias();
                        }

                        if (x.tmUnidades != null) {
                            x.tmUnidades.tmInsumos = null;
                            x.tmUnidades.tmInsumos1 = null;
                            x.tmUnidades.tmInsumos2 = null;
                            x.tmUnidades.tmPosicionesArancelarias = null;                            
                        }
                        else {
                            x.tmUnidades = new tmUnidades();
                        }

                        if (x.tmUnidades1 != null)
                        {
                            x.tmUnidades1.tmInsumos = null;
                            x.tmUnidades1.tmInsumos1 = null;
                            x.tmUnidades1.tmInsumos2 = null;
                            x.tmUnidades1.tmPosicionesArancelarias = null;
                        }
                        else
                        {
                            x.tmUnidades1 = new tmUnidades();
                        }

                        if (x.tmUnidades2 != null)
                        {
                            x.tmUnidades2.tmInsumos = null;
                            x.tmUnidades2.tmInsumos1 = null;
                            x.tmUnidades2.tmInsumos2 = null;
                            x.tmUnidades2.tmPosicionesArancelarias = null;
                        }
                        else
                        {
                            x.tmUnidades2 = new tmUnidades();
                        }
                    });

                    int count = listInsumos.Count;

                    DTResult<tmInsumos> resultado = new DTResult<tmInsumos>
                    {
                        draw = param.Draw,
                        data = listInsumos,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmInsumosController.GetInsumos", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmInsumos/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        ViewBag.listTipoMaterial = Funciones.GetListOfSelectListItem(db.tmLineas.ToList(), "varDscrpcionLnea", "intIdLnea");
                        ViewBag.listPosArancelaria = Funciones.GetListOfSelectListItem(db.tmPosicionesArancelarias.ToList(), "varPscionArnclria", "intIdPscionArnclria");
                        ViewBag.listEstadoMercancia = Funciones.GetListOfSelectListItem(db.tmComprasTipo.ToList(), "varDscrpcionCmpra", "intIdTpoCmpra");
                        ViewBag.listUnidad = Funciones.GetListOfSelectListItem(db.tmUnidades.ToList(), "varDscrpcionUndadEspnol", "intIdUndad");
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmInsumosController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmInsumos()
        {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmInsumos tmInsumos = new tmInsumos();

                    tmInsumos.varCdInsmo = Request["varCdInsmo"];
                    tmInsumos.varDscrpcionInsmo = Request["varDscrpcionInsmo"];
                    tmInsumos.bitInsmoNcional = true;

                    if (!string.IsNullOrWhiteSpace(Request["intIdLnea"]))
                    {
                        tmInsumos.intIdLnea = Convert.ToInt32(Request["intIdLnea"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdPscionArnclria"]))
                    {
                        tmInsumos.intIdPscionArnclria = Convert.ToInt32(Request["intIdPscionArnclria"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdUndad"]))
                    {
                        tmInsumos.intIdUndad = Convert.ToInt32(Request["intIdUndad"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdUndadDian"]))
                    {
                        tmInsumos.intIdUndadDian = Convert.ToInt32(Request["intIdUndadDian"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdUndadCmpra"]))
                    {
                        tmInsumos.intIdUndadCmpra = Convert.ToInt32(Request["intIdUndadCmpra"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdTpoCmpra"]))
                    {
                        tmInsumos.intIdTpoCmpra = Convert.ToInt32(Request["intIdTpoCmpra"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numFctorMncmex"]))
                    {
                        tmInsumos.numFctorMncmex = Convert.ToDecimal(Request["numFctorMncmex"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar Tipo Material
                        db.tmInsumos.Add(tmInsumos);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación insumos",
                            string.Format("Insumo: {0}", tmInsumos.varDscrpcionInsmo),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmInsumos/Index/" };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmInsumosController.SetTmInsumos", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar un insumo", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmInsumos tmInsumos = db.tmInsumos.Where(i => i.intIdInsmo == id).FirstOrDefault();
                        //Eliminar Compra
                        db.tmInsumos.Remove(tmInsumos);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina insumo",
                            string.Format("Insumo: {0}", tmInsumos.varDscrpcionInsmo),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmInsumos/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmInsumosController.Delete", usr.varLgin);
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
                        tmInsumos tmInsumos = db.tmInsumos.Where(i => i.intIdInsmo == id).FirstOrDefault();

                        ViewBag.listTipoMaterial = Funciones.GetListOfSelectListItem(db.tmLineas.ToList(), "varDscrpcionLnea", "intIdLnea");
                        ViewBag.listPosArancelaria = Funciones.GetListOfSelectListItem(db.tmPosicionesArancelarias.ToList(), "varPscionArnclria", "intIdPscionArnclria");
                        ViewBag.listEstadoMercancia = Funciones.GetListOfSelectListItem(db.tmComprasTipo.ToList(), "varDscrpcionCmpra", "intIdTpoCmpra");
                        ViewBag.listUnidad = Funciones.GetListOfSelectListItem(db.tmUnidades.ToList(), "varDscrpcionUndadEspnol", "intIdUndad");

                        return View(tmInsumos);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmInsumosController.Edit", usr.varLgin);
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
                        int intIdInsmo = Convert.ToInt32(Request["intIdInsmo"]);
                        tmInsumos tmInsumos = db.tmInsumos.Where(i => i.intIdInsmo == intIdInsmo).FirstOrDefault();

                        tmInsumos.varDscrpcionInsmo = Request["varDscrpcionInsmo"];
                        tmInsumos.bitInsmoNcional = true;

                        if (!string.IsNullOrWhiteSpace(Request["intIdLnea"]))
                        {
                            tmInsumos.intIdLnea = Convert.ToInt32(Request["intIdLnea"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdPscionArnclria"]))
                        {
                            tmInsumos.intIdPscionArnclria = Convert.ToInt32(Request["intIdPscionArnclria"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdUndad"]))
                        {
                            tmInsumos.intIdUndad = Convert.ToInt32(Request["intIdUndad"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdUndadDian"]))
                        {
                            tmInsumos.intIdUndadDian = Convert.ToInt32(Request["intIdUndadDian"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdUndadCmpra"]))
                        {
                            tmInsumos.intIdUndadCmpra = Convert.ToInt32(Request["intIdUndadCmpra"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdTpoCmpra"]))
                        {
                            tmInsumos.intIdTpoCmpra = Convert.ToInt32(Request["intIdTpoCmpra"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numFctorMncmex"]))
                        {
                            tmInsumos.numFctorMncmex = Convert.ToDecimal(Request["numFctorMncmex"]);
                        }

                        //Actualizar Compras
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de insumo",
                            string.Format("Insumo: {0}", tmInsumos.varDscrpcionInsmo),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmInsumos/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmInsumosController.Update", usr.varLgin);
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