using ComEx.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ComEx.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult FlotCharts()
        {
            return View("FlotCharts");
        }

        public ActionResult MorrisCharts()
        {
            return View("MorrisCharts");
        }

        public ActionResult Tables()
        {
            return View("Tables");
        }

        public ActionResult Forms()
        {
            return View("Forms");
        }

        public ActionResult Panels()
        {
            return View("Panels");
        }

        public ActionResult Buttons()
        {
            return View("Buttons");
        }

        public ActionResult Notifications()
        {
            return View("Notifications");
        }

        public ActionResult Typography()
        {
            return View("Typography");
        }

        public ActionResult Icons()
        {
            return View("Icons");
        }

        public ActionResult Grid()
        {
            return View("Grid");
        }

        public ActionResult Blank()
        {
            return View("Blank");
        }

        public ActionResult Login()
        {
            GlobalConfig.get().AppAnoFiltro = DateTime.Now.Year;
            GlobalConfig.get().ChkFiltroPorAno = true;

            if (Session["UsuarioLogueado"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tcUsuarios usuario) 
        {
            if (ModelState.IsValid) {
                using (db_comexEntities db = new db_comexEntities()) {
                    tcUsuarios usr = db.tcUsuarios.Where(u => u.varLgin.Equals(usuario.varLgin) && u.varPssword.Equals(usuario.varPssword)).FirstOrDefault();

                    if (usr != null)
                    {
                        Session["UsuarioLogueado"] = usr;
                        FormsAuthentication.SetAuthCookie(usr.varLgin, false);

                        return RedirectToAction("Index");
                    }
                    else {
                        ViewBag.Message = "Usuario y Password No Válidos";
                    }
                }
            }
            return View(usuario);
        }

        public ActionResult LogOut()
        {
            Session["UsuarioLogueado"] = null; //it's my session variable
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut(); //you write this when you use FormsAuthentication
            return RedirectToAction("Login", "Home");
        }

        public void SetAppAnoFiltro(int intAno) {
            try
            {
                GlobalConfig.get().AppAnoFiltro = intAno;
            }
            catch (Exception) { }
        }

        public void SetChkFiltroPorAno(bool filtrarAno)
        {
            try
            {
                GlobalConfig.get().ChkFiltroPorAno = filtrarAno;
            }
            catch (Exception) { }
        }
    }
}