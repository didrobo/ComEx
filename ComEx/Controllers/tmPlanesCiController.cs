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
    public class tmPlanesCiController : Controller
    {
        // GET: tmPlanesCi
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPlanesCI(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmPlanesCI> listPlanesCI = new List<tmPlanesCI>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listPlanesCI = db.tmPlanesCI.Include("tmImportadorExportador").ToList();
                    }


                    listPlanesCI.ForEach(x => { if (x.tmImportadorExportador != null) x.tmImportadorExportador.tmPlanesCI = null; else x.tmImportadorExportador = new tmImportadorExportador(); });

                    int count = listPlanesCI.Count;

                    DTResult<tmPlanesCI> resultado = new DTResult<tmPlanesCI>
                    {
                        draw = param.Draw,
                        data = listPlanesCI,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPlanesCiController.GetPlanesCI", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmPlanesCi/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        ViewBag.listExportador = Funciones.GetListOfSelectListItem(db.tmImportadorExportador.ToList(), "varNmbre", "intIdImprtdorExprtdor");
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPlanesCiController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmPlanesCI()
        {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmPlanesCI tmPlanesCI = new tmPlanesCI();

                    tmPlanesCI.varCdPlanCI = Request["varCdPlanCI"];
                    tmPlanesCI.varDscrpcionPlanCI = Request["varDscrpcionPlanCI"];
                    tmPlanesCI.bitBloqueo = Convert.ToBoolean(Request["bitBloqueo"]);

                    if (!string.IsNullOrWhiteSpace(Request["intIdImprtdorExprtdor"]))
                    {
                        tmPlanesCI.intIdImprtdorExprtdor = Convert.ToInt32(Request["intIdImprtdorExprtdor"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["FecPrsntcion"]))
                    {
                        tmPlanesCI.FecPrsntcion = Convert.ToDateTime(Request["FecPrsntcion"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["FecFnalExprtcion"]))
                    {
                        tmPlanesCI.FecFnalExprtcion = Convert.ToDateTime(Request["FecFnalExprtcion"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["FecFnalCmpras"]))
                    {
                        tmPlanesCI.FecFnalCmpras = Convert.ToDateTime(Request["FecFnalCmpras"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar Tipo Material
                        db.tmPlanesCI.Add(tmPlanesCI);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación planes CI",
                            string.Format("Código: {0} Descripción: {1}", tmPlanesCI.varCdPlanCI, tmPlanesCI.varDscrpcionPlanCI),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmPlanesCi/Index/" };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPlanesCiController.SetTmPlanesCI", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar un plan", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmPlanesCI tmPlanesCI = db.tmPlanesCI.Where(i => i.intIdPlanCI == id).FirstOrDefault();
                        //Eliminar Plan
                        db.tmPlanesCI.Remove(tmPlanesCI);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina Plan CI",
                            string.Format("Código: {0} Descripción: {1}", tmPlanesCI.varCdPlanCI, tmPlanesCI.varDscrpcionPlanCI),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmPlanesCi/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPlanesCiController.Delete", usr.varLgin);
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
                        tmPlanesCI tmPlanesCI = db.tmPlanesCI.Where(i => i.intIdPlanCI == id).FirstOrDefault();

                        ViewBag.listExportador = Funciones.GetListOfSelectListItem(db.tmImportadorExportador.ToList(), "varNmbre", "intIdImprtdorExprtdor");

                        return View(tmPlanesCI);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPlanesCiController.Edit", usr.varLgin);
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
                        int intIdPlanCI = Convert.ToInt32(Request["intIdPlanCI"]);
                        tmPlanesCI tmPlanesCI = db.tmPlanesCI.Where(i => i.intIdPlanCI == intIdPlanCI).FirstOrDefault();

                        tmPlanesCI.varCdPlanCI = Request["varCdPlanCI"];
                        tmPlanesCI.varDscrpcionPlanCI = Request["varDscrpcionPlanCI"];
                        tmPlanesCI.bitBloqueo = Convert.ToBoolean(Request["bitBloqueo"]);

                        if (!string.IsNullOrWhiteSpace(Request["intIdImprtdorExprtdor"]))
                        {
                            tmPlanesCI.intIdImprtdorExprtdor = Convert.ToInt32(Request["intIdImprtdorExprtdor"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["FecPrsntcion"]))
                        {
                            tmPlanesCI.FecPrsntcion = Convert.ToDateTime(Request["FecPrsntcion"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["FecFnalExprtcion"]))
                        {
                            tmPlanesCI.FecFnalExprtcion = Convert.ToDateTime(Request["FecFnalExprtcion"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["FecFnalCmpras"]))
                        {
                            tmPlanesCI.FecFnalCmpras = Convert.ToDateTime(Request["FecFnalCmpras"]);
                        }

                        //Actualizar Compras
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de plan CI",
                            string.Format("Código: {0} Descripción: {1}", tmPlanesCI.varCdPlanCI, tmPlanesCI.varDscrpcionPlanCI),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmPlanesCi/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPlanesCiController.Update", usr.varLgin);
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