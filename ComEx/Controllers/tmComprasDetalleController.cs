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
    public class tmComprasDetalleController : Controller
    {
        // GET: tmComprasDetalle
        public ActionResult Index()
        {
            return View();
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
                        var lstRegalia = (from a in db.tmRegalias
                                          join b in db.tmLineas
                                          on a.intIdLnea equals b.intIdLnea
                                          orderby a.intAno descending, a.intMes descending
                                          select new
                                          {
                                              intIdRglia = a.intIdRglia,
                                              varCdLnea = b.varCdLnea,
                                              intAno = a.intAno,
                                              intMes = a.intMes
                                          }
                                          ).AsEnumerable().Select(x => new SelectListItem
                                          {
                                              Text = string.Format("{0}-{1}/{2}", x.varCdLnea.ToString(), x.intAno.ToString(), x.intMes.ToString().PadLeft(2, '0')),
                                              Value = x.intIdRglia.ToString()
                                          }).ToList();

                        lstRegalia.Insert(0, new SelectListItem() { Selected = true, Text = "Seleccionar...", Value = "" });

                        ViewBag.lstInsumo = Funciones.GetListOfSelectListItem(db.tmInsumos.ToList(), "varDscrpcionInsmo", "intIdInsmo");
                        ViewBag.lstAdquirida = Funciones.GetListOfSelectListItem(db.tmAdquiridasComprasTipo.ToList(), "varDscrpcionAdquirdaTpoCmpra", "intIdAdquirdaTpoCmpra");
                        ViewBag.listRegalia = lstRegalia;

                        //Enviar id
                        ViewBag.Id = id.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasDetalleController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult GetComprasDetalle(DTParameters param, int id) {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try {
                    List<vOrdenesCompraDetalleCI> lisComprasDetalle = new List<vOrdenesCompraDetalleCI>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        lisComprasDetalle = db.vOrdenesCompraDetalleCI.Where(cd => cd.intIdCmpra == id).ToList();
                    }

                    int count = lisComprasDetalle.Count;

                    DTResult<vOrdenesCompraDetalleCI> resultado = new DTResult<vOrdenesCompraDetalleCI>
                    {
                        draw = param.Draw,
                        data = lisComprasDetalle,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasDetalleController.GetComprasDetalle", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public JsonResult GetComprasDetalleCP(DTParameters param, int id)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vOrdenesCompraDetalleCI> lisComprasDetalle = new List<vOrdenesCompraDetalleCI>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        lisComprasDetalle = db.vOrdenesCompraDetalleCI.Where(cd => cd.intIdCPDtlle == id).ToList();
                    }

                    int count = lisComprasDetalle.Count;

                    DTResult<vOrdenesCompraDetalleCI> resultado = new DTResult<vOrdenesCompraDetalleCI>
                    {
                        draw = param.Draw,
                        data = lisComprasDetalle,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasDetalleController.GetComprasDetalleCP", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public JsonResult SetTmComprasDetalle()
        {
            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmComprasDetalle tmCompraDetalle = new tmComprasDetalle();
                    tmCompraDetalle.intIdCmpra = Convert.ToInt32(Request["intIdCmpra"]);
                    tmCompraDetalle.intIdInsmo = Convert.ToInt32(Request["intIdInsmo"]);
                    tmCompraDetalle.numCntdadFctra = Convert.ToDecimal(Request["numCntdadFctra"]);
                    tmCompraDetalle.numVlorUntrio = Convert.ToDecimal(Request["numVlorUntrio"]);
                    tmCompraDetalle.numCntdadCP = Convert.ToDecimal(Request["numCntdadCP"]);
                    tmCompraDetalle.numSbttal = Convert.ToDecimal(Request["numSbttal"]);
                    tmCompraDetalle.numFnos = Convert.ToDecimal(Request["numFnos"]);
                    tmCompraDetalle.numLey = Convert.ToDecimal(Request["numLey"]);
                    tmCompraDetalle.varCmntrios = Request["varCmntrios"];

                    if (!string.IsNullOrWhiteSpace(Request["intCdItem"]))
                    {
                        tmCompraDetalle.intCdItem = Convert.ToInt32(Request["intCdItem"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numPrcntjeIva"]))
                    {
                        tmCompraDetalle.numPrcntjeIva = Convert.ToDecimal(Request["numPrcntjeIva"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdAdqurdasTtlo"]))
                    {
                        tmCompraDetalle.intIdAdqurdasTtlo = Convert.ToInt32(Request["intIdAdqurdasTtlo"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numRglias"]))
                    {
                        tmCompraDetalle.numRglias = Convert.ToDecimal(Request["numRglias"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdRglia"]))
                    {
                        tmCompraDetalle.intIdRglia = Convert.ToInt32(Request["intIdRglia"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["numPorcMnria"]))
                    {
                        tmCompraDetalle.numPorcMnria = Convert.ToDecimal(Request["numPorcMnria"]);
                    }
                    
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar ComprasDetalle
                        db.tmComprasDetalle.Add(tmCompraDetalle);
                        db.SaveChanges();

                        //Recuperar los datos del orden de compra
                        tmCompras tmCompra = db.tmCompras.Where(i => i.intIdCmpra == tmCompraDetalle.intIdCmpra).FirstOrDefault();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación del detalle orden de compra",
                            string.Format("Compra: {0} Aux: {1} Fac: {2} Cargue: {3} Lote: {4}", tmCompraDetalle.intIdCmpra, tmCompra.varCdAuxliar, tmCompra.varNmroFctra, tmCompra.intCnsctvoCrgue, tmCompra.varNumLte),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmCompras/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasDetalleController.SetTmComprasDetalle", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar el detalle de la compra", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmComprasDetalle tmCompraDetalle = db.tmComprasDetalle.Where(i => i.intIdCmpraDtlle == id).FirstOrDefault();
                        tmCompras tmCompra = db.tmCompras.Where(i => i.intIdCmpra == tmCompraDetalle.intIdCmpra).FirstOrDefault();
                        //Eliminar Compra
                        db.tmComprasDetalle.Remove(tmCompraDetalle);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina el detalle de orden de compra",
                            string.Format("Compra: {0} Aux: {1} Fac: {2} Cargue: {3} Lote: {4}", tmCompra.intIdCmpra, tmCompra.varCdAuxliar, tmCompra.varNmroFctra, tmCompra.intCnsctvoCrgue, tmCompra.varNumLte),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmCompras/Index/" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasDetalleController.Delete", usr.varLgin);
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
                        tmComprasDetalle tmCompraDetalle = db.tmComprasDetalle.Where(i => i.intIdCmpraDtlle == id).FirstOrDefault();

                        var lstRegalia = (from a in db.tmRegalias
                                          join b in db.tmLineas
                                          on a.intIdLnea equals b.intIdLnea
                                          orderby a.intAno descending, a.intMes descending
                                          select new
                                          {
                                              intIdRglia = a.intIdRglia,
                                              varCdLnea = b.varCdLnea,
                                              intAno = a.intAno,
                                              intMes = a.intMes
                                          }
                                          ).AsEnumerable().Select(x => new SelectListItem
                                          {
                                              Text = string.Format("{0}-{1}/{2}", x.varCdLnea.ToString(), x.intAno.ToString(), x.intMes.ToString().PadLeft(2, '0')),
                                              Value = x.intIdRglia.ToString()
                                          }).ToList();

                        lstRegalia.Insert(0, new SelectListItem() { Selected = true, Text = "Seleccionar...", Value = "" });

                        ViewBag.lstInsumo = Funciones.GetListOfSelectListItem(db.tmInsumos.ToList(), "varDscrpcionInsmo", "intIdInsmo");
                        ViewBag.lstAdquirida = Funciones.GetListOfSelectListItem(db.tmAdquiridasComprasTipo.ToList(), "varDscrpcionAdquirdaTpoCmpra", "intIdAdquirdaTpoCmpra");
                        ViewBag.listRegalia = lstRegalia;

                        return View(tmCompraDetalle);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasDetalleController.Edit", usr.varLgin);
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
                        int intIdCmpraDtlle = Convert.ToInt32(Request["intIdCmpraDtlle"]);
                        tmComprasDetalle tmCompraDetalle = db.tmComprasDetalle.Where(i => i.intIdCmpraDtlle == intIdCmpraDtlle).FirstOrDefault();

                        tmCompraDetalle.intIdCmpra = Convert.ToInt32(Request["intIdCmpra"]);
                        tmCompraDetalle.intIdInsmo = Convert.ToInt32(Request["intIdInsmo"]);
                        tmCompraDetalle.numCntdadFctra = Convert.ToDecimal(Request["numCntdadFctra"]);
                        tmCompraDetalle.numVlorUntrio = Convert.ToDecimal(Request["numVlorUntrio"]);
                        tmCompraDetalle.numCntdadCP = Convert.ToDecimal(Request["numCntdadCP"]);
                        tmCompraDetalle.numSbttal = Convert.ToDecimal(Request["numSbttal"]);
                        tmCompraDetalle.numFnos = Convert.ToDecimal(Request["numFnos"]);
                        tmCompraDetalle.numLey = Convert.ToDecimal(Request["numLey"]);
                        tmCompraDetalle.varCmntrios = Request["varCmntrios"];

                        if (!string.IsNullOrWhiteSpace(Request["intCdItem"]))
                        {
                            tmCompraDetalle.intCdItem = Convert.ToInt32(Request["intCdItem"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numPrcntjeIva"]))
                        {
                            tmCompraDetalle.numPrcntjeIva = Convert.ToDecimal(Request["numPrcntjeIva"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdAdqurdasTtlo"]))
                        {
                            tmCompraDetalle.intIdAdqurdasTtlo = Convert.ToInt32(Request["intIdAdqurdasTtlo"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numRglias"]))
                        {
                            tmCompraDetalle.numRglias = Convert.ToDecimal(Request["numRglias"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdRglia"]))
                        {
                            tmCompraDetalle.intIdRglia = Convert.ToInt32(Request["intIdRglia"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["numPorcMnria"]))
                        {
                            tmCompraDetalle.numPorcMnria = Convert.ToDecimal(Request["numPorcMnria"]);
                        }
                        //Actualizar ComprasDetalle
                        db.SaveChanges();

                        //Recuperar los datos del orden de compra
                        tmCompras tmCompra = db.tmCompras.Where(i => i.intIdCmpra == tmCompraDetalle.intIdCmpra).FirstOrDefault();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización del detalle orden de compra",
                            string.Format("Compra: {0} Aux: {1} Fac: {2} Cargue: {3} Lote: {4}", tmCompraDetalle.intIdCmpra, tmCompra.varCdAuxliar, tmCompra.varNmroFctra, tmCompra.intCnsctvoCrgue, tmCompra.varNumLte),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmCompras/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmComprasDetalleController.Update", usr.varLgin);
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