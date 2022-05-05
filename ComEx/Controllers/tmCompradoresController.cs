using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class tmCompradoresController : Controller
    {
        // GET: tmCompradores
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCompradorByNameOrNit(string paramSearch)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<tmCompradores> listCompradores = new List<tmCompradores>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        listCompradores = db.tmCompradores.AsNoTracking().Where(p => p.varNmbre.Contains(paramSearch) || p.varCdCmprdor.Contains(paramSearch)).ToList();
                    }

                    result = Json(listCompradores, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCompradoresController.GetCompradorByNameOrNit", usr.varLgin);
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