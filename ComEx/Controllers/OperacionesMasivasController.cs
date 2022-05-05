using ComEx.Context;
using ComEx.Helpers;
using ComEx.Models;
using ComEx.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class OperacionesMasivasController : Controller
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;

        // GET: OperacionesMasivas
        public ActionResult BorradorDEX()
        {
            return View();
        }

        public ActionResult ImpresionDocumentos()
        {
            string strUrl = Request.Url.Segments.Last();

            ViewBag.pageOrigen = strUrl;
            return View();
        }

        public JsonResult GetListBorradores(string paramSearch)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    object listBorradores = null;
                    object listLotes = null;

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        List<vBorradoresDexCreacion> listVBorradoresDexCreacion = db.vBorradoresDexCreacion.Where(c => c.varCdAuxliar.Contains(paramSearch)).ToList();

                        var borradores = listVBorradoresDexCreacion.Select(s => new {
                            intIdDEX = s.intIdDEX,
                            varCdAuxliar = s.varCdAuxliar,
                            fecAuxliar = s.fecAuxliar,
                            intIdCmprdor = s.intIdCmprdor,
                            varNmbre = s.varNmbre
                        }).Distinct().ToList();

                        var lotes = listVBorradoresDexCreacion.Select(s => new
                        {
                            intIdDEX = s.intIdDEX,
                            varNumLte = s.varNumLte
                        }).Distinct().ToList();

                        listBorradores = borradores;
                        listLotes = lotes;
                    }

                    result = Json(new { borradores = listBorradores, lotes = listLotes }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "OperacionesMasivasController.GetListBorradores", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public JsonResult GetListDEX(string paramSearch)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    object list = null;

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        List<tmvDEXEncabezado> listDEX = db.tmvDEXEncabezado.Where(c => c.varCdAuxliar.Contains(paramSearch)).ToList();

                        var borradores = listDEX.Select(s => new {
                            intIdDEX = s.intIdDEX,
                            varCdAuxliar = s.varCdAuxliar,
                            fecAuxliar = s.fecAuxliar
                        }).Distinct().ToList();

                        list = borradores;
                    }

                    result = Json(new { dex = list }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "OperacionesMasivasController.GetListDEX", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public JsonResult GetListLotesSinDEX(string paramSearch)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    object listLotes = null;

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        List<vLotesSinDEX> listVLotesSinDEX = db.vLotesSinDEX.Where(
                            c => c.varNumLte.ToString().Contains(paramSearch)
                            ).ToList();

                        var lotes = listVLotesSinDEX.GroupBy(c => c.varNumLte).Select(group => new
                        {
                            lote = group.Key,
                            cantCmpras = group.Count()
                        }).OrderBy(or => or.lote);

                        listLotes = lotes;
                    }

                    result = Json(listLotes, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "OperacionesMasivasController.GetListLotesSinDEX", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        public JsonResult CrearBorradorDEX(Propiedades[] datosBorradorDEX)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    string cmbBorrador = datosBorradorDEX.Where(w => w.name.Equals("cmbBorrador")).Select(s => s.value).FirstOrDefault();
                    string txtNumBorrador = datosBorradorDEX.Where(w => w.name.Equals("txtNumBorrador")).Select(s => s.value).FirstOrDefault().ToUpper();
                    string cmbComprador = datosBorradorDEX.Where(w => w.name.Equals("cmbComprador")).Select(s => s.value).FirstOrDefault();
                    string[] cmbLote = datosBorradorDEX.Where(w => w.name.Equals("cmbLote")).Select(s => s.value).ToArray();
                    string mensaje = "";
                    int? intIdDex = Convert.ToInt32(cmbBorrador);
                    int? intIdCmprdor = Convert.ToInt32(cmbComprador);

                    if (cmbBorrador != null)
                    {
                        mensaje = string.Format("Se modificaron los consumos en el borrador del DEX: {0} Lotes: {1}", txtNumBorrador, String.Join(",", cmbLote));
                    }
                    else {
                        mensaje = string.Format("Se creó el borrador del DEX: {0} Lotes: {1}", txtNumBorrador, String.Join(",", cmbLote));
                    }

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.spCrearBorradorDex(intIdDex, txtNumBorrador, intIdCmprdor, String.Join(",", cmbLote));

                        System.Web.HttpContext.Current.Cache.Remove("listTmNotificaciones");

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación Borrador del DEX",
                            mensaje,
                            "Masivo",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    result = Json(new { success = true, mensaje = mensaje });

                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "OperacionesMasivasController.CrearBorradorDEX", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult SetSessionFilter(List<DTFilters> listFiltros)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    Session["listFiltros"] = listFiltros;
                    result = Json(new { success = true });

                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "OperacionesMasivasController.SetSessionFilter", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GetListDocumentosImpresion(DTParameters param)
        {
            JsonResult result = new JsonResult();
            result.MaxJsonLength = 500000000;
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<vDocumentosAdjuntosDexCIComex> listDocsDex = new List<vDocumentosAdjuntosDexCIComex>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Database.CommandTimeout = 180;
                        if (param.parametros[0] != null)
                        {
                            int dex = Convert.ToInt32(param.parametros[0]);
                            listDocsDex = db.vDocumentosAdjuntosDexCIComex.AsNoTracking().Where(x => x.intIdDEX == dex).ToList();
                        }
                        else
                        {
                            if (Session["listFiltros"] != null)
                            {
                                List<DTFilters> listFiltros = (List<DTFilters>)Session["listFiltros"];
                                List<vOrdenesCompraCI> listOrdenesCompraCI = new List<vOrdenesCompraCI>();

                                if (GlobalConfig.get().ChkFiltroPorAno)
                                {
                                    int ano = GlobalConfig.get().AppAnoFiltro;
                                    listOrdenesCompraCI = db.vOrdenesCompraCI.Where(x => x.fecCmpra.Value.Year == ano).ToList();
                                }
                                else
                                {
                                    listOrdenesCompraCI = db.vOrdenesCompraCI.ToList();
                                }

                                IQueryable<vOrdenesCompraCI> results = listOrdenesCompraCI.AsQueryable();
                                listOrdenesCompraCI = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                                if (listOrdenesCompraCI.Count > 0)
                                {
                                    List<int> listIds = listOrdenesCompraCI.Select(s => s.intIdCP.Value).ToList();
                                    
                                    if (GlobalConfig.get().ChkFiltroPorAno)
                                    {
                                        int ano = GlobalConfig.get().AppAnoFiltro;
                                        listDocsDex = db.vDocumentosAdjuntosDexCIComex.Where(
                                            x => 
                                                x.intAno == ano &&
                                                listIds.Any(id =>  id == x.intIdCP.Value)
                                        ).ToList();
                                    }
                                    else
                                    {
                                        listDocsDex = db.vDocumentosAdjuntosDexCIComex.Where(
                                            x =>
                                                listIds.Any(id => id == x.intIdCP.Value)
                                        ).ToList();
                                    }                                    

                                    //listDocsDex = listDocsDex.Where(
                                    //    x => listOrdenesCompraCI.Select(
                                    //        s => s.intIdCP
                                    //    ).Contains(x.intIdCP)
                                    //).ToList();
                                }
                            }
                        }
                    }

                    //Verifica si los archivos existen
                    ValidateIfFilesExist(listDocsDex);

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vDocumentosAdjuntosDexCIComex> data;
                    int count;

                    if (param.Columns != null)
                    {
                        data = new vDocumentosAdjuntosDexCIResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, listDocsDex, columnSearch);
                        count = new vDocumentosAdjuntosDexCIResultSet().Count(param.Search.Value, listDocsDex, columnSearch);
                    }
                    else
                    {
                        IQueryable<vDocumentosAdjuntosDexCIComex> results = listDocsDex.AsQueryable();

                        listDocsDex = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = listDocsDex;
                        count = listDocsDex.Count();
                    }
                    
                    //Ordena la información
                    data = data.OrderBy(x => x.intIdCP).ThenByDescending(t => t.varTipoDocumento).ToList();

                    DTResult<vDocumentosAdjuntosDexCIComex> resultado = new DTResult<vDocumentosAdjuntosDexCIComex>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "OperacionesMasivasController.GetListDocumentosImpresion", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        private void ValidateIfFilesExist(List<vDocumentosAdjuntosDexCIComex> data)
        {
            tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
            try
            {
                for (int x = 0; x < data.Count; x++)
                {
                    string strRutaArchivo = Server.MapPath("~" + data[x].varRtaArchvoAdjnto);

                    if (System.IO.File.Exists(strRutaArchivo)) {
                        data[x].varExiste = "SI";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "OperacionesMasivasController.ValidateIfFilesExist", usr.varLgin);
            }
        }

        public JsonResult ImprimirDocumentos(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    JsonResult datos = GetListDocumentosImpresion(param);
                    DTResult<vDocumentosAdjuntosDexCIComex> resultado = datos.Data as DTResult<vDocumentosAdjuntosDexCIComex>;

                    if (resultado != null) {
                        List<string> listArchivos = resultado.data.Where(w => w.varExiste == "SI").Select(x => Server.MapPath("~" + x.varRtaArchvoAdjnto)).ToList();

                        //Si no hay CPs Dian por imprimir sólo se imprime la primera hoja de los documentos.. ya que se imprime uno por hoja
                        bool printJustFirstPage = !resultado.data.Exists(w => w.varExiste == "SI" && w.varTipoDocumento == "CP");


                        string dir = Path.GetDirectoryName(path);
                        dir += @"\tmp\";

                        string fileToPrint = dir + usr.varLgin + string.Format("-imp-masiva-{0}{1}{2}.pdf", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                        bool success = Funciones.MergePDFs(listArchivos, fileToPrint, printJustFirstPage);

                        if (success) {
                            fileToPrint = @"\tmp\" + usr.varLgin + string.Format("-imp-masiva-{0}{1}{2}.pdf", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            result = Json(new { success = true, Message = "Se imprimieron los documentos correctamente", fileToPrint = fileToPrint });
                        }                        
                    }                    
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "OperacionesMasivasController.ImprimirDocumentos", usr.varLgin);
                    result = Json(new { success = false, Message = string.Format("Error al cargar los CPs.\n\r {0}", ex.Message + " " + ex.InnerException) });
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