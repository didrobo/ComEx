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
    public class tmResolucionesProveedorController : Controller
    {
        // GET: tmResolucionesProveedor
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetResolucionProveedor(DTParameters param, int id)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmResolucionesProveedor> listResoluciones = new List<tmResolucionesProveedor>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listResoluciones = db.tmResolucionesProveedor.AsNoTracking().Where(cd => cd.intIdPrvdor == id).ToList();
                    }

                    int count = listResoluciones.Count;

                    DTResult<tmResolucionesProveedor> resultado = new DTResult<tmResolucionesProveedor>
                    {
                        draw = param.Draw,
                        data = listResoluciones,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmResolucionesProveedorController.GetResolucionProveedor", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar una resolución", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {

                        //Valida si está asociada a una compra
                        int countCmpras = db.tmCompras.Where(x => x.intIdRslcion == id).Count();
                        if (countCmpras > 0)
                        {
                            ObjJson = new { Message = string.Format("No se puede borrar la resolución por que está asignada a {0} ordenes de compra", countCmpras), Success = false };
                            return Json(ObjJson, JsonRequestBehavior.AllowGet);
                        }

                        tmResolucionesProveedor tmResolucionesProveedor = db.tmResolucionesProveedor.Where(i => i.intIdRslcion == id).FirstOrDefault();
                        //Eliminar Plan
                        db.tmResolucionesProveedor.Remove(tmResolucionesProveedor);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina resolución",
                            string.Format("Id Proveedor: {0} Resolución: {1}", tmResolucionesProveedor.intIdPrvdor, tmResolucionesProveedor.NmroRslcion),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmProveedor/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmResolucionesProveedorController.Delete", usr.varLgin);
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

        public JsonResult CUResolucion()
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
                        tmResolucionesProveedor tmResolucionesProveedor;

                        if (operacion.Equals("Nuevo"))
                        {
                            tmResolucionesProveedor = new tmResolucionesProveedor();
                        }
                        else
                        {
                            int intIdRslcion = Convert.ToInt32(Request["intIdRslcion"]);

                            //Valida si está asociada a una compra
                            int countCmpras = db.tmCompras.Where(x => x.intIdRslcion == intIdRslcion).Count();
                            if (countCmpras > 0)
                            {
                                ObjJson = new { Message = string.Format("No se puede editar la resolución por que está asignada a {0} ordenes de compra", countCmpras), Success = false };
                                return Json(ObjJson, JsonRequestBehavior.AllowGet);
                            }

                            tmResolucionesProveedor = db.tmResolucionesProveedor.Where(i => i.intIdRslcion == intIdRslcion).FirstOrDefault();
                        }

                        tmResolucionesProveedor.NmroRslcion = Request["NmroRslcion"];
                        tmResolucionesProveedor.varPrfjo = Request["varPrfjo"].ToString().ToUpper();
                        tmResolucionesProveedor.NmroRslcionDsde = Request["NmroRslcionDsde"];
                        tmResolucionesProveedor.NmroRslcionHsta = Request["NmroRslcionHsta"];
                        tmResolucionesProveedor.bitActva = Convert.ToBoolean(Request["bitActva"]);


                        if (!string.IsNullOrWhiteSpace(Request["intIdPrvdor"]))
                        {
                            tmResolucionesProveedor.intIdPrvdor = Convert.ToInt32(Request["intIdPrvdor"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["fecRslcion"]))
                        {
                            tmResolucionesProveedor.fecRslcion = Convert.ToDateTime(Request["fecRslcion"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intVgnciaMses"]))
                        {
                            tmResolucionesProveedor.intVgnciaMses = Convert.ToInt32(Request["intVgnciaMses"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["FecVncmientoRslcion"]))
                        {
                            tmResolucionesProveedor.FecVncmientoRslcion = Convert.ToDateTime(Request["FecVncmientoRslcion"]);
                        }

                        if (operacion.Equals("Nuevo"))
                        {
                            //Guardar Resllución
                            db.tmResolucionesProveedor.Add(tmResolucionesProveedor);
                        }
                        
                            db.SaveChanges();

                            //Guarda el Log
                            tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Crear/Editar Resolución",
                                string.Format("Id Proveedor: {0} Resolución: {1}", tmResolucionesProveedor.intIdPrvdor, tmResolucionesProveedor.NmroRslcion),
                                "Manual",
                                usr.varLgin);

                            db.tcLogOperaciones.Add(mytcLogOperaciones);
                            db.SaveChanges();
                        }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmResolucionesProveedorController.CUResolucion", usr.varLgin);
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