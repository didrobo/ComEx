using ComEx.Context;
using ComEx.Helpers;
using ComEx.Hubs;
using ComEx.Models;
using ComEx.Models.Entities;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace ComEx.Controllers
{
    public class tmCPEncabezadoController : Controller
    {
        // GET: tmCPEncabezado
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCps(DTParameters param) {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try {
                    List<vCpCI> lisCPs = new List<vCpCI>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            lisCPs = db.vCpCI.Where(x => x.fecCP.Value.Year == ano).ToList();
                        }
                        else
                        {
                            lisCPs = db.vCpCI.AsNoTracking().ToList();
                        }                        
                    }

                    List<DTFilters> columnSearch = new List<DTFilters>();

                    if (param.ListFiltros != null) {
                        columnSearch = param.ListFiltros.ToList();
                    }

                    List<vCpCI> data;
                    DTResult<vCpCI> dtResult;
                    vCpCI totals;
                    int count;

                    if (param.Columns != null)
                    {
                        dtResult = new vCpCIResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, lisCPs, columnSearch);

                        data = dtResult.data;
                        count = dtResult.recordsTotal;
                        totals = dtResult.totals;
                    }
                    else
                    {
                        IQueryable<vCpCI> results = lisCPs.AsQueryable();

                        lisCPs = Funciones.FiltrarListDataTables(results, columnSearch).ToList();
                        data = lisCPs;
                        count = lisCPs.Count();

                        totals = new vCpCI()
                        {
                            numBrtos = lisCPs.Sum(s => s.numBrtos),
                            numFnos = lisCPs.Sum(s => s.numFnos)
                        };
                    }

                    DTResult<vCpCI> resultado = new DTResult<vCpCI>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count,
                        totals = totals
                    };

                    result = Json(resultado, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = 500000000;
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCPEncabezadoController.GetCps", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult AnularCP(int intIdCP)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try {

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.spAnularCP(intIdCP, true);

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Anulación de CP",
                            string.Format("Se anula el CP con Id: {0}", intIdCP),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }
                    
                    result = Json(new { success = true, mensaje = "Se anuló el CP correctamente" });     
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCPEncabezadoController.AnularCP", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult ModificarCP(int intIdCP)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    ObjectParameter varCdCPOrig = new ObjectParameter("varCdCPOrig", typeof(string));
                    ObjectParameter varCdCPModi = new ObjectParameter("varCdCPModi", typeof(decimal));

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        db.spCrearModificacionCP(intIdCP, varCdCPOrig, varCdCPModi);

                        //Guarda el Log
                        tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Modificación de CP",
                            string.Format("Se creó el CP de modificación {0} para el CP {1}", varCdCPModi.Value, varCdCPOrig.Value),
                            "Manual",
                            usr.varLgin);

                        db.tcLogOperaciones.Add(mytcLogOperaciones);
                        db.SaveChanges();
                    }

                    result = Json(new { success = true, mensaje = string.Format("Se creó el CP de modificación {0} para el CP {1}", varCdCPModi.Value, varCdCPOrig.Value) });
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCPEncabezadoController.ModificarCP", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult CrearXmlCP(Propiedades[] datosEncXmlCP, List<DTFilters> listFiltros)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    // Se llena el nodo Cab
                    CabType Cab = new CabType()
                    {
                        Ano = datosEncXmlCP.Where(w => w.name.Equals("intAno")).Select(s => s.value).FirstOrDefault(),
                        CodCpt = Convert.ToInt32(datosEncXmlCP.Where(w => w.name.Equals("intConcepto")).Select(s => s.value).FirstOrDefault()),
                        Formato = datosEncXmlCP.Where(w => w.name.Equals("intFormato")).Select(s => s.value).FirstOrDefault(),
                        Version = datosEncXmlCP.Where(w => w.name.Equals("intVersion")).Select(s => s.value).FirstOrDefault(),
                        NumEnvio = datosEncXmlCP.Where(w => w.name.Equals("intNumEnvio")).Select(s => s.value).FirstOrDefault(),
                        FecEnvio = Convert.ToDateTime(datosEncXmlCP.Where(w => w.name.Equals("fecFechaEnvio")).Select(s => s.value).FirstOrDefault()),
                        FecInicial = Convert.ToDateTime(datosEncXmlCP.Where(w => w.name.Equals("fecFechaInicial")).Select(s => s.value).FirstOrDefault()),
                        FecFinal = Convert.ToDateTime(datosEncXmlCP.Where(w => w.name.Equals("fecFechaFinal")).Select(s => s.value).FirstOrDefault())
                    };

                    List<vCpCI> lisCPs = new List<vCpCI>();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        //Se consultan todos los CPs en el rango de fechas del formulario de creación de XML
                        lisCPs = db.vCpCI.AsNoTracking().Where(cp => cp.fecCP >= Cab.FecInicial && cp.fecCP <= Cab.FecFinal).ToList();
                       
                        //Se la tabla fue filtrada se aplican los filtros
                        if (listFiltros != null)
                        {
                            IQueryable<vCpCI> results = lisCPs.AsQueryable();
                            lisCPs = Funciones.FiltrarListDataTables(results, listFiltros).ToList();
                        }

                        if (lisCPs.Count > 0)
                        {                            
                            List<vXmlCIDiarioEncabezado> listXmlCPEnc = new List<vXmlCIDiarioEncabezado>();
                            List<vXmlCIDiarioDetalle> listXmlCPDet = new List<vXmlCIDiarioDetalle>();

                            //Se consultan las vistas de encabezado y detalle del XML de CP en el rango de fechas del formulario de creación de XML
                            listXmlCPEnc = db.vXmlCIDiarioEncabezado.AsNoTracking().Where(cp => cp.fecCP >= Cab.FecInicial && cp.fecCP <= Cab.FecFinal).ToList();
                            listXmlCPDet = db.vXmlCIDiarioDetalle.AsNoTracking().Where(cp => cp.fecCP >= Cab.FecInicial && cp.fecCP <= Cab.FecFinal).ToList();

                            // Se toman sólo los CP que cumplen con el filtro de la tabla
                            listXmlCPEnc = listXmlCPEnc.Where(
                                xmlCPEnc => lisCPs.Count(x => xmlCPEnc.cp_Id.ToString().Contains(x.intIdCP.ToString())) > 0
                                ).ToList();

                            listXmlCPDet = listXmlCPDet.Where(
                                xmlCPDet => lisCPs.Count(x => xmlCPDet.cp_Id.ToString().Contains(x.intIdCP.ToString())) > 0
                                ).ToList();

                            // Se llenan las clases de xsd con la información
                            cp[] arrCp = listXmlCPEnc.OrderBy(o => o.cp_Id).Select(s => new cp()
                            {
                                cpto = Convert.ToInt32(s.cpto),
                                tdoc = s.tdoc,
                                ndoc = s.ndoc,
                                apl1 = s.apl1,
                                apl2 = s.apl2,
                                nom1 = s.nom1,
                                nom2 = s.nom2,
                                razsoc = s.razsoc,
                                nforant = s.nforant,
                                fecfant = s.fecfant == null ? DateTime.Now : Convert.ToDateTime(s.fecfant),
                                fecfantSpecified = s.fecfant == null ? false : true,
                                cantfac = Convert.ToInt32(s.cantfac),
                                vtcons = Convert.ToDecimal(s.vtcons),
                                vtexen = s.vtexen,
                                flimexp = Convert.ToDateTime(s.flimexp),
                                nitems = Convert.ToInt32(s.nitems),
                                cphoja2 = listXmlCPDet.Where(d => d.cp_Id == s.cp_Id).Select(sd => new cphoja2() {
                                    nfact = sd.nfact,
                                    ffac = sd.ffac == null ? DateTime.Now : Convert.ToDateTime(sd.ffac),
                                    ffacSpecified = sd.ffac == null ? false : true,
                                    resfac = sd.resfac,
                                    fres = sd.fres == null ? DateTime.Now : Convert.ToDateTime(sd.fres),
                                    fresSpecified = sd.fres == null ? false : true,
                                    tipo = Convert.ToInt32(sd.tipo),
                                    subp = sd.subp,
                                    desc = sd.desc,
                                    cunfi = Convert.ToDecimal(sd.cunfi),
                                    unfi = sd.unfi,
                                    cunco = Convert.ToDecimal(sd.cunco),
                                    unco = sd.unco,
                                    vuni = Convert.ToDecimal(sd.vuni),
                                    vtotal = Convert.ToDecimal(sd.vtotal),
                                    tiva = sd.tiva,
                                    vexen = sd.vexen,
                                    codins = Convert.ToInt32(sd.codins)
                                }).ToArray()
                            }).ToArray();

                            Cab.ValorTotal = arrCp.Sum(c => c.tdoc);
                            Cab.CantReg = arrCp.Length.ToString();

                            if (Convert.ToInt32(Cab.CantReg) <= 5000)
                            {
                                mas mas = new mas()
                                {
                                    Cab = Cab,
                                    cp = arrCp
                                };

                                XmlSerializer xsSubmit = new XmlSerializer(typeof(mas));
                                string nombreArchivo = "Dmuisca_" + Cab.CodCpt.ToString("D2") + Convert.ToInt32(Cab.Formato).ToString("D5") + Convert.ToInt32(Cab.Version).ToString("D2") + Cab.Ano + Convert.ToInt32(Cab.NumEnvio).ToString("D8") + ".xml";
                                string archivo = Server.MapPath("~/tmp/") + nombreArchivo;

                                //Si ya existe se elimina el archivo
                                if (System.IO.File.Exists(archivo))
                                {
                                    System.IO.File.Delete(archivo);
                                }

                                Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
                                using (var sww = new StreamWriter(archivo, false, encoding))
                                {
                                    xsSubmit.Serialize(sww, mas);
                                }

                                XmlDocument xDoc = new XmlDocument();
                                xDoc.Load(archivo);

                                string xml = xDoc.LastChild.OuterXml;

                                archivo = "~/tmp/" + nombreArchivo;

                                result = Json(new { urlArchivo = archivo, success = true, mensaje = string.Format("Se creó el XML de CPs correctamente") });

                                //Guarda el Log 
                                tcLogOperaciones mytcLogOperaciones = Funciones.GetLogOperacion("Creación XML CP",
                                    string.Format("Se creó el XML de CPs con fechas del {0} al {1}", Cab.FecInicial.ToString("yyyy-MM-dd"), Cab.FecFinal.ToString("yyyy-MM-dd")),
                                    "Masivo",
                                    usr.varLgin);

                                mytcLogOperaciones.xmlDatos = xml;

                                db.tcLogOperaciones.Add(mytcLogOperaciones);
                                db.SaveChanges();
                            }
                            else {
                                result = Json(new { error = "No se pueden enviar más de 5.000 CPs en un mismo archivo" });
                            }
                        }
                        else {
                            result = Json(new { error = "No se encontraron CPs con los parametros de búsqueda ingresados" });
                        }                        
                    }                    
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCPEncabezadoController.CrearXmlCP", usr.varLgin);
                    result = Json(new { error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult GenerarCPsManuales(List<DTFilters> listFiltros)
        {
            //Variables
            JsonResult result = new JsonResult();
            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
                bool ChkFiltroPorAno = GlobalConfig.get().ChkFiltroPorAno;
                int AppAnoFiltro = GlobalConfig.get().AppAnoFiltro;

                HostingEnvironment.QueueBackgroundWorkItem(c => CrearCPsManuales(usr, listFiltros, ChkFiltroPorAno, AppAnoFiltro));
                result = Json(new { success = true });
            }
            else
            {
                result = Json(new { success = false });
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        private void CrearCPsManuales(tcUsuarios objUsuario, List<DTFilters> lstFiltros, bool ChkFiltroPorAno, int AppAnoFiltro)
        {
            //Variables
            List<vPdfCpManualEncabezado> listPdfCPEnc = new List<vPdfCpManualEncabezado>();
            List<vPdfCpManualDetalle> listPdfCPDet = new List<vPdfCpManualDetalle>();
            List<vCpCI> listCP = new List<vCpCI>();
            List<tmDocumentosAdjuntos> listDocumentosAdjuntos = new List<tmDocumentosAdjuntos>();
            string strUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/");

            List<DTFilters> listFiltros = null;
            int intIdTpoDcmntoArchvosAdjntos;

            try
            {
                //Iniciar la creación del reporte
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "Mostrar", "Generanado CPs Manuales...");

                if (lstFiltros != null)
                {
                    listFiltros = lstFiltros;
                }

                using (db_comexEntities db = new db_comexEntities())
                {
                    //Si la tabla fue filtrada se aplican los filtros
                    if (listFiltros != null)
                    {
                        //Si esta activo el filtro por año
                        if (ChkFiltroPorAno)
                        {
                            listCP = db.vCpCI.Where(x => x.fecCP.Value.Year == AppAnoFiltro && String.IsNullOrEmpty(x.varNmroCPManual) && String.IsNullOrEmpty(x.varNmroCP)).ToList();
                        }
                        else
                        {
                            listCP = db.vCpCI.Where(x => String.IsNullOrEmpty(x.varNmroCPManual) && String.IsNullOrEmpty(x.varNmroCP)).ToList();
                        }

                        IQueryable<vCpCI> results = listCP.AsQueryable();
                        listCP = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                        if (listCP.Count > 0)
                        {
                            DateTime fecMin = listCP.Min(x => x.fecCP).Value;
                            DateTime fecMax = listCP.Max(x => x.fecCP).Value;

                            //Se consultan las vistas de encabezado y detalle del XML de CP en el rango de fechas de los CP filtrados
                            listPdfCPEnc = db.vPdfCpManualEncabezado.AsNoTracking().Where(cp => cp.fecCP >= fecMin && cp.fecCP <= fecMax).ToList();
                            listPdfCPDet = db.vPdfCpManualDetalle.AsNoTracking().Where(cp => cp.fecCP >= fecMin && cp.fecCP <= fecMax).ToList();

                            listPdfCPEnc = listPdfCPEnc.Where(
                                x => listCP.Select(
                                    s => s.intIdCP
                                ).Contains(x.cp_Id)
                            ).ToList();

                            listPdfCPDet = listPdfCPDet.Where(
                                x => listCP.Select(
                                    s => s.intIdCP
                                ).Contains(x.cp_Id)
                            ).ToList();
                        }
                    }
                    else
                    {
                        //Si esta activo el filtro por año
                        if (ChkFiltroPorAno)
                        {
                            listCP = db.vCpCI.Where(x => x.fecCP.Value.Year == AppAnoFiltro && String.IsNullOrEmpty(x.varNmroCPManual) && String.IsNullOrEmpty(x.varNmroCP)).ToList();
                        }
                        else
                        {
                            listCP = db.vCpCI.Where(x => String.IsNullOrEmpty(x.varNmroCPManual) && String.IsNullOrEmpty(x.varNmroCP)).ToList();
                        }

                        if (listCP.Count > 0)
                        {
                            DateTime fecMin = listCP.Min(x => x.fecCP).Value;
                            DateTime fecMax = listCP.Max(x => x.fecCP).Value;

                            //Se consultan las vistas de encabezado y detalle del XML de CP en el rango de fechas de los CP filtrados
                            listPdfCPEnc = db.vPdfCpManualEncabezado.AsNoTracking().Where(cp => cp.fecCP >= fecMin && cp.fecCP <= fecMax).ToList();
                            listPdfCPDet = db.vPdfCpManualDetalle.AsNoTracking().Where(cp => cp.fecCP >= fecMin && cp.fecCP <= fecMax).ToList();

                            listPdfCPEnc = listPdfCPEnc.Where(
                                x => listCP.Select(
                                    s => s.intIdCP
                                ).Contains(x.cp_Id)
                            ).ToList();

                            listPdfCPDet = listPdfCPDet.Where(
                                x => listCP.Select(
                                    s => s.intIdCP
                                ).Contains(x.cp_Id)
                            ).ToList();
                        }
                    }

                    if (listCP.Count > 0 && listPdfCPEnc.Count > 0)
                    {
                        //Selecciona el tipo de Documento
                        intIdTpoDcmntoArchvosAdjntos = db.tipoDocumentosArchivosAdjuntos.Where(
                            x => x.varCdTpoDcmnto == "CPM"
                            ).FirstOrDefault().intIdTpoDcmntoArchvosAdjntos;

                        //List<tmDocumentosAdjuntos> listDocumentos = new List<tmDocumentosAdjuntos>();
                        tmDocumentosAdjuntos documento;

                        //Generar Archivo uno por uno
                        foreach (vCpCI cp in listCP)
                        {
                            //Variables
                            string varNmroCPManual = string.Format("0006401{0}", cp.varCdCP);
                            string strNombreArchivo = string.Format("0006401{0}.pdf", cp.varCdCP);
                            string strRutaArchivo = Server.MapPath("~/Archivos/CPsManuales/" + cp.fecCP.Value.Year.ToString());
                            string strRuta_Archivo = string.Format(@"{0}/{1}", strRutaArchivo, strNombreArchivo);

                            //Validar si existe la ruta.
                            if (!Directory.Exists(strRutaArchivo))
                            {
                                Directory.CreateDirectory(strRutaArchivo);
                            }

                            vPdfCpManualEncabezado xmlCPEnc = listPdfCPEnc.Where(x => x.cp_Id == cp.intIdCP).FirstOrDefault();
                            vPdfCpManualDetalle xmlCPDet = listPdfCPDet.Where(x => x.cp_Id == cp.intIdCP).FirstOrDefault();

                            bool success = CrearPdfCPManual(strRuta_Archivo, cp, objUsuario, xmlCPEnc, xmlCPDet);

                            if (success)
                            {
                                //Selecciona los Adjuntos que ya existen
                                tmDocumentosAdjuntos doc = db.tmDocumentosAdjuntos.Where(x => x.varFrmlrio == "frmCompras" && x.intIdTpoDcmntoArchvosAdjntos == intIdTpoDcmntoArchvosAdjntos && x.intIdDcmnto == cp.intIdCP).FirstOrDefault();

                                //Crear el registro para asociar el archivo al CP
                                documento = new tmDocumentosAdjuntos()
                                {
                                    intIdTpoDcmntoArchvosAdjntos = intIdTpoDcmntoArchvosAdjntos,
                                    varRta = string.Format(@"{0}/{1}", "/Archivos/CPsManuales/" + cp.fecCP.Value.Year.ToString(), strNombreArchivo),
                                    varObsrvciones = string.Format("Documento creado el {0}", DateTime.Now.ToString("yyyy-MM-dd")),
                                    varFrmlrio = "frmCPs",
                                    intIdDcmnto = cp.intIdCP,
                                    intidUsuario = objUsuario.intIdUsuario
                                };

                                db.tmDocumentosAdjuntos.Remove(doc);
                                db.tmDocumentosAdjuntos.Add(documento);

                                tmCPEncabezado tmCPEncabezado = db.tmCPEncabezado.Where(x => x.intIdCP == cp.intIdCP).FirstOrDefault();

                                tmCPEncabezado.varNmroCPManual = varNmroCPManual;
                                tmCPEncabezado.fecCPManual = xmlCPEnc.fecexp;
                            }
                        }
                        db.SaveChanges();
                        //Temrinar de crear los reportes
                        PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", "CPs manuales creados correctamente.");
                    }
                    else
                    {
                        //Temrinar de crear los reportes
                        PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", "No hay CPs disponibles para generar los CP manules.<br />Revise los filtros y que los CPs seleccionados no tengan CP Dian o Manual asignados.");
                    }
                }
                                
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCPEncabezadoController.CrearCPsManuales", objUsuario.varLgin);
                //Temrinar de crear los reportes
                PeticionSignalR.EnviarServidor(strUrl, "Reportes", "OcultarConMensaje", ex.Message);
            }
        }

        private bool CrearPdfCPManual(string strRuta_Archivo, vCpCI cp, tcUsuarios objUsuario, vPdfCpManualEncabezado xmlCPEnc, vPdfCpManualDetalle xmlCPDet)
        {
            bool success = false;

            try
            {
                if (xmlCPEnc != null && xmlCPDet != null)
                {
                    Microsoft.Office.Interop.Excel.Application oExcel;
                    Workbooks oLibros;
                    Workbook oLibro;
                    Sheets oHojas;
                    Worksheet oHoja;
                    Range oCeldas;

                    string sTemplate;

                    sTemplate = string.Format(@"{0}/{1}", Server.MapPath("~/Report"), "PlantillaCPsManuales.xltx");

                    oExcel = new Microsoft.Office.Interop.Excel.Application();

                    oExcel.Visible = false;
                    oExcel.DisplayAlerts = false;

                    oLibros = oExcel.Workbooks;
                    oLibros.Open(sTemplate);
                    oLibro = oLibros.Item[1];
                    oHojas = oLibro.Worksheets;
                    oHoja = (Worksheet)oHojas.Item[1];
                    oHoja.Name = "CP_640";
                    oCeldas = oHoja.Cells;

                    LlenarDatosExcel(oCeldas, cp, xmlCPEnc, xmlCPDet);

                    oHoja.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, strRuta_Archivo);
                    oLibro.Close();

                    oLibros = null;
                    oLibro = null;
                    oHojas = null;
                    oHoja = null;
                    oCeldas = null;
                    oExcel.Quit();
                    oExcel = null;
                    GC.Collect();

                    success = true;
                }
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "tmCPEncabezadoController.CrearPdfCPManual", objUsuario.varLgin);
                success = false;
            }

            return success;
        }

        private void LlenarDatosExcel(Range oCeldas, vCpCI cp, vPdfCpManualEncabezado xmlCPEnc, vPdfCpManualDetalle xmlCPDet)
        {

            oCeldas[5, 4] = cp.fecCP.Value.Year; //1. Año
            oCeldas[5, 11] = xmlCPEnc.cpto; //2. Concepto

            oCeldas[7, 35] = cp.varCdCP; //4. Número de Formulario

            oCeldas[15, 2] = xmlCPEnc.scitdoc; //20. Tipo docum.
            oCeldas[15, 4] = xmlCPEnc.scindoc; //18. Número de identificación.
            oCeldas[15, 14] = xmlCPEnc.scidv; //6. DV
            oCeldas[15, 16] = xmlCPEnc.scirazsoc; //20. Razón Social

            oCeldas[18, 2] = xmlCPEnc.tdoc; //24. Tipo docum.
            oCeldas[18, 4] = xmlCPEnc.ndoc; //25. Número de identificación.
            oCeldas[18, 14] = xmlCPEnc.dv; //6. DV
            oCeldas[18, 16] = xmlCPEnc.apl1; //27. Primer apellido
            oCeldas[18, 23] = xmlCPEnc.apl2; //28. Segundo apellido
            oCeldas[18, 30] = xmlCPEnc.nom1; //29. Primer nombre
            oCeldas[18, 35] = xmlCPEnc.nom2; //30. Otros nombres
            oCeldas[20, 2] = xmlCPEnc.razsoc; //31. Razón Social

            oCeldas[23, 2] = xmlCPEnc.nforant; //32. No. Formulario Anterior
            oCeldas[23, 13] = xmlCPEnc.fecfant; //33. Fecha Formulario Anterior

            oCeldas[26, 2] = (int)xmlCPEnc.cantfac; //34. Cantidad de facturas
            oCeldas[26, 12] = (decimal)xmlCPEnc.vtcons; //35. Valor total consolidado   
            oCeldas[26, 20] = (decimal)xmlCPEnc.vtexen; //36. Valor total exención IVA

            oCeldas[28, 36] = (DateTime)xmlCPEnc.flimexp; //38. Fecha límite de export. m/cías.
            oCeldas[30, 2] = (int)xmlCPEnc.nitems; //39. No. de Ítem

            oCeldas[32, 3] = xmlCPDet.nfact; //40. No. Factura compra
            oCeldas[32, 9] = (DateTime)xmlCPDet.ffac; //41. Fecha factura 
            oCeldas[32, 15] = xmlCPDet.resfac; //42. Resolución facturación 
            oCeldas[32, 21] = xmlCPDet.fres; //43. Fecha resolución 
            oCeldas[32, 27] = xmlCPDet.desctipo; //44. Tipo de producto o servicio 
            oCeldas[32, 34] = (int)xmlCPDet.tipo; //Cod.
            oCeldas[32, 36] = xmlCPDet.subp; //45. Subpartida arancelaria

            oCeldas[34, 3] = xmlCPDet.desc; //46. Descripción de la mercancía o SIP

            oCeldas[36, 3] = xmlCPDet.cunfi; //47. Cantidad Unid. fisicas 
            oCeldas[36, 10] = xmlCPDet.descunfi; //48. Unidad física 
            oCeldas[36, 21] = xmlCPDet.unfi; //Cod.
            oCeldas[36, 23] = xmlCPDet.cunco; //49. Cantidad unidades comerciales 
            oCeldas[36, 31] = xmlCPDet.descunco; //50. Unidad comercial 
            oCeldas[36, 40] = xmlCPDet.unco; //Cod.

            oCeldas[38, 3] = xmlCPDet.vuni; //51. Valor unitario 
            oCeldas[38, 10] = xmlCPDet.vtotal; //52. Valor total 
            oCeldas[38, 17] = xmlCPDet.tiva; //53. Tarifa IVA (Exenta) 
            oCeldas[38, 23] = xmlCPDet.vexen; //54. Valor exención IVA
            oCeldas[38, 31] = xmlCPDet.codins; //55. Código insumo

            oCeldas[49, 8] = xmlCPEnc.varNmbreRprsntnteLgal; //1001. Apellidos y Nombres
            oCeldas[50, 6] = xmlCPEnc.rltdoc; //1002. Tipo doc.
            oCeldas[50, 14] = xmlCPEnc.rlndoc; //1003. No. Identificación
            oCeldas[50, 25] = xmlCPEnc.rldv; //1004. DV
            oCeldas[51, 8] = xmlCPEnc.codrep; //1005. Cód. Representación
            oCeldas[52, 8] = xmlCPEnc.codorg + "    " + xmlCPEnc.scirazsoc; //1006. Organización

            oCeldas[53, 33] = (DateTime)xmlCPEnc.fecexp; //997. Fecha expedición
            oCeldas[53, 38] = (DateTime)xmlCPEnc.fecexp; //997. Fecha expedición


        }
    }
}