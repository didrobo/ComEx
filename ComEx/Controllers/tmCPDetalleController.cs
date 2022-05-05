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
    public class tmCPDetalleController : Controller
    {
        // GET: tmCPDetalle
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCPsDetalle(DTParameters param, int id)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmCPDetalle> lisCPsDetalle = new List<tmCPDetalle>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.LazyLoadingEnabled = false;
                        lisCPsDetalle = db.tmCPDetalle.Where(cd => cd.intIdCp == id).ToList();
                    }

                    int count = lisCPsDetalle.Count;

                    DTResult<tmCPDetalle> resultado = new DTResult<tmCPDetalle>
                    {
                        draw = param.Draw,
                        data = lisCPsDetalle,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCPDetalleController.GetCPsDetalle", usr.varLgin);
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