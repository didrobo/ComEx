using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class CpPdfController : Controller
    {
        private db_comexEntities db = new db_comexEntities();
        string path = AppDomain.CurrentDomain.BaseDirectory;
        string dir = "";

        // GET: CpPdf
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CPs()
        {

            dir = Path.GetDirectoryName(path);
            dir += @"\Archivos\CPs\Entrada\";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir); // inside the if statement
            }

            ViewBag.listPlanCI = Funciones.GetListOfSelectListItem(db.tmPlanesCI.Where(x => x.bitBloqueo != true).ToList(), "varDscrpcionPlanCI", "intIdPlanCI");

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            List<FileInfo> files = dirInfo.GetFiles("*.pdf").Where(x => x.Name.StartsWith("640") && x.Name.Replace(".pdf", "").Length == 13).ToList();

            return View(files);
        }

        [HttpPost]
        public ActionResult ProcesarArchivos()
        {
            //Variables
            object objJson = null;
            //Variables OUT del procedimiento
            ObjectParameter intErrores = new ObjectParameter("Errores", typeof(int));
            ObjectParameter intBuenos = new ObjectParameter("Buenos", typeof(int));
            ObjectParameter intTotalRegistros = new ObjectParameter("TotalDocumentos", typeof(int));

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
                //Recuperar el plan CI
                int intIdPlanCI = Convert.ToInt32(Request["intIdPlanCI"]);
                string varDscrpcionPlanCI = Request["varDscrpcionPlanCI"].ToString();

                //Se declaran las listas de las tablas para almacenar la información de los PDF
                List<tmpTmvCpEncabezadoInterface> listEnc = new List<tmpTmvCpEncabezadoInterface>();
                List<tmpTmvCpDetalleInterface> listDet = new List<tmpTmvCpDetalleInterface>();

                // Se truncan las tablas temporales
                db.Database.ExecuteSqlCommand("DELETE FROM temporal.tmpTmvCpEncabezadoInterface");

                //Se crea un ciclo para recorrer todos los archivos en la ruta
                dir = Path.GetDirectoryName(path);
                dir += @"\Archivos\CPs\";

                foreach (string file in Directory.EnumerateFiles(dir + @"Entrada\", "*.pdf"))
                {
                    //Variables
                    string txt = "";
                    tmpTmvCpEncabezadoInterface tblEnc = new tmpTmvCpEncabezadoInterface();
                    tmpTmvCpDetalleInterface tblDet = new tmpTmvCpDetalleInterface();
                    FileInfo archivo = new FileInfo(file);

                    if (archivo.Name.StartsWith("640") && archivo.Name.Replace(".pdf", "").Length == 13)
                    {
                        //Se lee el archivo PDF para obtener un string con la información
                        PDFParser pdfParser = new PDFParser();
                        pdfParser.ExtractText(file, "", ref txt);

                        //Se crea un Array de Strings que en cada casilla contiene la información del PDF por hoja
                        string[] arrayHojas = txt.Split(new string[] { "&newpage;" }, StringSplitOptions.RemoveEmptyEntries);

                        if (arrayHojas.Length == 2)
                        {

                            // Se separan las casillas de la hoja 1
                            string[] datosHoja = arrayHojas[0].Split('^');

                            tblEnc.C4_numeroFormulario = archivo.Name.Replace(".pdf", "");                            
                            tblEnc.nombreArchivo = archivo.Name;
                            tblEnc.intIdIntrfceErrores = "0";
                            tblEnc.intIdPlanCI = intIdPlanCI;

                            ProcesarHoja1(datosHoja, ref tblEnc);
                            tblEnc.archivoAdjunto = Url.Content("~/Archivos/CPs/Salida/" + tblEnc.C1_ano + @"/" + archivo.Name);

                            listEnc.Add(tblEnc);

                            // Se separan las casillas de la hoja 2
                            datosHoja = arrayHojas[1].Split('^');

                            tblDet.C4_DetalleNumeroFormulario = tblEnc.C4_numeroFormulario;

                            ProcesarHoja2(datosHoja, ref tblDet);
                            listDet.Add(tblDet);
                        }
                    }
                }

                // Se guarda la información en las tablas temporales
                db.tmpTmvCpEncabezadoInterface.AddRange(listEnc);
                db.tmpTmvCpDetalleInterface.AddRange(listDet);
                db.SaveChanges();

                //Se ejecuta el procedimiento almacenado spValidacionesCpPdfInterface
                db.spValidacionesCpPdfInterface(intErrores, intBuenos, intTotalRegistros);

                //Devolver mensaje
                objJson = new
                {
                    success = true,
                    Message = "PDFs de CPs cargados correctamente.",
                    PlanCI = varDscrpcionPlanCI,
                    intErrores = intErrores.Value,
                    intBuenos = intBuenos.Value,
                    intTotalRegistros = intTotalRegistros.Value,
                };
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "CpPdfController.ProcesarArchivos", usr.varLgin);
                objJson = new { success = false, Message = string.Format("Error al cargar los CPs.\n\r {0}", ex.Message + " " + ex.InnerException) };
            }

            return Json(objJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTempCPs()
        {
            //Variables
            object objJson = null;

            #region Validar Session
            if (Session["UsuarioLogueado"] == null)
            {
                RedirectToAction("LogOut", "Home");
                return null;
            }
            //Datos de la sessión del Usuario
            tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
            #endregion

            try
            {
                //Recuperar los datos con el procedimiento spCpPdfInterface
                List<spCpPdfInterface_Result> lstCpPdfInterface = db.spCpPdfInterface().ToList();

                //Se mueven los archivos de carpetas
                MoverArchivos(lstCpPdfInterface);

                return Json(new { data = lstCpPdfInterface }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "CpPdfController.GetTempCPs", usr.varLgin);
                objJson = new { success = false, Message = ex.Message + " " + ex.InnerException };
            }

            return Json(objJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubirCPs()
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    //Ejecuta el procedimiento de creación de ordenes de compra
                    db.spInsertarTrasladoCPPdfInterface(usr.varLgin);

                    result = Json(new { success = true, Message = "Se subieron los CPs correctamente" });
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "CpPdfController.SubirCPs", usr.varLgin);
                    result = Json(new { success = false, Message = string.Format("Error al cargar los CPs.\n\r {0}", ex.Message + " " + ex.InnerException) });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        private void MoverArchivos(List<spCpPdfInterface_Result> lstCpPdfInterface) {
            
            // Carpeta de entrada
            string dirEntrada = Path.GetDirectoryName(path);
            dirEntrada += @"\Archivos\CPs\Entrada\";
            
            // Carpeta errores
            string dirErrores = Path.GetDirectoryName(path);
            dirErrores += @"\Archivos\CPs\ConError\";

            if (!Directory.Exists(dirErrores))
            {
                Directory.CreateDirectory(dirErrores); // inside the if statement
            }

            foreach (spCpPdfInterface_Result cp in lstCpPdfInterface) {

                try
                {
                    if (cp.intIdIntrfceErrores == "0")
                    {
                        System.IO.File.Move(dirEntrada + cp.C4_numeroFormulario + ".pdf", dirSalida(cp.C997_fechaAceptacion.Value.Year.ToString()) + cp.C4_numeroFormulario + ".pdf");
                    }
                    else
                    {
                        System.IO.File.Move(dirEntrada + cp.C4_numeroFormulario + ".pdf", dirErrores + cp.C4_numeroFormulario + ".pdf");
                    }
                }
                catch (Exception ex) { }
            }

        }

        private string dirSalida(string ano)
        {
            // Carpeta de salida
            string dirSalida = Path.GetDirectoryName(path);
            dirSalida += @"\Archivos\CPs\Salida\" + ano + @"\";

            if (!Directory.Exists(dirSalida))
            {
                Directory.CreateDirectory(dirSalida); // inside the if statement
            }

            return dirSalida;
        }

        private void ProcesarHoja1(string[] datosHoja, ref tmpTmvCpEncabezadoInterface tblEnc)
        {
            tblEnc.C1_ano = datosHoja[0].Split('|')[0].Trim();
            tblEnc.C2_concepto = datosHoja[1].Split('|')[0].Trim();

            tblEnc.C6_SciDigitoVerificacion = datosHoja[2].Split('|')[0].Trim();
            tblEnc.C11_SciRazonSocial = datosHoja[3].Split('|')[0].Trim();
            tblEnc.C18_SciNit = datosHoja[4].Split('|')[0].Trim();
            tblEnc.C20_SciTipoDocumento = datosHoja[5].Split('|')[0].Trim();

            tblEnc.C24_proveeTipoDocumento = datosHoja[6].Split('|')[0].Trim();
            tblEnc.C25_proveeNit = datosHoja[7].Split('|')[0].Trim();
            tblEnc.C26_proveeDigitoVerificacion = datosHoja[8].Split('|')[0].Trim();
            tblEnc.C27_proveePrimerApellido = datosHoja[9].Split('|')[0].Trim();
            tblEnc.C28_proveeSegundoApellido = datosHoja[10].Split('|')[0].Trim();
            tblEnc.C29_proveePrimerNombre = datosHoja[11].Split('|')[0].Trim();
            tblEnc.C30_proveeOtrosNombres = datosHoja[12].Split('|')[0].Trim();
            tblEnc.C31_proveeRazonSocial = datosHoja[13].Split('|')[0].Trim();

            tblEnc.C32_numFormularioAnterior = datosHoja[14].Split('|')[0].Trim();
            tblEnc.C33_fecFormularioAnterior = datosHoja[15].Split('|')[0].Trim();

            tblEnc.C34_CantidadDeFacturas = datosHoja[16].Split('|')[0].Trim();
            tblEnc.C35_ValorTtalCnslddo = datosHoja[17].Split('|')[0].Trim();
            tblEnc.C36_ValorTtalExcncionIva = datosHoja[18].Split('|')[0].Trim();

            tblEnc.C37_EfectosCP = datosHoja[19].Split('|')[0].Trim();
            tblEnc.C38_FecLmteExprtcion = datosHoja[20].Split('|')[0].Trim();
            tblEnc.C39_NoDeitems = datosHoja[21].Split('|')[0].Trim();

            tblEnc.C997_fechaAceptacion = datosHoja[22].Split('|')[0].Trim();
            tblEnc.C1001_apellidosNombresQuienSuscribeDocumento = datosHoja[23].Split('|')[0].Trim();
            tblEnc.C1002_tipoDocumentoQuienSuscribeDocumento = datosHoja[24].Split('|')[0].Trim();
            tblEnc.C1003_numeroIdentificacionQuienSuscribeDocumento = datosHoja[25].Split('|')[0].Trim();
            tblEnc.C1004_digitoVerificacionQuienSuscribeDocumento = datosHoja[26].Split('|')[0].Trim();
            tblEnc.C1005_codigoRepresentacionQuienSuscribeDocumento = datosHoja[27].Split('|')[0].Trim();
            tblEnc.C1006_organizacionQuienSuscribeDocumento = datosHoja[28].Split('|')[0].Trim();
        }

        private void ProcesarHoja2(string[] datosHoja, ref tmpTmvCpDetalleInterface tblDet)
        {
            tblDet.C40_NoFactura = datosHoja[2].Split('|')[0].Trim();
            tblDet.C41_FecFactura = datosHoja[8].Split('|')[0].Trim();
            tblDet.C42_NoResolFactura = datosHoja[14].Split('|')[0].Trim();
            tblDet.C43_FecFactura = datosHoja[20].Split('|')[0].Trim();

            tblDet.C44_TipoProdServicio = datosHoja[26].Split('|')[0].Trim();
            tblDet.C44a_CodTipoProdServicio = datosHoja[27].Split('|')[0].Trim();

            tblDet.C45_Subpartida = datosHoja[33].Split('|')[0].Trim();
            tblDet.C46_DescripcionMercancia = datosHoja[39].Split('|')[0].Trim();
            tblDet.C47_CantUnidadFisica = datosHoja[45].Split('|')[0].Trim();

            tblDet.C48_UnidadFisica = datosHoja[51].Split('|')[0].Trim();
            tblDet.C48a_CodUnidadFisica = datosHoja[52].Split('|')[0].Trim();

            tblDet.C49_CantUnidadCmrcial = datosHoja[58].Split('|')[0].Trim();

            tblDet.C50_UnidadComercial = datosHoja[64].Split('|')[0].Trim();
            tblDet.C50a_CodUnidadComercial = datosHoja[65].Split('|')[0].Trim();

            tblDet.C51_ValorUnitario = datosHoja[71].Split('|')[0].Trim();
            tblDet.C52_ValorTotal = datosHoja[77].Split('|')[0].Trim();
            tblDet.C53_TarifaIva = datosHoja[83].Split('|')[0].Trim();
            tblDet.C54_ValorExcencionIva = datosHoja[89].Split('|')[0].Trim();
            tblDet.C55_CodInsumo = datosHoja[95].Split('|')[0].Trim();
        }
    }
}