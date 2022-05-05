using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class NavbarController : Controller
    {
        // GET: Navbar
        [Authorize]
        public ActionResult Index()
        {
            List<tcMenu> litMenu = new List<tcMenu>();
            if (Session["UsuarioLogueado"] != null)
            {
                if (HttpContext.Cache["litMenu"] == null)
                {
                    tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                    try
                    {
                        using (db_comexEntities db = new db_comexEntities())
                        {
                            IQueryable<tcMenu> varmenu =
                                from menu in db.tcMenu
                                join rolMenu in db.tcRolMenu on menu.intIdMnu equals rolMenu.intIdMnu
                                join rolUsuario in db.tcRolUsuario on rolMenu.intIdRol equals rolUsuario.intIdRol
                                where rolUsuario.intIdUsuario == usr.intIdUsuario
                                select menu;

                            varmenu = varmenu.Distinct();
                            litMenu = varmenu.ToList();
                            HttpContext.Cache["litMenu"] = varmenu.ToList();
                        }
                    }
                    catch (Exception ex) {
                        Log.GetInstancia().EscribirLog(ex.Message, "Error", "NavbarController.Index", usr.varLgin);
                    }
                }
                else {
                    litMenu = (List<tcMenu>)HttpContext.Cache["litMenu"];
                }

                return PartialView("_Navbar", litMenu);
            }
            else {
                RedirectToAction("LogOut", "Home");
                return PartialView("_Navbar", litMenu);
            }           
        }
    }
}