using ComEx.Context;
using ComEx.Helpers;
using ComEx.Models;
using ComEx.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class tmProveedorController : Controller
    {
        // GET: tmProveedor
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProveedorByNameOrNit(string paramSearch)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vProveedores> listVProveedores = new List<vProveedores>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        listVProveedores = db.vProveedores.Where(p => p.varNmbre.Contains(paramSearch) || p.varNitPrvdor.Contains(paramSearch)).ToList();
                    }

                    result = Json(listVProveedores, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmProveedorController.GetProveedorByNameOrNit", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public JsonResult GetProveedores(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vProveedores> lisProveedores = new List<vProveedores>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        lisProveedores = db.vProveedores.AsNoTracking().ToList();
                    }

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vProveedores> data;
                    int count;

                    if (param.Columns != null)
                    {
                        data = new vProveedoresResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, lisProveedores, columnSearch);
                        count = new vProveedoresResultSet().Count(param.Search.Value, lisProveedores, columnSearch);
                    }
                    else
                    {
                        IQueryable<vProveedores> results = lisProveedores.AsQueryable();

                        lisProveedores = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = lisProveedores;
                        count = lisProveedores.Count();
                    }

                    DTResult<vProveedores> resultado = new DTResult<vProveedores>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = 500000000;
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmProveedorController.GetProveedores", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        // GET: tmProveedor/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        ViewBag.listPais = Funciones.GetListOfSelectListItem(db.tmPaises.ToList(), "varDscrpcionPais", "intIdpais");
                        ViewBag.listCiudad = Funciones.GetListOfSelectListItem(db.tmCiudades.ToList(), "varDscrpcionCiudad", "intIdCiudad");
                        ViewBag.listRegimen = Funciones.GetListOfSelectListItem(db.tmRegimenes.ToList(), "varNbreRgmen", "intIdRgmen");
                        ViewBag.listExportador = Funciones.GetListOfSelectListItem(db.tmImportadorExportador.ToList(), "varNmbre", "intIdImprtdorExprtdor"); 
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmProveedorController.Create", usr.varLgin);
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return View();
        }

        public JsonResult SetTmProveedor()
        {

            object ObjJson = new object();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    tmProveedores tmProveedores = new tmProveedores();

                    tmProveedores.varCdPrvdor = Request["varCdPrvdor"];
                    tmProveedores.varNitPrvdor = Request["varNitPrvdor"];
                    tmProveedores.varNmbre = Request["varNmbre"];
                    tmProveedores.varNmbre1 = Request["varNmbre1"];
                    tmProveedores.varNmbre2 = Request["varNmbre2"];
                    tmProveedores.varAplldos = Request["varAplldos"];
                    tmProveedores.varAplldos1 = Request["varAplldos1"];
                    tmProveedores.bitAcgdosLey1429 = Convert.ToBoolean(Request["bitAcgdosLey1429"]);
                    tmProveedores.bitBlqueado = Convert.ToBoolean(Request["bitBlqueado"]);
                    tmProveedores.varDrccion = Request["varDrccion"];
                    tmProveedores.varTlfno = Request["varTlfno"];
                    tmProveedores.varEmail = Request["varEmail"];
                    tmProveedores.varFairminedId = Request["varFairminedId"];

                    if (!string.IsNullOrWhiteSpace(Request["intIdRgmen"]))
                    {
                        tmProveedores.intIdRgmen = Convert.ToInt32(Request["intIdRgmen"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdPais"]))
                    {
                        tmProveedores.intIdPais = Convert.ToInt32(Request["intIdPais"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdCmpnia"]))
                    {
                        tmProveedores.intIdCmpnia = Convert.ToInt32(Request["intIdCmpnia"]);
                    }

                    if (!string.IsNullOrWhiteSpace(Request["intIdCiudad"]))
                    {
                        tmProveedores.intIdCiudad = Convert.ToInt32(Request["intIdCiudad"]);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Guardar Proveedor
                        db.tmProveedores.Add(tmProveedores);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación proveedores",
                            string.Format("NIT: {0} Nombre: {1}", tmProveedores.varNitPrvdor, String.IsNullOrEmpty(tmProveedores.varNmbre) ? tmProveedores.varNmbre1 : tmProveedores.varNmbre),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos guardados correctamente.", Success = true, Url = "~/tmProveedor/Index/" };
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmProveedorController.SetTmProveedor", usr.varLgin);
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
                        ObjJson = new { Message = "Asegúrese de seleccionar un proveedor", Success = false };
                        return Json(ObjJson, JsonRequestBehavior.AllowGet);
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        tmProveedores tmProveedores = db.tmProveedores.Where(i => i.intIdPrvdor == id).FirstOrDefault();
                        //Eliminar Plan
                        db.tmProveedores.Remove(tmProveedores);
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Se elimina proveedor",
                            string.Format("NIT: {0} Nombre: {1}", tmProveedores.varNitPrvdor, String.IsNullOrEmpty(tmProveedores.varNmbre) ? tmProveedores.varNmbre1 : tmProveedores.varNmbre),
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
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmProveedorController.Delete", usr.varLgin);
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
                        tmProveedores tmProveedores = db.tmProveedores.Where(i => i.intIdPrvdor == id).FirstOrDefault();

                        ViewBag.listPais = Funciones.GetListOfSelectListItem(db.tmPaises.ToList(), "varDscrpcionPais", "intIdpais");
                        ViewBag.listCiudad = Funciones.GetListOfSelectListItem(db.tmCiudades.ToList(), "varDscrpcionCiudad", "intIdCiudad");
                        ViewBag.listRegimen = Funciones.GetListOfSelectListItem(db.tmRegimenes.ToList(), "varNbreRgmen", "intIdRgmen");
                        ViewBag.listExportador = Funciones.GetListOfSelectListItem(db.tmImportadorExportador.ToList(), "varNmbre", "intIdImprtdorExprtdor");

                        return View(tmProveedores);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmProveedorController.Edit", usr.varLgin);
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
                        int intIdPrvdor = Convert.ToInt32(Request["intIdPrvdor"]);
                        tmProveedores tmProveedores = db.tmProveedores.Where(i => i.intIdPrvdor == intIdPrvdor).FirstOrDefault();

                        tmProveedores.varCdPrvdor = Request["varCdPrvdor"];
                        tmProveedores.varNitPrvdor = Request["varNitPrvdor"];
                        tmProveedores.varNmbre = Request["varNmbre"].ToString().ToUpper();
                        tmProveedores.varNmbre1 = Request["varNmbre1"].ToString().ToUpper();
                        tmProveedores.varNmbre2 = Request["varNmbre2"].ToString().ToUpper();
                        tmProveedores.varAplldos = Request["varAplldos"].ToString().ToUpper();
                        tmProveedores.varAplldos1 = Request["varAplldos1"].ToString().ToUpper();
                        tmProveedores.bitAcgdosLey1429 = Convert.ToBoolean(Request["bitAcgdosLey1429"]);
                        tmProveedores.bitBlqueado = Convert.ToBoolean(Request["bitBlqueado"]);
                        tmProveedores.varDrccion = Request["varDrccion"];
                        tmProveedores.varTlfno = Request["varTlfno"];
                        tmProveedores.varEmail = Request["varEmail"];
                        tmProveedores.varFairminedId = Request["varFairminedId"];

                        if (!string.IsNullOrWhiteSpace(Request["intIdRgmen"]))
                        {
                            tmProveedores.intIdRgmen = Convert.ToInt32(Request["intIdRgmen"]);
                        }
                        else {
                            tmProveedores.intIdRgmen = null;
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdPais"]))
                        {
                            tmProveedores.intIdPais = Convert.ToInt32(Request["intIdPais"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdCmpnia"]))
                        {
                            tmProveedores.intIdCmpnia = Convert.ToInt32(Request["intIdCmpnia"]);
                        }

                        if (!string.IsNullOrWhiteSpace(Request["intIdCiudad"]))
                        {
                            tmProveedores.intIdCiudad = Convert.ToInt32(Request["intIdCiudad"]);
                        }

                        //Actualizar Compras
                        db.SaveChanges();

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualización de proveedor",
                            string.Format("NIT: {0} Nombre: {1}", tmProveedores.varNitPrvdor, String.IsNullOrEmpty(tmProveedores.varNmbre) ? tmProveedores.varNmbre1 : tmProveedores.varNmbre),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    ObjJson = new { Message = "Datos Actualizados correctamente.", Success = true, Url = "~/tmProveedor/Index" };

                    //return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmProveedorController.Update", usr.varLgin);
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