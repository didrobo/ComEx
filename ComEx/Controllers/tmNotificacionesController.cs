using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class tmNotificacionesController : Controller
    {
        private List<tmNotificaciones> listTmNotificaciones
        {
            get
            {
                List<tmNotificaciones> list = new List<tmNotificaciones>();
                if (System.Web.HttpContext.Current.Cache["listTmNotificaciones"] != null)
                {
                    list = (List<tmNotificaciones>)System.Web.HttpContext.Current.Cache["listTmNotificaciones"];
                }
                else
                {
                    System.Web.HttpContext.Current.Cache.Insert("listTmNotificaciones", list, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                }

                return list;
            }
            set
            {
                if (System.Web.HttpContext.Current.Cache["listTmNotificaciones"] == null)
                {
                    System.Web.HttpContext.Current.Cache.Insert("listTmNotificaciones", value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                }
                else
                {
                    System.Web.HttpContext.Current.Cache["listTmNotificaciones"] = value;
                }
            }

        }

        // GET: tmNotificaciones
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ConsultarAlertas()
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        if (listTmNotificaciones.Count == 0)
                        {
                            listTmNotificaciones = db.spConsultarNotificaciones().ToList().Select(s => new tmNotificaciones()
                            {
                                intIdNtfccion = s.intIdNtfccion,
                                varUrl = s.varUrl,
                                varIcno = s.varIcno,
                                varNmbre = s.varNmbre,
                                varDscrpcion = s.varDscrpcion,
                                intCntdad = s.intCntdad
                            }).ToList();
                        }
                    }

                    result = Json(new { success = true, alertas = listTmNotificaciones, cantAlertas = listTmNotificaciones.Count }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmNotificacionesController.ConsultarAlertas", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public ActionResult DetalleAlerta(string id)
        {
            DataTable dtData = new DataTable();
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
                try
                {
                    SqlServer sqlServer = new SqlServer();
                    dtData = sqlServer.EjecutarConsulta("select * from " + id);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmNotificacionesController.DetalleAlerta", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return View(dtData);
        }
    }
}