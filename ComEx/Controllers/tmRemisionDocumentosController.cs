using ComEx.Context;
using ComEx.Helpers;
using ComEx.Models;
using ComEx.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace ComEx.Controllers
{
    public class tmRemisionDocumentosController : Controller
    {
        // GET: tmRemisionDocumentos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FormatoSobres()
        {
            return View();
        }

        public ActionResult CargueXmlRemisiones()
        {
            return View();
        }

        public JsonResult GetRemisionDocs(DTParameters param)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    List<spTmRemisionDocumentos_Result> listRemisionDocumentos = new List<spTmRemisionDocumentos_Result>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        DateTime desde = Convert.ToDateTime(param.parametros[0]);
                        DateTime hasta = Convert.ToDateTime(param.parametros[1]);

                        Session["RemisionDocs_desde"] = desde;
                        Session["RemisionDocs_hasta"] = hasta;

                        listRemisionDocumentos = db.spTmRemisionDocumentos(desde, hasta).ToList();
                    }

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null)
                    {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<spTmRemisionDocumentos_Result> data;
                    int count;

                    if (param.Columns != null)
                    {
                        data = new spTmRemisionDocumentosResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, listRemisionDocumentos, columnSearch);
                        count = new spTmRemisionDocumentosResultSet().Count(param.Search.Value, listRemisionDocumentos, columnSearch);
                    }
                    else
                    {
                        IQueryable<spTmRemisionDocumentos_Result> results = listRemisionDocumentos.AsQueryable();

                        listRemisionDocumentos = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = listRemisionDocumentos;
                        count = listRemisionDocumentos.Count();
                    }

                    DTResult<spTmRemisionDocumentos_Result> resultado = new DTResult<spTmRemisionDocumentos_Result>
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
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRemisionDocumentosController.GetRemisionDocs", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult ActualizarDatosEnvio(List<DTFilters> listFiltros, DateTime desde, DateTime hasta, string varNumGuiaEnvio, DateTime fecEnvio)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    if (listFiltros != null)
                    {
                        using (db_comexEntities db = new db_comexEntities())
                        {
                            db.Database.CommandTimeout = 180;
                            List<spTmRemisionDocumentos_Result> listRemisionDocumentos = new List<spTmRemisionDocumentos_Result>();
                            listRemisionDocumentos = db.spTmRemisionDocumentos(desde, hasta).ToList();

                            IQueryable<spTmRemisionDocumentos_Result> results = listRemisionDocumentos.AsQueryable();
                            listRemisionDocumentos = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                            if (listRemisionDocumentos.Count > 0)
                            {
                                int[] idsRemisiones = listRemisionDocumentos.Select(s => s.intIdRmsion).ToArray();

                                List<tmRemisionDocumentos> listRemisiones = db.tmRemisionDocumentos.Where(x => idsRemisiones.Contains(x.intIdRmsion)).ToList();

                                listRemisiones.ForEach(remision => { remision.varNumGuiaEnvio = varNumGuiaEnvio; remision.fecEnvio = fecEnvio; });
                                db.SaveChanges();

                                //Guarda el Log
                                tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Actualizar datos de envío",
                                    string.Format("Se asignó la guía de envío {0} de {1}", varNumGuiaEnvio, fecEnvio.ToString("yyyy/MM/dd")),
                                    "Masivo",
                                    usr.varLgin);

                                db.tcLogOperaciones.Add(mytcLogOperaciones);
                                db.SaveChanges();

                                result = Json(new { success = true, mensaje = string.Format("Se asignó la guía de envío {0} de {1}", varNumGuiaEnvio, fecEnvio.ToString("yyyy/MM/dd")) });
                            }
                            else
                            {
                                result = Json(new { success = false, error = "No se encontró ningún registro" });
                            }
                        }
                    }
                    else
                    {
                        result = Json(new { success = false, error = "Debe aplicar algún filtro sobre el reporte" });
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRemisionDocumentosController.ActualizarDatosEnvio", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult CrearXml(List<DTFilters> listFiltros, DateTime desde, DateTime hasta)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.Database.CommandTimeout = 180;
                        List<spTmRemisionDocumentos_Result> listRemisionDocumentos = new List<spTmRemisionDocumentos_Result>();
                        listRemisionDocumentos = db.spTmRemisionDocumentos(desde, hasta).ToList();

                        if (listFiltros != null)
                        {
                            IQueryable<spTmRemisionDocumentos_Result> results = listRemisionDocumentos.AsQueryable();
                            listRemisionDocumentos = Funciones.FiltrarListDataTables(results, listFiltros).ToList();
                        }

                        if (listRemisionDocumentos.Count > 0)
                        {
                            listRemisionDocumentos = listRemisionDocumentos.Select(s => new spTmRemisionDocumentos_Result() {
                                intIdRmsion = s.intIdRmsion,
                                varCdPrvdor = s.varCdPrvdor,
                                intMes = s.intMes,
                                fecEnvio = s.fecEnvio,
                                varNumGuiaEnvio = s.varNumGuiaEnvio,
                                varDscrpcionCiudad = s.varDscrpcionCiudad
                            }).ToList();

                            string nombreArchivo = "Remisiones_" + desde.ToString("yyyyMMdd") + "_" + hasta.ToString("yyyyMMdd") + ".xml";
                            string archivo = Server.MapPath("~/tmp/") + nombreArchivo;

                            //Si ya existe se elimina el archivo
                            if (System.IO.File.Exists(archivo))
                            {
                                System.IO.File.Delete(archivo);
                            }

                            string xml = Funciones.SerializarObjetoToXml(listRemisionDocumentos, typeof(List<spTmRemisionDocumentos_Result>));

                            XmlDocument xDoc = new XmlDocument();
                            xDoc.LoadXml(xml);
                            xDoc.Save(archivo);

                            archivo = "~/tmp/" + nombreArchivo;

                            result = Json(new { urlArchivo = archivo, success = true, mensaje = string.Format("Se creó el XML de Remisiones correctamente") });

                            //Guarda el Log 
                            tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación XML Remisiones",
                                string.Format("Se creó el XML de Remisiones con fechas del {0} al {1}", desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd")),
                                "Masivo",
                                usr.varLgin);

                            mytcLogOperaciones.xmlDatos = xml;

                            db.tcLogOperaciones.Add(mytcLogOperaciones);
                            db.SaveChanges();
                        }
                        else
                        {
                            result = Json(new { success = false, error = "No se encontró ningún registro" });
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRemisionDocumentosController.CrearXml", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        [HttpPost]
        public ActionResult ProcesarXmlRemisiones()
        {
            //Variables
            object objJson = null;
            #region Validar Session
            if (Session["UsuarioLogueado"] == null)
            {
                RedirectToAction("LogOut", "Home");
            }
            //Datos de la sessión del Usuario
            tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

            #endregion

            try
            {
                #region Validaciones
                //Validar que se selecciono por lo menos un archvio
                if (Request.Files.Count <= 0)
                {
                    objJson = new { success = false, Message = "Asegúrese de seleccionar un archivo XML." };
                    return Json(objJson, JsonRequestBehavior.AllowGet);
                }
                #endregion

                //Archivo
                HttpPostedFileBase Archivo = Request.Files[0];
                //Obtener la extensión del archivo
                string strExtensionArchivo = Path.GetExtension(Archivo.FileName);
                //Validar si la extensión es permitida
                if (!strExtensionArchivo.Equals(".xml"))
                {
                    objJson = new { success = false, Message = "Solo se permiten extensiones XML" };
                    return Json(objJson, JsonRequestBehavior.AllowGet);
                }

                //Subir archivo a la carpeta tmp
                string strRutaArchivo = Server.MapPath("~/tmp/") + Archivo.FileName;
                Archivo.SaveAs(strRutaArchivo);

                //Crear objeto a partir del XML
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(strRutaArchivo);

                List<spTmRemisionDocumentos_Result> listRemisionDocumentos = (List<spTmRemisionDocumentos_Result>) Funciones.DeSerializarObjetoXml(xDoc.OuterXml, typeof(List<spTmRemisionDocumentos_Result>));

                using (db_comexEntities db = new db_comexEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;

                    int[] listIds = listRemisionDocumentos.Select(s => s.intIdRmsion).Distinct().ToArray();
                    List<tmRemisionDocumentos> listRemisiones = db.tmRemisionDocumentos.Where(x => listIds.Contains(x.intIdRmsion)).Include("tmProveedores").AsNoTracking().ToList();

                    listRemisionDocumentos.Join(listRemisiones, (listXml) => listXml.intIdRmsion, (listDB) => listDB.intIdRmsion,
                        (listXml, listDB) =>
                        {
                            listDB.fecRcbdo = listXml.fecRcbdo;
                            listDB.fecEntrgaCliente = listXml.fecEntrgaCliente;
                            listDB.varQuienRcbe = listXml.varQuienRcbe;
                            listDB.fecDspcho = listXml.fecDspcho;
                            listDB.varNumGuiaDspacho = listXml.varNumGuiaDspacho;

                            return listDB;
                        }
                        ).ToList();
                    
                    //Guarda el Log 
                    tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Cargue XML Remisiones",
                        "Se carga XML de remisiones",
                        "Masivo",
                        usr.varLgin);

                    mytcLogOperaciones.xmlDatos = xDoc.OuterXml;

                    db.tcLogOperaciones.Add(mytcLogOperaciones);
                    db.SaveChanges();

                    listRemisiones.ForEach(x => {
                        if (x.tmProveedores != null)
                        {
                            x.tmProveedores.tmRemisionDocumentos = null;
                            x.tmProveedores.tmCompras = null;
                            x.tmProveedores.tmCPEncabezado = null;
                            x.tmProveedores.tmPaises = null;
                            x.tmProveedores.tmRegimenes = null;
                            x.tmProveedores.tmResolucionesProveedor = null;
                            x.tmProveedores.tmTituloMineroProveedor = null;
                            x.tmProveedores.tmTituloMineroProveedor1 = null;
                        }
                        else
                        {
                            x.tmProveedores = new tmProveedores();
                        }

                        if (x.tmProveedores.tmCiudades != null)
                        {
                            x.tmProveedores.tmCiudades.tmProveedores = null;
                            x.tmProveedores.tmCiudades.tmCompras = null;
                            x.tmProveedores.tmCiudades.tmDepartamentos = null;
                            x.tmProveedores.tmCiudades.tmImportadorExportador = null;
                            x.tmProveedores.tmCiudades.tmTerceros = null;
                            x.tmProveedores.tmCiudades.tmTituloMineroProveedor = null;
                        }
                        else
                        {
                            x.tmProveedores.tmCiudades = new tmCiudades();
                        }
                    });

                    //Eliminar archivo
                    System.IO.File.Delete(strRutaArchivo);

                    objJson = new
                    {
                        success = true,
                        Message = "Se cargó el XML de remisiones correctamente",
                        data = listRemisiones
                    };
                    return Json(objJson, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmRemisionDocumentosController.ProcesarXmlRemisiones", usr.varLgin);
                objJson = new { success = false, Message = string.Format("Error al cargar las remisiones.\n\r {0}", ex.Message + " " + ex.InnerException) };
            }

            return Json(objJson, JsonRequestBehavior.AllowGet);
        }
    }
}