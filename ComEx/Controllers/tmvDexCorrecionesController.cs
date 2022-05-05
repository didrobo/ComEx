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
    public class tmvDexCorrecionesController : Controller
    {
        // GET: tmvDexCorreciones
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCorreccionesDEX(DTParameters param, int id)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmvDexCorreciones> listCorrecciones = new List<tmvDexCorreciones>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listCorrecciones = db.tmvDexCorreciones.AsNoTracking().Where(cd => cd.intIdDEX == id).ToList();
                    }

                    int count = listCorrecciones.Count;

                    DTResult<tmvDexCorreciones> resultado = new DTResult<tmvDexCorreciones>
                    {
                        draw = param.Draw,
                        data = listCorrecciones,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDexCorrecionesController.GetCorreccionesDEX", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
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
                        ObjJson = new { Message = "Asegúrese de seleccionar una corrección", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmvDexCorreciones tmvDexCorreciones = db.tmvDexCorreciones.Where(i => i.intIdDEXCorrcions == id).FirstOrDefault();
                        //Eliminar Plan
                        db.tmvDexCorreciones.Remove(tmvDexCorreciones);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina corrección DEX",
                            string.Format("Id DEX: {0} DEX: {1} DEX Corregido: {2}", tmvDexCorreciones.intIdDEX, tmvDexCorreciones.varNmroDEX, tmvDexCorreciones.varNmroDEXCrrgdo),
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
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDexCorrecionesController.Delete", usr.varLgin);
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

        public JsonResult CUCorreccion()
        {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {

                        string operacion = Request["operacion"];
                        tmvDexCorreciones tmvDexCorreciones;

                        if (operacion.Equals("Nuevo"))
                        {
                            tmvDexCorreciones = new tmvDexCorreciones();
                        }
                        else
                        {
                            int intIdDEXCorrcions = Convert.ToInt32(Request["intIdDEXCorrcions"]);
                            tmvDexCorreciones = db.tmvDexCorreciones.Where(i => i.intIdDEXCorrcions == intIdDEXCorrcions).FirstOrDefault();
                        }

                        tmvDexCorreciones.varNmroDEX = Request["varNmroDEX"];
                        tmvDexCorreciones.varNmroDEXCrrgdo = Request["varNmroDEXCrrgdo"];
                        tmvDexCorreciones.varRtaArchvoAdjnto = Request["varRtaArchvoAdjnto"];


                        if (!string.IsNullOrWhiteSpace(Request["intIdDEX"]))
                        {
                            tmvDexCorreciones.intIdDEX = Convert.ToInt32(Request["intIdDEX"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecAprbcionDEX"]))
                        {
                            tmvDexCorreciones.fecAprbcionDEX = Convert.ToDateTime(Request["fecAprbcionDEX"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecAprbcionDEXCrrgdo"]))
                        {
                            tmvDexCorreciones.fecAprbcionDEXCrrgdo = Convert.ToDateTime(Request["fecAprbcionDEXCrrgdo"]);
                        }

                        if (operacion.Equals("Nuevo"))
                        {
                            //Guardar Corrección
                            db.tmvDexCorreciones.Add(tmvDexCorreciones);
                        }

                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Crear/Editar Corrección",
                            string.Format("Id DEX: {0} DEX: {1} DEX Corregido: {2}", tmvDexCorreciones.intIdDEX, tmvDexCorreciones.varNmroDEX, tmvDexCorreciones.varNmroDEXCrrgdo),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDexCorrecionesController.CUCorreccion", usr.varLgin);
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