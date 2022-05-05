using ComEx.Context;
using ComEx.Helpers;
using FastMember;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class DexPdfController : Controller
    {
        private db_comexEntities db = new db_comexEntities();
        string path = AppDomain.CurrentDomain.BaseDirectory;
        string dir = "";

        // GET: DexPdf
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DEX()
        {

            dir = Path.GetDirectoryName(path);
            dir += @"\Archivos\DEX\Entrada\";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir); // inside the if statement
            }

            ViewBag.listPlanCI = Funciones.GetListOfSelectListItem(db.tmPlanesCI.Where(x => x.bitBloqueo != true).ToList(), "varDscrpcionPlanCI", "intIdPlanCI");

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            List<FileInfo> files = dirInfo.GetFiles("*.pdf").ToList();

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

                int intTotalRegistros = 0;

                //Se declaran las listas de las tablas para almacenar la información de los DEX
                List<tmpTmvDEXEncabezadoInterface> listEnc = new List<tmpTmvDEXEncabezadoInterface>();
                List<tmptmvDEXDetalleInterface> listDet = new List<tmptmvDEXDetalleInterface>();
                List<tmpTmvDEXEncabezadoTransportadorInterface> listTrans = new List<tmpTmvDEXEncabezadoTransportadorInterface>();
                List<tmpTmvDEXEncabezadoSoporteInterface> listSopor = new List<tmpTmvDEXEncabezadoSoporteInterface>();
                List<tmptmvDEXDetalleContinuacionInterface> listDetCont = new List<tmptmvDEXDetalleContinuacionInterface>();
                List<tmptmvDEXDetalleCpsInterface> listCps = new List<tmptmvDEXDetalleCpsInterface>();

                // Se truncan las tablas temporales
                db.Database.ExecuteSqlCommand("DELETE FROM temporal.tmpTmvDEXEncabezadoInterface");

                //Se crea un ciclo para recorrer todos los archivos en la ruta
                dir = Path.GetDirectoryName(path);
                dir += @"\Archivos\DEX\";

                foreach (string file in Directory.EnumerateFiles(dir + @"Entrada\", "*.pdf"))
                {

                    //Variables
                    string txt = "";
                    intTotalRegistros++;
                    tmpTmvDEXEncabezadoInterface tblEnc = new tmpTmvDEXEncabezadoInterface(); //Hojas 1
                    tmptmvDEXDetalleInterface tblDet = null; //Hojas 2                                        
                    tmptmvDEXDetalleContinuacionInterface tblDetCont = new tmptmvDEXDetalleContinuacionInterface(); //Hojas 8
                    
                    FileInfo archivo = new FileInfo(file);

                    //Se lee el archivo PDF para obtener un string con la información
                    PDFParser pdfParser = new PDFParser();
                    pdfParser.ExtractText(file, "", ref txt);

                    //Se crea un Array de Strings que en cada casilla contiene la información del PDF por hoja
                    string[] arrayHojas = txt.Split(new string[] { "&newpage;" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int x = 0; x < arrayHojas.Length; x++)
                    {

                        // Se separan las casillas
                        string[] datosHoja = arrayHojas[x].Split('^');

                        // Hoja 1
                        if (x == 0)
                        {                            
                            tblEnc.nombreArchivo = archivo.Name;
                            tblEnc.intIdIntrfceErrores = 0;
                            tblEnc.intIdTpoDex = 1;

                            ProcesarHoja1(datosHoja, ref tblEnc);
                            tblEnc.archivoAdjunto = Url.Content("~/Archivos/DEX/Salida/" + tblEnc.C1_ano + @"/" + archivo.Name);

                            //Valida que sea un DEX
                            if (tblEnc.C4_numeroFormulario.StartsWith("600") && !(listEnc.Exists(dex => dex.C4_numeroFormulario == tblEnc.C4_numeroFormulario)))
                            {
                                listEnc.Add(tblEnc);
                            }
                            else
                            {
                                //Se detiene el ciclo
                                x = arrayHojas.Length;

                                // Carpeta de entrada
                                string dirEntrada = Path.GetDirectoryName(path);
                                dirEntrada += @"\Archivos\Dex\Entrada\";

                                // Carpeta errores
                                string dirErrores = Path.GetDirectoryName(path);
                                dirErrores += @"\Archivos\Dex\ConError\";

                                if (!Directory.Exists(dirErrores))
                                {
                                    Directory.CreateDirectory(dirErrores); // inside the if statement
                                }

                                if (System.IO.File.Exists(dirErrores + tblEnc.nombreArchivo))
                                {
                                    System.IO.File.Delete(dirErrores + tblEnc.nombreArchivo);
                                }
                                System.IO.File.Move(dirEntrada + tblEnc.nombreArchivo, dirErrores + tblEnc.nombreArchivo);
                            }
                        }
                        // Hoja 2
                        else if (x > 0 && tblEnc.C79_totalSeries != null && tblEnc.C79_totalSeries != "" & x <= Convert.ToInt32(tblEnc.C79_totalSeries))
                        {
                            tblDet = new tmptmvDEXDetalleInterface();
                            ProcesarHoja2(datosHoja, ref tblDet);
                            listDet.Add(tblDet);
                        }
                        else
                        {
                            // Hoja 3
                            if (datosHoja[11].Split('|')[0].Trim() == "31")
                            {
                                ProcesarHoja3(datosHoja, ref listTrans);
                            }
                            // Hoja 4
                            else if (datosHoja.Length > 15)
                            {
                                ProcesarHoja4(datosHoja, ref listSopor);
                            }
                            // Hoja 8
                            else if (datosHoja.Length == 15)
                            {
                                tblDetCont = new tmptmvDEXDetalleContinuacionInterface();
                                ProcesarHoja8(datosHoja, ref tblDetCont);
                                listDetCont.Add(tblDetCont);

                                //Se concatena la descripción de las mercancías en el ítem del DEX
                                listDet.Where(det => det.C4_detalleNumeroFormulario == tblDetCont.C4_detalleContinuacionNumeroFormulario && det.C98_numeroSerie == tblDetCont.C172_numeroSerie
                                    ).FirstOrDefault().C112_descripcionSubpartida += tblDetCont.C112_descripcionSubpartida;
                            }
                        }
                    }
                }

                //Se van a extraer los CP que vienen el DEX

                var cps = listDet.Select(x => new {
                    dex = x.C4_detalleNumeroFormulario,
                    arrayCPs = x.C112_descripcionSubpartida.Replace(".", ",").Replace(";", ",").Replace(":", ",").Split(',').Where(
                        n => n.Contains("000640")
                        ).Select(
                            s => s.Trim().Substring(s.Trim().IndexOf("000640") + 3, 13)
                        ).ToArray()
                }).ToList();

                if (cps != null) {

                    cps.ForEach(d => listCps.AddRange(d.arrayCPs.Select(c => new tmptmvDEXDetalleCpsInterface
                        {
                            C4_detalleNumeroFormulario = d.dex,
                            Cp = c
                        }).ToList<tmptmvDEXDetalleCpsInterface>())
                    );
                   
                }

                // Se guarda la información en las tablas temporales
                db.tmpTmvDEXEncabezadoInterface.AddRange(listEnc);
                db.tmptmvDEXDetalleInterface.AddRange(listDet);
                db.tmpTmvDEXEncabezadoTransportadorInterface.AddRange(listTrans);
                db.tmpTmvDEXEncabezadoSoporteInterface.AddRange(listSopor);
                db.tmptmvDEXDetalleContinuacionInterface.AddRange(listDetCont);
                //db.tmptmvDEXDetalleCpsInterface.AddRange(listCps);
                db.SaveChanges();

                //Para llenar los CPs, se hace un BulkCopy ya que el método anterior es muy lento
                var connectionString = ConfigurationManager.ConnectionStrings["db_comexConnectionString"].ConnectionString;
                var bulkCopy = new SqlBulkCopy(connectionString);
                bulkCopy.DestinationTableName = "temporal.tmptmvDEXDetalleCpsInterface";

                DataTable table = new DataTable("temporal.tmptmvDEXDetalleCpsInterface");
                using (var reader = ObjectReader.Create(listCps))
                {
                    table.Load(reader);
                }
                table.Columns.Remove("tmpTmvDEXEncabezadoInterface");
                bulkCopy.WriteToServer(table);

                //Se ejecuta el procedimiento almacenado spValidacionesDexPdfInterface
                db.spValidacionesDexPdfInterface(intBuenos, intErrores);

                //Devolver mensaje
                objJson = new
                {
                    success = true,
                    Message = "PDFs de DEX cargados correctamente.",
                    PlanCI = varDscrpcionPlanCI,
                    intErrores = intErrores.Value == DBNull.Value ? 0 : intErrores.Value,
                    intBuenos = intBuenos.Value == DBNull.Value ? 0 : intBuenos.Value,
                    intTotalRegistros,
                };

            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "DexPdfController.ProcesarArchivos", usr.varLgin);
                objJson = new { success = false, Message = string.Format("Error al cargar los DEX.\n\r {0}", ex.Message + " " + ex.InnerException) };
            }

            return Json(objJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTempDEX()
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
                //Recuperar los datos con el procedimiento spCompararTmvDEXEncabezadoVsPdf
                List<spCompararTmvDEXEncabezadoVsPdf_Result> lstDexPdfInterface = db.spCompararTmvDEXEncabezadoVsPdf().ToList();

                //Se mueven los archivos de carpetas
                MoverArchivos(lstDexPdfInterface);

                return Json(new { data = lstDexPdfInterface }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "DexPdfController.GetTempDEX", usr.varLgin);
                objJson = new { success = false, Message = ex.Message + " " + ex.InnerException };
            }

            return Json(objJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubirDEX()
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    //Ejecuta el procedimiento de creación de ordenes de compra
                    db.spCompletaTmvDEXEncabezadoVsPdf(usr.varLgin);

                    result = Json(new { success = true, Message = "Se subieron los DEX correctamente" });
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "DexPdfController.SubirDEX", usr.varLgin);
                    result = Json(new { success = false, Message = string.Format("Error al cargar los DEX.\n\r {0}", ex.Message + " " + ex.InnerException) });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        private void MoverArchivos(List<spCompararTmvDEXEncabezadoVsPdf_Result> lstDexPdfInterface)
        {

            // Carpeta de entrada
            string dirEntrada = Path.GetDirectoryName(path);
            dirEntrada += @"\Archivos\Dex\Entrada\";

            // Carpeta errores
            string dirErrores = Path.GetDirectoryName(path);
            dirErrores += @"\Archivos\Dex\ConError\";

            if (!Directory.Exists(dirErrores))
            {
                Directory.CreateDirectory(dirErrores); // inside the if statement
            }

            foreach (spCompararTmvDEXEncabezadoVsPdf_Result dex in lstDexPdfInterface)
            {

                try
                {
                    if (dex.intIdIntrfceErrores == 0 || dex.intIdIntrfceErrores == 49 || dex.intIdIntrfceErrores == 50 || dex.intIdIntrfceErrores == 53 || dex.intIdIntrfceErrores == 326)
                    {
                        if (System.IO.File.Exists(dirSalida(dex.C997_fechaAceptacion.Value.Year.ToString()) + dex.nombreArchivo)) {
                            System.IO.File.Delete(dirSalida(dex.C997_fechaAceptacion.Value.Year.ToString()) + dex.nombreArchivo);
                        }
                        System.IO.File.Move(dirEntrada + dex.nombreArchivo, dirSalida(dex.C997_fechaAceptacion.Value.Year.ToString()) + dex.nombreArchivo);
                    }
                    else
                    {
                        if (System.IO.File.Exists(dirErrores + dex.nombreArchivo)) {
                            System.IO.File.Delete(dirErrores + dex.nombreArchivo);
                        }
                        System.IO.File.Move(dirEntrada + dex.nombreArchivo, dirErrores + dex.nombreArchivo);
                    }
                }
                catch (Exception ex) { }
            }

        }

        private string dirSalida(string ano)
        {
            // Carpeta de salida
            string dirSalida = Path.GetDirectoryName(path);
            dirSalida += @"\Archivos\Dex\Salida\" + ano + @"\";

            if (!Directory.Exists(dirSalida))
            {
                Directory.CreateDirectory(dirSalida); // inside the if statement
            }

            return dirSalida;
        }

        private void ProcesarHoja1(string[] datosHoja, ref tmpTmvDEXEncabezadoInterface tblEnc)
        {
            //Datos del Archivo
            tblEnc.C1_ano = datosHoja[0].Split('|')[0].Trim();
            tblEnc.C4_numeroFormulario = datosHoja[1].Split('|')[0].Trim();

            //Exportador
            tblEnc.C6_expoDigitoVerificacion = datosHoja[2].Split('|')[0].Trim();
            tblEnc.C7_expoPrimerApellido = datosHoja[3].Split('|')[0].Trim();
            tblEnc.C8_expoSegundoApellido = datosHoja[4].Split('|')[0].Trim();
            tblEnc.C9_expoPrimer_nombre = datosHoja[5].Split('|')[0].Trim();
            tblEnc.C10_expoOtrosNombres = datosHoja[6].Split('|')[0].Trim();
            tblEnc.C11_expoRazonSocial = datosHoja[7].Split('|')[0].Trim();
            tblEnc.C18_expoNit = datosHoja[8].Split('|')[0].Trim();
            tblEnc.C20_expoTipoDocumento = datosHoja[9].Split('|')[0].Trim();

            //Declarante
            tblEnc.C24_declaTipoDocumento = datosHoja[10].Split('|')[0].Trim();
            tblEnc.C25_declaNit = datosHoja[11].Split('|')[0].Trim();
            tblEnc.C26_declaDigitoVerificacion = datosHoja[12].Split('|')[0].Trim();
            tblEnc.C27_declaPrimerApellido = datosHoja[13].Split('|')[0].Trim();
            tblEnc.C28_declaSegundoApellido = datosHoja[14].Split('|')[0].Trim();
            tblEnc.C29_declaPrimerNombre = datosHoja[15].Split('|')[0].Trim();
            tblEnc.C30_declaOtrosNombres = datosHoja[16].Split('|')[0].Trim();
            tblEnc.C31_declaRazonSocial = datosHoja[17].Split('|')[0].Trim();

            //Destinatario
            tblEnc.C32_clienteTipoDocumento = datosHoja[18].Split('|')[0].Trim();
            tblEnc.C33_nit = datosHoja[19].Split('|')[0].Trim();
            tblEnc.C34_primerApellido = datosHoja[20].Split('|')[0].Trim();
            tblEnc.C35_segundoApellido = datosHoja[21].Split('|')[0].Trim();
            tblEnc.C36_primerNombre = datosHoja[22].Split('|')[0].Trim();
            tblEnc.C37_otrosNombres = datosHoja[23].Split('|')[0].Trim();
            tblEnc.C38_clienteRazonSocial = datosHoja[24].Split('|')[0].Trim();
            tblEnc.C39_clienteDirreccion = datosHoja[25].Split('|')[0].Trim();
            tblEnc.C40_clienteCiudad = datosHoja[26].Split('|')[0].Trim();

            //Datos del negocio

            int c = 27;

            tblEnc.C41_claseDex = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C41_claseDex != "")
            {
                tblEnc.C41a_claseDexCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C42_formularioAnterior = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblEnc.C43_tipoDiligenciamiento = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C43_tipoDiligenciamiento != "")
            {
                tblEnc.C43a_tipoDiligenciamientoCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C44_tipoDespacho = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C44_tipoDespacho != "")
            {
                tblEnc.C44a_tipoDespachoCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C45_tipoCorrecion = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C45_tipoCorrecion != "")
            {
                tblEnc.C45a_tipoCorrecionCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C46_numeroReferencia = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C47_numeroProgramaEspecialMuestras = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C48_numeroAutorizacionEmbarque = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblEnc.C49_regimenAduanero = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C49_regimenAduanero != "")
            {
                tblEnc.C49a_regimenAduaneroCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C50_aduanaDespacho = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C50_aduanaDespacho != "")
            {
                tblEnc.C51a_aduanaDespacho = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C51_codigoPaisTramite = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C52_codigoRegionProcedencia = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblEnc.C53_tipoDatos = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C53_tipoDatos != "")
            {
                tblEnc.C53a_tiposDatosCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C54_tipoEmbaque = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C54_tipoEmbaque != "")
            {
                tblEnc.C54_tipoEmbarqueCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C55_codigoNaturalezaTransaccion = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C56_codigoIncoterms = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C57_lugarEntrega = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C58_codigoMonedaTransaccion = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C59_valorfacturaMonedaTransaccion = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C60_tasaCambio = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblEnc.C61_formaPago = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C61_formaPago != "")
            {
                tblEnc.C61_formaPagoCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C62_cantidadPagoAnticipados = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C63_fecha1PagoAnticipado = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C64_mercaciaManoViajeroSi = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C64a_mercanciaManoViajeroNO = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C65_sistemasEspecialesSI = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C65a_sistemasEspecialesNo = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C66_exportacionTransitoSi = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C66a_exportacionTransitoNo = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblEnc.C67_modoTransporte = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C67_modoTransporte != "")
            {
                tblEnc.C67a_modoTransporteCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C68_tipoCarga = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C68_tipoCarga != "")
            {
                tblEnc.C68_tipoCargaCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            //Lugares
            tblEnc.C69_aduanaSalida = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C69_aduanaSalida != "")
            {
                tblEnc.C69a_aduanaSalidaCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C70_paisDestinoFinal = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblEnc.C70_paisDestinoFinal != "")
            {
                tblEnc.C70a_paisDestinoFinalCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblEnc.C71_codigoDestinoFinal = datosHoja[c].Split('|')[0].Trim();
            c++;

            //Valores estadisticos totales
            tblEnc.C72_valorFobUsd = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C73_valorFletesUsd = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C74_valorSegurosUsd = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C75_valorOtrosGastosUsd = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C76_valorTotalExportacion = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C77_valorReintegroUsd = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C78_totalVanUsd = datosHoja[c].Split('|')[0].Trim();
            c++;

            //Totales para control
            tblEnc.C79_totalSeries = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C80_totalBultos = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C81_totalPesoBruto = datosHoja[c].Split('|')[0].Trim();
            c++;

            //Actuación aduanera
            tblEnc.C82_numeroAceptacion = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C83_fechaAceptacion = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C84_numeroAutorizacionEmbarque = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C85_fechaAutorizacionEmbarque = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C86_nombreFuncionarioResponsable = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C87_cargo = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C88_tipoDocumento = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C89_numeroDocumentoIdentificacion = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C90_numeroRadicacion = datosHoja[c].Split('|')[0].Trim();
            c++;
            c++;

            tblEnc.C1001_apellidosNombresQuienSuscribeDocumento = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C1002_tipoDocumentoQuienSuscribeDocumento = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C1003_numeroIdentificacionQuienSuscribeDocumento = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C1004_digitoVerificacionQuienSuscribeDocumento = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C1005_codigoRepresentacionQuienSuscribeDocumento = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C1006_organizacionQuienSuscribeDocumento = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblEnc.C997_fechaAceptacion = datosHoja[c].Split('|')[0].Trim();
        }

        private void ProcesarHoja2(string[] datosHoja, ref tmptmvDEXDetalleInterface tblDet)
        {
            //Datos del Archivo
            tblDet.C4_detalleNumeroFormulario = datosHoja[0].Split('|')[0].Trim();

            //Subpartidas declaradas
            tblDet.C98_numeroSerie = datosHoja[11].Split('|')[0].Trim();
            tblDet.C99_subPartida = datosHoja[12].Split('|')[0].Trim();
            tblDet.C100_codigoComplementario = datosHoja[13].Split('|')[0].Trim();
            tblDet.C101_codigoSuplementario = datosHoja[14].Split('|')[0].Trim();

            int c = 15;

            tblDet.C102_unidadFisica = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C102_unidadFisica != "")
            {
                tblDet.C102a_UnidadFisicaCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblDet.C103_cantidadUnidadFisica = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblDet.C104_unidadComercial = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C104_unidadComercial != "")
            {
                tblDet.C104a_unidadComercialCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblDet.C105_cantidadUnidadComercial = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblDet.C106_clasesEmbalaje = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C106_clasesEmbalaje != "")
            {
                tblDet.C106a_clasesEmbalajeCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblDet.C107_numeroBultos = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblDet.C108_pesoBrutoKilogramo = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblDet.C109_pesoNetoKilogramo = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblDet.C110_valorFobUsd = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblDet.C111_marcas = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblDet.C112_descripcionSubpartida = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblDet.C113_unidadMedidaPlazo = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C113_unidadMedidaPlazo != "")
            {
                c++;
            }

            tblDet.C114_plazo = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblDet.C115_paisOrigen = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C115_paisOrigen != "")
            {
                tblDet.C115a_paisOrigenCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblDet.C116_regionPaisOrigen = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C116_regionPaisOrigen != "")
            {
                tblDet.C116a_regionPaisOrigenCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblDet.C117_preferenciaArancelaria = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C117_preferenciaArancelaria != "")
            {
                tblDet.C117a_preferenciaArancelariaCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            //Régimen precedente
            tblDet.C118_aduanaPrecedente = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C118_aduanaPrecedente != "")
            {
                tblDet.C118a_aduanaPrecedenteCodigo = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblDet.C119_numeroDeclaracionPrecedente = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblDet.C120_anoAceptacion = datosHoja[c].Split('|')[0].Trim();
            c++;

            tblDet.C121_regimenPrecendente = datosHoja[c].Split('|')[0].Trim();
            c++;

            if (tblDet.C121_regimenPrecendente != "")
            {
                tblDet.C121a_regimenPrecendente = datosHoja[c].Split('|')[0].Trim();
                c++;
            }

            tblDet.C122_codigoModalidadPrecedente = datosHoja[c].Split('|')[0].Trim();
            c++;
            tblDet.C123_numeroSeriePrecedente = datosHoja[c].Split('|')[0].Trim();
            c++;
        }

        private void ProcesarHoja3(string[] datosHoja, ref List<tmpTmvDEXEncabezadoTransportadorInterface> listTrans)
        {
            tmpTmvDEXEncabezadoTransportadorInterface tblTrans = null; //Hojas 3

            //Se hace un ciclo de 7 iteracciones para recorrer los 7 ítems que pueden venir en la hoja
            for (int x = 0; x < 7; x++)
            {
                int c = 11 + x;
                if (datosHoja[c].Split('|')[0].Trim() != "")
                {
                    tblTrans = new tmpTmvDEXEncabezadoTransportadorInterface();

                    //Datos del Archivo
                    tblTrans.C4_transNumeroFormulario = datosHoja[0].Split('|')[0].Trim();

                    tblTrans.C124_trasn1TipoDocumento = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C125_trans1Nit = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C126_trans1DigitoVerificacion = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C127_trasn1PrimerApellido = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C128_trans1SegundoApellido = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C129_trans1PrimerNombre = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C130_trasn1OtrosNombres = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C131_trans1RazonSocial = datosHoja[c].Split('|')[0].Trim();
                    c += 7;

                    tblTrans.C132_trans1NacionalidadBandera = datosHoja[c].Split('|')[0].Trim();

                    if (tblTrans.C132_trans1NacionalidadBandera != "")
                    {
                        c++;
                        tblTrans.C132a_trans1NacionalidadBanderaCodigo = datosHoja[c].Split('|')[0].Trim();
                        c += 7;
                    }
                    else { c += 7; }

                    tblTrans.C133_trans1ManifiestoCarga = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C134_trans1FechaManifiesto = datosHoja[c].Split('|')[0].Trim();
                    c += 7;
                    tblTrans.C135_trans1IdentificacionMedioTransporte = datosHoja[c].Split('|')[0].Trim();
                    c += 7;

                    tblTrans.C136_trans1LugarEmbarque = datosHoja[c].Split('|')[0].Trim();

                    if (tblTrans.C136_trans1LugarEmbarque != "")
                    {
                        c++;
                        tblTrans.C136a_trans1LugarEmbarqueCodigo = datosHoja[c].Split('|')[0].Trim();
                    }

                    listTrans.Add(tblTrans);
                }
                else
                {
                    break;
                }
            }
        }

        private void ProcesarHoja4(string[] datosHoja, ref List<tmpTmvDEXEncabezadoSoporteInterface> listSopor)
        {
            tmpTmvDEXEncabezadoSoporteInterface tblSopor = null; //Hojas 4

            //Se hace un ciclo de 11 iteracciones para recorrer los 11 ítems que pueden venir en la hoja
            for (int x = 0; x < 11; x++)
            {
                int c = 11 + x;
                if (datosHoja[c + 11].Split('|')[0].Trim() != "")
                {
                    tblSopor = new tmpTmvDEXEncabezadoSoporteInterface();

                    //Datos del Archivo
                    tblSopor.C4_soporteNumeroFormulario = datosHoja[0].Split('|')[0].Trim();

                    tblSopor.C137_soporte1NumeroSerie = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C138_soporte1CodigoTipoDocumento = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C139_soporte1Descripcion = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C140_soporte1NumeroDocumento = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C141_soporte1NumeroIdentficacionEmisor = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C142_soporte1DigitoVerificacion = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C143_soporte1NombreEmisor = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C144_soporte1FechaExpedicion = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C145_soporte1FechaVencimiento = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    tblSopor.C146_soporte1CodigoMoneda = datosHoja[c].Split('|')[0].Trim();
                    c += 11;
                    //tblSopor.C147_soporte1montoDocumento = datosHoja[c].Split('|')[0].Trim();
                    //c += 11;
                    //tblSopor.C148_soporte1UnidadComercial = datosHoja[c].Split('|')[0].Trim();
                    //c += 11;

                    listSopor.Add(tblSopor);
                }
                else {
                    break;
                }
            }
        }

        private void ProcesarHoja8(string[] datosHoja, ref tmptmvDEXDetalleContinuacionInterface tblDetCont)
        {
            tblDetCont.C4_detalleContinuacionNumeroFormulario = datosHoja[0].Split('|')[0].Trim();
            tblDetCont.C172_numeroSerie = datosHoja[12].Split('|')[0].Trim();
            tblDetCont.C112_descripcionSubpartida = datosHoja[13].Split('|')[0].Trim();
        }
    }
}