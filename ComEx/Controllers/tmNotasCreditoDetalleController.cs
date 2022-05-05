using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class tmNotasCreditoDetalleController : Controller
    {
        // GET: tmNotasCreditoDetalle
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CrearNotaCredito(tmNotasCreditoEncabezado tmNotasCreditoEncabezado)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    int intIdCP = 0;
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.tmNotasCreditoEncabezado.Add(tmNotasCreditoEncabezado);

                        int intIdCmpraDtlle = tmNotasCreditoEncabezado.tmNotasCreditoDetalle.FirstOrDefault().intIdCmpraDtlle;

                        tmCPDetalle tmCPDetalle = db.tmComprasDetalle.Where(
                            dc => dc.intIdCmpraDtlle == intIdCmpraDtlle
                        ).Select(x => x.tmCPDetalle).FirstOrDefault();
                       
                        tmCPDetalle.numCntdadCp -= tmNotasCreditoEncabezado.tmNotasCreditoDetalle.FirstOrDefault().numCntdad;
                        tmCPDetalle.numVlorCpPorCI -= tmNotasCreditoEncabezado.tmNotasCreditoDetalle.FirstOrDefault().numSbttal;

                        intIdCP = tmCPDetalle.intIdCp;

                        db.SaveChanges();
                        
                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación de devolución",
                            string.Format("Se creó la devolución {0}", tmNotasCreditoEncabezado.varNtaCrdto),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    result = Json(new { success = true, intIdCP = intIdCP, mensaje = string.Format("Se creó la devolución {0} correctamente", tmNotasCreditoEncabezado.varNtaCrdto) });
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmNotasCreditoDetalleController.CrearNotaCredito", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
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