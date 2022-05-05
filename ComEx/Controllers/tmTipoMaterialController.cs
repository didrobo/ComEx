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
    public class tmTipoMaterialController : Controller
    {
        // GET: tmTipoMaterial
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTipoMaterial(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmLineas> lisTipoMaterial = new List<tmLineas>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        lisTipoMaterial = db.tmLineas.Include("tmRetefuente").ToList();
                    }


                    lisTipoMaterial.ForEach(x => { if (x.tmRetefuente != null) x.tmRetefuente.tmLineas = null; else x.tmRetefuente = new tmRetefuente(); });

                    int count = lisTipoMaterial.Count;

                    DTResult<tmLineas> resultado = new DTResult<tmLineas>
                    {
                        draw = param.Draw,
                        data = lisTipoMaterial,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmTipoMaterialController.GetTipoMaterial", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmTipoMaterial/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        ViewBag.listReteFuente = Funciones.GetListOfSelectListItem(db.tmRetefuente.ToList(), "varDscrpcionRtncionfuente", "intIdRtncionfuente");
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmTipoMaterialController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmTipoMaterial() {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmLineas tmLineas = new tmLineas();

                    tmLineas.varCdLnea = Request["varCdLnea"];
                    tmLineas.varDscrpcionLnea = Request["varDscrpcionLnea"];
                    tmLineas.varDscrpcionDian = Request["varDscrpcionDian"];

                    if (!string.IsNullOrWhiteSpace(Request["numPrcntjeBseRglias"]))
                    {
                        tmLineas.numPrcntjeBseRglias = Convert.ToDecimal(Request["numPrcntjeBseRglias"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdRtncionfuente"]))
                    {
                        tmLineas.intIdRtncionfuente = Convert.ToInt32(Request["intIdRtncionfuente"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numFctor"]))
                    {
                        tmLineas.numFctor = Convert.ToDecimal(Request["numFctor"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar 
                        db.tmLineas.Add(tmLineas);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación tipo de material",
                            string.Format("Cod: {0} Desc: {1}", tmLineas.varCdLnea, tmLineas.varDscrpcionLnea),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmTipoMaterial/Index/" };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmTipoMaterialController.SetTmTipoMaterial", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar un tipo de material", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmLineas tmLineas = db.tmLineas.Where(i => i.intIdLnea == id).FirstOrDefault();
                        //Eliminar 
                        db.tmLineas.Remove(tmLineas);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina tipo de material",
                            string.Format("Cod: {0} Desx: {1}", tmLineas.varCdLnea, tmLineas.varDscrpcionLnea),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmTipoMaterial/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmTipoMaterialController.Delete", usr.varLgin);
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
                        tmLineas tmLineas = db.tmLineas.Where(i => i.intIdLnea == id).FirstOrDefault();

                        ViewBag.listReteFuente = Funciones.GetListOfSelectListItem(db.tmRetefuente.ToList(), "varDscrpcionRtncionfuente", "intIdRtncionfuente");

                        return View(tmLineas);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmTipoMaterialController.Edit", usr.varLgin);
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
                        int intIdLnea = Convert.ToInt32(Request["intIdLnea"]);
                        tmLineas tmLineas = db.tmLineas.Where(i => i.intIdLnea == intIdLnea).FirstOrDefault();

                        tmLineas.varCdLnea = Request["varCdLnea"];
                        tmLineas.varDscrpcionLnea = Request["varDscrpcionLnea"];
                        tmLineas.varDscrpcionDian = Request["varDscrpcionDian"];

                        if (!string.IsNullOrWhiteSpace(Request["numPrcntjeBseRglias"]))
                        {
                            tmLineas.numPrcntjeBseRglias = Convert.ToDecimal(Request["numPrcntjeBseRglias"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdRtncionfuente"]))
                        {
                            tmLineas.intIdRtncionfuente = Convert.ToInt32(Request["intIdRtncionfuente"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numFctor"]))
                        {
                            tmLineas.numFctor = Convert.ToDecimal(Request["numFctor"]);
                        }

                        //Actualizar 
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de tipo de material",
                            string.Format("Cod: {0} Desc: {1}", tmLineas.varCdLnea, tmLineas.varDscrpcionLnea),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmTipoMaterial/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmTipoMaterialController.Update", usr.varLgin);
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