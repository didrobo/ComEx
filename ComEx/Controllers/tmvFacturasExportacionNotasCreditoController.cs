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
    public class tmvFacturasExportacionNotasCreditoController : Controller
    {
        // GET: tmvFacturasExportacionNotasCredito
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetNotasCreditoFactExp(DTParameters param, int id)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmvFacturasExportacionNotasCredito> listNotasCredito = new List<tmvFacturasExportacionNotasCredito>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listNotasCredito = db.tmvFacturasExportacionNotasCredito.Include("tmTiposDescuentos").Where(cd => cd.intIdFctraExprtcion == id).ToList();
                    }

                    listNotasCredito.ForEach(x => { if (x.tmTiposDescuentos != null) x.tmTiposDescuentos.tmvFacturasExportacionNotasCredito = null; else x.tmTiposDescuentos = new tmTiposDescuentos(); });

                    int count = listNotasCredito.Count;

                    DTResult<tmvFacturasExportacionNotasCredito> resultado = new DTResult<tmvFacturasExportacionNotasCredito>
                    {
                        draw = param.Draw,
                        data = listNotasCredito,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionNotasCreditoController.GetNotasCreditoFactExp", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar una nota crédito", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmvFacturasExportacionNotasCredito tmvFacturasExportacionNotasCredito = db.tmvFacturasExportacionNotasCredito.Where(i => i.intIdFctraExprtcionNtaCrdto == id).FirstOrDefault();
                        //Eliminar 
                        db.tmvFacturasExportacionNotasCredito.Remove(tmvFacturasExportacionNotasCredito);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina nota crédito Fact Exp",
                            string.Format("Id Fact Exp: {0} Nota Crédto: {1}", tmvFacturasExportacionNotasCredito.intIdFctraExprtcion, tmvFacturasExportacionNotasCredito.varNmroNtaCrdto),
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
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionNotasCreditoController.Delete", usr.varLgin);
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

        public JsonResult CUNotaCredito()
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
                        tmvFacturasExportacionNotasCredito tmvFacturasExportacionNotasCredito;

                        if (operacion.Equals("Nuevo"))
                        {
                            tmvFacturasExportacionNotasCredito = new tmvFacturasExportacionNotasCredito();
                        }
                        else
                        {
                            int intIdFctraExprtcionNtaCrdto = Convert.ToInt32(Request["intIdFctraExprtcionNtaCrdto"]);
                            tmvFacturasExportacionNotasCredito = db.tmvFacturasExportacionNotasCredito.Where(i => i.intIdFctraExprtcionNtaCrdto == intIdFctraExprtcionNtaCrdto).FirstOrDefault();
                        }

                        tmvFacturasExportacionNotasCredito.varNmroNtaCrdto = Request["varNmroNtaCrdto"];
                        tmvFacturasExportacionNotasCredito.varObsrvciones = Request["varObsrvciones"];


                        if (!string.IsNullOrWhiteSpace(Request["intIdFctraExprtcion"]))
                        {
                            tmvFacturasExportacionNotasCredito.intIdFctraExprtcion = Convert.ToInt32(Request["intIdFctraExprtcion"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdTpoDscuento"]))
                        {
                            tmvFacturasExportacionNotasCredito.intIdTpoDscuento = Convert.ToInt32(Request["intIdTpoDscuento"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecAsientoNtaCrdto"]))
                        {
                            tmvFacturasExportacionNotasCredito.fecAsientoNtaCrdto = Convert.ToDateTime(Request["fecAsientoNtaCrdto"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorNtaCrdto"]))
                        {
                            tmvFacturasExportacionNotasCredito.numVlorNtaCrdto = Convert.ToDecimal(Request["numVlorNtaCrdto"]);
                        }


                        if (operacion.Equals("Nuevo"))
                        {
                            //Guardar Corrección
                            db.tmvFacturasExportacionNotasCredito.Add(tmvFacturasExportacionNotasCredito);
                        }

                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Crear/Editar Nota Crédito Fac Exp.",
                            string.Format("Id Fact Exp: {0} Nota Crédto: {1}", tmvFacturasExportacionNotasCredito.intIdFctraExprtcion, tmvFacturasExportacionNotasCredito.varNmroNtaCrdto),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionNotasCreditoController.CUNotaCredito", usr.varLgin);
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

        public JsonResult GetTipoDescuento()
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmTiposDescuentos> listTipoDescuento = new List<tmTiposDescuentos>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listTipoDescuento = db.tmTiposDescuentos.ToList();
                    }

                    result = Json(listTipoDescuento, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionNotasCreditoController.GetTipoDescuento", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException }, JsonRequestBehavior.AllowGet);
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