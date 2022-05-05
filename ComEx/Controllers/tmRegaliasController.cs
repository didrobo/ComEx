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
    public class tmRegaliasController : Controller
    {
        // GET: tmRegalias
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetRegalias(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmRegalias> listRegalias = new List<tmRegalias>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listRegalias = db.tmRegalias.Include("tmLineas").ToList();
                    }


                    listRegalias.ForEach(x => { if (x.tmLineas != null) x.tmLineas.tmRegalias = null; else x.tmLineas = new tmLineas(); });

                    int count = listRegalias.Count;

                    DTResult<tmRegalias> resultado = new DTResult<tmRegalias>
                    {
                        draw = param.Draw,
                        data = listRegalias,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRegaliasController.GetRegalias", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmRegalias/Create
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
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRegaliasController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmRegalias()
        {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmRegalias tmRegalias = new tmRegalias();

                    if (!string.IsNullOrWhiteSpace(Request["intIdLnea"]))
                    {
                        tmRegalias.intIdLnea = Convert.ToInt32(Request["intIdLnea"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intAno"]))
                    {
                        tmRegalias.intAno = Convert.ToInt32(Request["intAno"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intMes"]))
                    {
                        tmRegalias.intMes = Convert.ToInt32(Request["intMes"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorPorGrmo"]))
                    {
                        tmRegalias.numVlorPorGrmo = Convert.ToDecimal(Request["numVlorPorGrmo"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["fecIncioPriodo"]))
                    {
                        tmRegalias.fecIncioPriodo = Convert.ToDateTime(Request["fecIncioPriodo"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["fecFinPriodo"]))
                    {
                        tmRegalias.fecFinPriodo = Convert.ToDateTime(Request["fecFinPriodo"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar Tipo Material
                        db.tmRegalias.Add(tmRegalias);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación regalías",
                            string.Format("Id Tipo Material: {0} Año: {1} Mes: {2}", tmRegalias.intIdLnea, tmRegalias.intAno, tmRegalias.intMes),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmRegalias/Index/" };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRegaliasController.SetTmRegalias", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar una regalía", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmRegalias tmRegalias = db.tmRegalias.Where(i => i.intIdRglia == id).FirstOrDefault();
                        //Eliminar Compra
                        db.tmRegalias.Remove(tmRegalias);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina regalía",
                            string.Format("Id Tipo Material: {0} Año: {1} Mes: {2}", tmRegalias.intIdLnea, tmRegalias.intAno, tmRegalias.intMes),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmRegalias/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRegaliasController.Delete", usr.varLgin);
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
                        tmRegalias tmRegalias = db.tmRegalias.Where(i => i.intIdRglia == id).FirstOrDefault();

                        ViewBag.listTipoMaterial = Funciones.GetListOfSelectListItem(db.tmLineas.ToList(), "varDscrpcionLnea", "intIdLnea");

                        return View(tmRegalias);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRegaliasController.Edit", usr.varLgin);
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
                        int intIdRglia = Convert.ToInt32(Request["intIdRglia"]);
                        tmRegalias tmRegalias = db.tmRegalias.Where(i => i.intIdRglia == intIdRglia).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(Request["intIdLnea"]))
                        {
                            tmRegalias.intIdLnea = Convert.ToInt32(Request["intIdLnea"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intAno"]))
                        {
                            tmRegalias.intAno = Convert.ToInt32(Request["intAno"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intMes"]))
                        {
                            tmRegalias.intMes = Convert.ToInt32(Request["intMes"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorPorGrmo"]))
                        {
                            tmRegalias.numVlorPorGrmo = Convert.ToDecimal(Request["numVlorPorGrmo"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecIncioPriodo"]))
                        {
                            tmRegalias.fecIncioPriodo = Convert.ToDateTime(Request["fecIncioPriodo"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecFinPriodo"]))
                        {
                            tmRegalias.fecFinPriodo = Convert.ToDateTime(Request["fecFinPriodo"]);
                        }

                        //Actualizar Compras
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de regalía",
                            string.Format("Id Tipo Material: {0} Año: {1} Mes: {2}", tmRegalias.intIdLnea, tmRegalias.intAno, tmRegalias.intMes),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmRegalias/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRegaliasController.Update", usr.varLgin);
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