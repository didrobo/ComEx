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
    public class tmvFacturasExportacionDetalleController : Controller
    {
        // GET: tmvFacturasExportacionDetalle
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetFacturasDetalle(DTParameters param, int id)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmvFacturasExportacionDetalle> listFactDetalle = new List<tmvFacturasExportacionDetalle>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        db.Configuration.ProxyCreationEnabled = false;
                        listFactDetalle = db.tmvFacturasExportacionDetalle.Include("tmvProductosFacturacion").Where(cd => cd.intIdFctraExprtcion == id).ToList();
                    }

                    listFactDetalle.ForEach(x => { if (x.tmvProductosFacturacion != null) x.tmvProductosFacturacion.tmvFacturasExportacionDetalle = null; else x.tmvProductosFacturacion = new tmvProductosFacturacion(); });

                    int count = listFactDetalle.Count;

                    DTResult<tmvFacturasExportacionDetalle> resultado = new DTResult<tmvFacturasExportacionDetalle>
                    {
                        draw = param.Draw,
                        data = listFactDetalle,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionDetalleController.GetFacturasDetalle", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public ActionResult Create(int id)
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        ViewBag.lstProducto = Funciones.GetListOfSelectListItem(db.tmvProductosFacturacion.ToList(), "varDscrpcionEspñol", "intIdPrdctoFctrcion");

                        //Enviar id
                        ViewBag.Id = id.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionDetalleController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmFacturaDetalle()
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmvFacturasExportacionDetalle tmvFacturasExportacionDetalle = new tmvFacturasExportacionDetalle();

                    tmvFacturasExportacionDetalle.intIdFctraExprtcion = Convert.ToInt32(Request["intIdFctraExprtcion"]);
                    tmvFacturasExportacionDetalle.intIdPrdctoFctrcion = Convert.ToInt32(Request["intIdPrdctoFctrcion"]);
                    tmvFacturasExportacionDetalle.numCntdad = Convert.ToDecimal(Request["numCntdad"]);
                    tmvFacturasExportacionDetalle.numVlorUntrio = Convert.ToDecimal(Request["numVlorUntrio"]);
                    tmvFacturasExportacionDetalle.numPsoBrto = Convert.ToDecimal(Request["numPsoBrto"]);
                    tmvFacturasExportacionDetalle.numPsoNto = Convert.ToDecimal(Request["numPsoNto"]);
                    tmvFacturasExportacionDetalle.numSbttal = Convert.ToDecimal(Request["numSbttal"]);


                    if (!string.IsNullOrWhiteSpace(Request["intItem"]))
                    {
                        tmvFacturasExportacionDetalle.intItem = Convert.ToInt32(Request["intItem"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar
                        db.tmvFacturasExportacionDetalle.Add(tmvFacturasExportacionDetalle);
                        db.SaveChanges();

                        //Recuperar los datos de la factura
                        tmvFacturasExportacionEncabezado tmvFacturasExportacionEncabezado = db.tmvFacturasExportacionEncabezado.Where(i => i.intIdFctraExprtcion == tmvFacturasExportacionDetalle.intIdFctraExprtcion).FirstOrDefault();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación del detalle factura de exportación",
                            string.Format("Fac: {0} Export: {1} Lote: {2}", tmvFacturasExportacionEncabezado.varNmroFctra, tmvFacturasExportacionEncabezado.varDcmntoTrnsprte, tmvFacturasExportacionEncabezado.varNmroExprtcion),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvFacturasExportacionEncabezado/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionDetalleController.SetTmFacturaDetalle", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar el detalle de la factura", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmvFacturasExportacionDetalle tmvFacturasExportacionDetalle = db.tmvFacturasExportacionDetalle.Where(i => i.intIdFctraExprtcionDtlle == id).FirstOrDefault();
                        tmvFacturasExportacionEncabezado tmvFacturasExportacionEncabezado = db.tmvFacturasExportacionEncabezado.Where(i => i.intIdFctraExprtcion == tmvFacturasExportacionDetalle.intIdFctraExprtcion).FirstOrDefault();
                        //Eliminar 
                        db.tmvFacturasExportacionDetalle.Remove(tmvFacturasExportacionDetalle);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina el detalle de factura de exportación",
                            string.Format("Fac: {0} Export: {1} Lote: {2}", tmvFacturasExportacionEncabezado.varNmroFctra, tmvFacturasExportacionEncabezado.varDcmntoTrnsprte, tmvFacturasExportacionEncabezado.varNmroExprtcion),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvFacturasExportacionEncabezado/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionDetalleController.Delete", usr.varLgin);
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
                        tmvFacturasExportacionDetalle tmvFacturasExportacionDetalle = db.tmvFacturasExportacionDetalle.Where(i => i.intIdFctraExprtcionDtlle == id).FirstOrDefault();

                        ViewBag.lstProducto = Funciones.GetListOfSelectListItem(db.tmvProductosFacturacion.ToList(), "varDscrpcionEspñol", "intIdPrdctoFctrcion");

                        return View(tmvFacturasExportacionDetalle);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionDetalleController.Edit", usr.varLgin);
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
                        int intIdFctraExprtcionDtlle = Convert.ToInt32(Request["intIdFctraExprtcionDtlle"]);
                        tmvFacturasExportacionDetalle tmvFacturasExportacionDetalle = db.tmvFacturasExportacionDetalle.Where(i => i.intIdFctraExprtcionDtlle == intIdFctraExprtcionDtlle).FirstOrDefault();

                        tmvFacturasExportacionDetalle.intIdPrdctoFctrcion = Convert.ToInt32(Request["intIdPrdctoFctrcion"]);
                        tmvFacturasExportacionDetalle.numCntdad = Convert.ToDecimal(Request["numCntdad"]);
                        tmvFacturasExportacionDetalle.numVlorUntrio = Convert.ToDecimal(Request["numVlorUntrio"]);
                        tmvFacturasExportacionDetalle.numPsoBrto = Convert.ToDecimal(Request["numPsoBrto"]);
                        tmvFacturasExportacionDetalle.numPsoNto = Convert.ToDecimal(Request["numPsoNto"]);
                        tmvFacturasExportacionDetalle.numSbttal = Convert.ToDecimal(Request["numSbttal"]);


                        if (!string.IsNullOrWhiteSpace(Request["intItem"]))
                        {
                            tmvFacturasExportacionDetalle.intItem = Convert.ToInt32(Request["intItem"]);
                        }

                        //Actualizar 
                        db.SaveChanges();

                        //Recuperar los datos de la factura
                        tmvFacturasExportacionEncabezado tmvFacturasExportacionEncabezado = db.tmvFacturasExportacionEncabezado.Where(i => i.intIdFctraExprtcion == tmvFacturasExportacionDetalle.intIdFctraExprtcion).FirstOrDefault();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización del detalle factura de exportación",
                            string.Format("Fac: {0} Export: {1} Lote: {2}", tmvFacturasExportacionEncabezado.varNmroFctra, tmvFacturasExportacionEncabezado.varDcmntoTrnsprte, tmvFacturasExportacionEncabezado.varNmroExprtcion),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmvFacturasExportacionEncabezado/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvFacturasExportacionDetalleController.Update", usr.varLgin);
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