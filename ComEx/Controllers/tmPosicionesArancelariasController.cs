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
    public class tmPosicionesArancelariasController : Controller
    {
        // GET: tmPosicionesArancelarias
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPosicionArancelaria(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmPosicionesArancelarias> listPosicionArancelaria = new List<tmPosicionesArancelarias>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listPosicionArancelaria = db.tmPosicionesArancelarias.Include("tmUnidades").ToList();
                    }


                    listPosicionArancelaria.ForEach(x => { 
                        if (x.tmUnidades != null) x.tmUnidades.tmPosicionesArancelarias = null; else x.tmUnidades = new tmUnidades();
                        x.tmPosicionesArancelarias1 = null;
                        x.tmPosicionesArancelarias2 = null;


                    });

                    int count = listPosicionArancelaria.Count;

                    DTResult<tmPosicionesArancelarias> resultado = new DTResult<tmPosicionesArancelarias>
                    {
                        draw = param.Draw,
                        data = listPosicionArancelaria,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPosicionesArancelariasController.GetPosicionArancelaria", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmPosicionesArancelarias/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        ViewBag.listUnidad = Funciones.GetListOfSelectListItem(db.tmUnidades.ToList(), "varDscrpcionUndadEspnol", "intIdUndad");
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPosicionesArancelariasController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmPosicionArancelaria()
        {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmPosicionesArancelarias tmPosicionesArancelarias = new tmPosicionesArancelarias();

                    tmPosicionesArancelarias.varPscionArnclria = Request["varPscionArnclria"];
                    tmPosicionesArancelarias.varDscrpcionPscionArnclria = Request["varDscrpcionPscionArnclria"];

                    if (!string.IsNullOrWhiteSpace(Request["numPrcntjeArncel"]))
                    {
                        tmPosicionesArancelarias.numPrcntjeArncel = Convert.ToDecimal(Request["numPrcntjeArncel"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdUndad"]))
                    {
                        tmPosicionesArancelarias.intIdUndad = Convert.ToInt32(Request["intIdUndad"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numPrcntjeIva"]))
                    {
                        tmPosicionesArancelarias.numPrcntjeIva = Convert.ToDecimal(Request["numPrcntjeIva"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar Tipo Material
                        db.tmPosicionesArancelarias.Add(tmPosicionesArancelarias);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación posición arancelaria",
                            string.Format("P. Arancelaria: {0}", tmPosicionesArancelarias.varPscionArnclria),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmPosicionesArancelarias/Index/" };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPosicionesArancelariasController.SetTmPosicionArancelaria", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar una posición arancelaria", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmPosicionesArancelarias tmPosicionesArancelarias = db.tmPosicionesArancelarias.Where(i => i.intIdPscionArnclria == id).FirstOrDefault();
                        //Eliminar Compra
                        db.tmPosicionesArancelarias.Remove(tmPosicionesArancelarias);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina posición arancelaria",
                            string.Format("P. Arancelaria: {0}", tmPosicionesArancelarias.varPscionArnclria),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmPosicionesArancelarias/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPosicionesArancelariasController.Delete", usr.varLgin);
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
                        tmPosicionesArancelarias tmPosicionesArancelarias = db.tmPosicionesArancelarias.Where(i => i.intIdPscionArnclria == id).FirstOrDefault();

                        ViewBag.listUnidad = Funciones.GetListOfSelectListItem(db.tmUnidades.ToList(), "varDscrpcionUndadEspnol", "intIdUndad");

                        return View(tmPosicionesArancelarias);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPosicionesArancelariasController.Edit", usr.varLgin);
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
                        int intIdPscionArnclria = Convert.ToInt32(Request["intIdPscionArnclria"]);
                        tmPosicionesArancelarias tmPosicionesArancelarias = db.tmPosicionesArancelarias.Where(i => i.intIdPscionArnclria == intIdPscionArnclria).FirstOrDefault();

                        tmPosicionesArancelarias.varPscionArnclria = Request["varPscionArnclria"];
                        tmPosicionesArancelarias.varDscrpcionPscionArnclria = Request["varDscrpcionPscionArnclria"];

                        if (!string.IsNullOrWhiteSpace(Request["numPrcntjeArncel"]))
                        {
                            tmPosicionesArancelarias.numPrcntjeArncel = Convert.ToDecimal(Request["numPrcntjeArncel"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdUndad"]))
                        {
                            tmPosicionesArancelarias.intIdUndad = Convert.ToInt32(Request["intIdUndad"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numPrcntjeIva"]))
                        {
                            tmPosicionesArancelarias.numPrcntjeIva = Convert.ToDecimal(Request["numPrcntjeIva"]);
                        }

                        //Actualizar
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de posición arancelaria",
                            string.Format("P. Arancelaria: {0}", tmPosicionesArancelarias.varPscionArnclria),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmPosicionesArancelarias/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmPosicionesArancelariasController.Update", usr.varLgin);
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