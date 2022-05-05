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
    public class tmvProductosFacturacionController : Controller
    {
        // GET: tmvProductosFacturacion
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProductoFacturacion(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmvProductosFacturacion> listProductoFacturacion = new List<tmvProductosFacturacion>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listProductoFacturacion = db.tmvProductosFacturacion.Include("tmLineas").Include("tmPosicionesArancelarias").Include("tmUnidades").ToList();
                    }

                    listProductoFacturacion.ForEach(x => { if (x.tmLineas != null) x.tmLineas.tmvProductosFacturacion = null; else x.tmLineas = new tmLineas(); });

                    listProductoFacturacion.ForEach(x => {
                        if (x.tmPosicionesArancelarias != null)
                        {
                            x.tmPosicionesArancelarias.tmvProductosFacturacion = null;
                            x.tmPosicionesArancelarias.tmUnidades = null;
                            x.tmPosicionesArancelarias.tmPosicionesArancelarias1 = null;
                            x.tmPosicionesArancelarias.tmPosicionesArancelarias2 = null;
                        }
                        else x.tmPosicionesArancelarias = new tmPosicionesArancelarias();
                    });

                    listProductoFacturacion.ForEach(x => {
                        if (x.tmUnidades != null)
                        {
                            x.tmUnidades.tmvProductosFacturacion = null;
                            x.tmUnidades.tmPosicionesArancelarias = null;
                        }
                        else x.tmUnidades = new tmUnidades();
                    });

                    int count = listProductoFacturacion.Count;

                    DTResult<tmvProductosFacturacion> resultado = new DTResult<tmvProductosFacturacion>
                    {
                        draw = param.Draw,
                        data = listProductoFacturacion,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvProductosFacturacionController.GetProductoFacturacion", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmvProductosFacturacion/Create
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
                        ViewBag.listUnidad = Funciones.GetListOfSelectListItem(db.tmUnidades.ToList(), "varDscrpcionUndadEspnol", "intIdUndad");
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvProductosFacturacionController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmvProductoFacturacion()
        {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmvProductosFacturacion tmvProductosFacturacion = new tmvProductosFacturacion();

                    tmvProductosFacturacion.varCdPrdcto = Request["varCdPrdcto"];
                    tmvProductosFacturacion.varDscrpcionEspñol = Request["varDscrpcionEspñol"];


                    if (!string.IsNullOrWhiteSpace(Request["intIdLnea"]))
                    {
                        tmvProductosFacturacion.intIdLnea = Convert.ToInt32(Request["intIdLnea"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdPscionArnclria"]))
                    {
                        tmvProductosFacturacion.intIdPscionArnclria = Convert.ToInt32(Request["intIdPscionArnclria"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdUndad"]))
                    {
                        tmvProductosFacturacion.intIdUndad = Convert.ToInt32(Request["intIdUndad"]);
                    }                    

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar Tipo Material
                        db.tmvProductosFacturacion.Add(tmvProductosFacturacion);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación producto facturación",
                            string.Format("Producto Fac.: {0}", tmvProductosFacturacion.varDscrpcionEspñol),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvProductosFacturacion/Index/" };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvProductosFacturacionController.SetTmvProductoFacturacion", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar un producto de facturación", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmvProductosFacturacion tmvProductosFacturacion = db.tmvProductosFacturacion.Where(i => i.intIdPrdctoFctrcion == id).FirstOrDefault();
                        //Eliminar Compra
                        db.tmvProductosFacturacion.Remove(tmvProductosFacturacion);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina producto de facturación",
                            string.Format("Producto Fac.: {0}", tmvProductosFacturacion.varDscrpcionEspñol),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvProductosFacturacion/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvProductosFacturacionController.Delete", usr.varLgin);
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
                        tmvProductosFacturacion tmvProductosFacturacion = db.tmvProductosFacturacion.Where(i => i.intIdPrdctoFctrcion == id).FirstOrDefault();

                        ViewBag.listTipoMaterial = Funciones.GetListOfSelectListItem(db.tmLineas.ToList(), "varDscrpcionLnea", "intIdLnea");
                        ViewBag.listPosArancelaria = Funciones.GetListOfSelectListItem(db.tmPosicionesArancelarias.ToList(), "varPscionArnclria", "intIdPscionArnclria");                        
                        ViewBag.listUnidad = Funciones.GetListOfSelectListItem(db.tmUnidades.ToList(), "varDscrpcionUndadEspnol", "intIdUndad");

                        return View(tmvProductosFacturacion);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvProductosFacturacionController.Edit", usr.varLgin);
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
                        int intIdPrdctoFctrcion = Convert.ToInt32(Request["intIdPrdctoFctrcion"]);
                        tmvProductosFacturacion tmvProductosFacturacion = db.tmvProductosFacturacion.Where(i => i.intIdPrdctoFctrcion == intIdPrdctoFctrcion).FirstOrDefault();

                        tmvProductosFacturacion.varCdPrdcto = Request["varCdPrdcto"];
                        tmvProductosFacturacion.varDscrpcionEspñol = Request["varDscrpcionEspñol"];


                        if (!string.IsNullOrWhiteSpace(Request["intIdLnea"]))
                        {
                            tmvProductosFacturacion.intIdLnea = Convert.ToInt32(Request["intIdLnea"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdPscionArnclria"]))
                        {
                            tmvProductosFacturacion.intIdPscionArnclria = Convert.ToInt32(Request["intIdPscionArnclria"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdUndad"]))
                        {
                            tmvProductosFacturacion.intIdUndad = Convert.ToInt32(Request["intIdUndad"]);
                        }

                        //Actualizar Compras
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de producto de facturación",
                            string.Format("Producto Fac.: {0}", tmvProductosFacturacion.varDscrpcionEspñol),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmvProductosFacturacion/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvProductosFacturacionController.Update", usr.varLgin);
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