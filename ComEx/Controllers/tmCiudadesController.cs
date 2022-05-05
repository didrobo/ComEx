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
    public class tmCiudadesController : Controller
    {
        // GET: tmCiudades
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCiudad(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmCiudades> lisCiudad = new List<tmCiudades>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        lisCiudad = db.tmCiudades.Include("tmDepartamentos").ToList();
                    }


                    lisCiudad.ForEach(x => { if (x.tmDepartamentos != null) x.tmDepartamentos.tmCiudades = null; else x.tmDepartamentos = new tmDepartamentos(); });

                    int count = lisCiudad.Count;

                    DTResult<tmCiudades> resultado = new DTResult<tmCiudades>
                    {
                        draw = param.Draw,
                        data = lisCiudad,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCiudadesController.GetCiudad", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmCiudades/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        ViewBag.listDepartamento = Funciones.GetListOfSelectListItem(db.tmDepartamentos.ToList(), "varDscrpcionDprtmnto", "intIdDprtmnto");
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCiudadesController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmCiudad()
        {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmCiudades tmCiudades = new tmCiudades();

                    tmCiudades.varCdCiudad = Request["varCdCiudad"];
                    tmCiudades.varDscrpcionCiudad = Request["varDscrpcionCiudad"];
                    tmCiudades.varAbrvtra = Request["varAbrvtra"];
                    tmCiudades.varCd = Request["varCd"];

                    if (!string.IsNullOrWhiteSpace(Request["intIdDprtmnto"]))
                    {
                        tmCiudades.intIdDprtmnto = Convert.ToInt32(Request["intIdDprtmnto"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar 
                        db.tmCiudades.Add(tmCiudades);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación ciudad",
                            string.Format("Cod: {0} Desc: {1}", tmCiudades.varCdCiudad, tmCiudades.varDscrpcionCiudad),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmCiudades/Index/" };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCiudadesController.SetTmCiudad", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar una ciudad", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmCiudades tmCiudades = db.tmCiudades.Where(i => i.intIdCiudad == id).FirstOrDefault();
                        //Eliminar 
                        db.tmCiudades.Remove(tmCiudades);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina ciudad",
                            string.Format("Cod: {0} Desc: {1}", tmCiudades.varCdCiudad, tmCiudades.varDscrpcionCiudad),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmCiudades/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCiudadesController.Delete", usr.varLgin);
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
                        tmCiudades tmCiudades = db.tmCiudades.Where(i => i.intIdCiudad == id).FirstOrDefault();

                        ViewBag.listDepartamento = Funciones.GetListOfSelectListItem(db.tmDepartamentos.ToList(), "varDscrpcionDprtmnto", "intIdDprtmnto");

                        return View(tmCiudades);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCiudadesController.Edit", usr.varLgin);
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
                        int intIdCiudad = Convert.ToInt32(Request["intIdCiudad"]);
                        tmCiudades tmCiudades = db.tmCiudades.Where(i => i.intIdCiudad == intIdCiudad).FirstOrDefault();

                        tmCiudades.varCdCiudad = Request["varCdCiudad"];
                        tmCiudades.varDscrpcionCiudad = Request["varDscrpcionCiudad"];
                        tmCiudades.varAbrvtra = Request["varAbrvtra"];
                        tmCiudades.varCd = Request["varCd"];

                        if (!string.IsNullOrWhiteSpace(Request["intIdDprtmnto"]))
                        {
                            tmCiudades.intIdDprtmnto = Convert.ToInt32(Request["intIdDprtmnto"]);
                        }

                        //Actualizar 
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de ciudad",
                            string.Format("Cod: {0} Desc: {1}", tmCiudades.varCdCiudad, tmCiudades.varDscrpcionCiudad),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmCiudades/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCiudadesController.Update", usr.varLgin);
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