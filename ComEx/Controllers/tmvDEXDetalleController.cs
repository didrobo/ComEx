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
    public class tmvDEXDetalleController : Controller
    {
        // GET: tmvDEXDetalle
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDexDetalle(DTParameters param, int id)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vDexDetalleComex> listDexDetalle = new List<vDexDetalleComex>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        listDexDetalle = db.vDexDetalleComex.AsNoTracking().Where(cd => cd.intIdDEX == id).ToList();
                    }

                    int count = listDexDetalle.Count;

                    DTResult<vDexDetalleComex> resultado = new DTResult<vDexDetalleComex>
                    {
                        draw = param.Draw,
                        data = listDexDetalle,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXDetalleController.GetDexDetalle", usr.varLgin);
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
                        ViewBag.lstProducto = Funciones.GetListOfSelectListItem(db.tmCuadrosInsumoProducto.ToList(), "varCdgoOrdnrio", "intIdCuadroInsmoPrdcto");

                        //Enviar id
                        ViewBag.Id = id.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXDetalleController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmDexDetalle()
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmvDEXDetalle tmvDEXDetalle = new tmvDEXDetalle();

                    tmvDEXDetalle.intIdDEX = Convert.ToInt32(Request["intIdDEX"]);
                    tmvDEXDetalle.intIdCuadroInsmoPrdcto = Convert.ToInt32(Request["intIdCuadroInsmoPrdcto"]);
                    tmvDEXDetalle.numCntdadExprtda = Convert.ToDecimal(Request["numCntdadExprtda"]);
                    tmvDEXDetalle.numVlorFOB = Convert.ToDecimal(Request["numVlorFOB"]);
                    tmvDEXDetalle.numPsoNto = Convert.ToDecimal(Request["numPsoNto"]);
                    tmvDEXDetalle.varCmntriosDEX = Request["varCmntriosDEX"];


                    if (!string.IsNullOrWhiteSpace(Request["varCdItem"]))
                    {
                        tmvDEXDetalle.varCdItem = Convert.ToInt32(Request["varCdItem"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numVlorReintrgro"]))
                    {
                        tmvDEXDetalle.numVlorReintrgro = Convert.ToDecimal(Request["numVlorReintrgro"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numCntdadCnvrtir"]))
                    {
                        tmvDEXDetalle.numCntdadCnvrtir = Convert.ToDecimal(Request["numCntdadCnvrtir"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numCntdadAdcional"]))
                    {
                        tmvDEXDetalle.numCntdadAdcional = Convert.ToDecimal(Request["numCntdadAdcional"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numPsoBrto"]))
                    {
                        tmvDEXDetalle.numPsoBrto = Convert.ToDecimal(Request["numPsoBrto"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar
                        db.tmvDEXDetalle.Add(tmvDEXDetalle);
                        db.SaveChanges();

                        //Recuperar los datos del DEX
                        tmvDEXEncabezado tmvDEXEncabezado = db.tmvDEXEncabezado.Where(i => i.intIdDEX == tmvDEXDetalle.intIdDEX).FirstOrDefault();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación del detalle DEX",
                            string.Format("Aux: {0}", tmvDEXEncabezado.varCdAuxliar),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmvDEXEncabezado/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXDetalleController.SetTmDexDetalle", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar el detalle del DEX", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmvDEXDetalle tmvDEXDetalle = db.tmvDEXDetalle.Where(i => i.intIdDEXDtlle == id).FirstOrDefault();
                        tmvDEXEncabezado tmvDEXEncabezado = db.tmvDEXEncabezado.Where(i => i.intIdDEX == tmvDEXDetalle.intIdDEX).FirstOrDefault();
                        //Eliminar 
                        db.tmvDEXDetalle.Remove(tmvDEXDetalle);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina el detalle de DEX",
                            string.Format("Aux: {0}", tmvDEXEncabezado.varCdAuxliar),
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
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXDetalleController.Delete", usr.varLgin);
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
                        tmvDEXDetalle tmvDEXDetalle = db.tmvDEXDetalle.Where(i => i.intIdDEXDtlle == id).FirstOrDefault();

                        ViewBag.lstProducto = Funciones.GetListOfSelectListItem(db.tmCuadrosInsumoProducto.ToList(), "varCdgoOrdnrio", "intIdCuadroInsmoPrdcto");

                        return View(tmvDEXDetalle);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXDetalleController.Edit", usr.varLgin);
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
                        int intIdDEXDtlle = Convert.ToInt32(Request["intIdDEXDtlle"]);
                        tmvDEXDetalle tmvDEXDetalle = db.tmvDEXDetalle.Where(i => i.intIdDEXDtlle == intIdDEXDtlle).FirstOrDefault();

                        tmvDEXDetalle.intIdCuadroInsmoPrdcto = Convert.ToInt32(Request["intIdCuadroInsmoPrdcto"]);
                        tmvDEXDetalle.numCntdadExprtda = Convert.ToDecimal(Request["numCntdadExprtda"]);
                        tmvDEXDetalle.numVlorFOB = Convert.ToDecimal(Request["numVlorFOB"]);
                        tmvDEXDetalle.numPsoBrto = Convert.ToDecimal(Request["numPsoBrto"]);
                        tmvDEXDetalle.numPsoNto = Convert.ToDecimal(Request["numPsoNto"]);
                        tmvDEXDetalle.varCmntriosDEX = Request["varCmntriosDEX"];


                        if (!string.IsNullOrWhiteSpace(Request["varCdItem"]))
                        {
                            tmvDEXDetalle.varCdItem = Convert.ToInt32(Request["varCdItem"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numVlorReintrgro"]))
                        {
                            tmvDEXDetalle.numVlorReintrgro = Convert.ToDecimal(Request["numVlorReintrgro"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numCntdadCnvrtir"]))
                        {
                            tmvDEXDetalle.numCntdadCnvrtir = Convert.ToDecimal(Request["numCntdadCnvrtir"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numCntdadAdcional"]))
                        {
                            tmvDEXDetalle.numCntdadAdcional = Convert.ToDecimal(Request["numCntdadAdcional"]);
                        }

                        //Actualizar 
                        db.SaveChanges();

                        //Recuperar los datos del DEX
                        tmvDEXEncabezado tmvDEXEncabezado = db.tmvDEXEncabezado.Where(i => i.intIdDEX == tmvDEXDetalle.intIdDEX).FirstOrDefault();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización del detalle DEX",
                            string.Format("Aux: {0}", tmvDEXEncabezado.varCdAuxliar),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmvDEXEncabezado/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmvDEXDetalleController.Update", usr.varLgin);
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