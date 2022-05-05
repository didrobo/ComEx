using ComEx.Context;
using ComEx.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class ArchivosController : Controller
    {
        private db_comexEntities db = new db_comexEntities();

        // GET: Archivos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrdenesCompra() 
        {
            IEnumerable<dynamic> LinqQuery =
            (
                from a in db.tmRegalias
                join b in db.tmLineas
                on a.intIdLnea equals b.intIdLnea
                orderby a.intAno descending, a.intMes descending
                select new
                {
                    intIdRglia = a.intIdRglia,
                    varCdLnea = b.varCdLnea,
                    intAno = a.intAno,
                    intMes = a.intMes
                }
            );

            return View(LinqQuery);
        }

        [HttpPost]
        public ActionResult ProcesarArchivo()
        {
            //Variables
            object objJson = null;
            //Variables OUT del procedimiento
            ObjectParameter intErrores = new ObjectParameter("Errores", typeof(int));
            ObjectParameter intBuenos = new ObjectParameter("Buenos", typeof(int));
            ObjectParameter intTotalRegistros = new ObjectParameter("TotalRegistros", typeof(int));
            ObjectParameter dcFactorOnza = new ObjectParameter("factorOnza", typeof(decimal));
            ObjectParameter intRetencion = new ObjectParameter("retencion", typeof(int));
            ObjectParameter dcBseRglias = new ObjectParameter("numBseRglias", typeof(decimal));
            ObjectParameter dcBseRgliasAG = new ObjectParameter("numBseRgliasAG", typeof(decimal));
            ObjectParameter dcBseRgliasPT = new ObjectParameter("numBseRgliasPT", typeof(decimal));
            ObjectParameter dcFactor = new ObjectParameter("numFctor", typeof(decimal));
            ObjectParameter dcTotalBrutos = new ObjectParameter("numTotalBrutos", typeof(decimal));
            ObjectParameter dcTotalFinos = new ObjectParameter("numTotalFinos", typeof(decimal));
            ObjectParameter dcTotalFinosAG = new ObjectParameter("numTotalFinosAG", typeof(decimal));
            ObjectParameter dcTotalFinosPT = new ObjectParameter("numTotalFinosPT", typeof(decimal));
            ObjectParameter dcTotalRegalias = new ObjectParameter("numTotalRegalias", typeof(decimal));

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
                    objJson = new { success = false, Message = "Asegúrese de seleccionar un archivo con extensión xsl ó xlsx." };
                    return Json(objJson, JsonRequestBehavior.AllowGet);
                }

                //Validar que se ingrese un lote
                if (string.IsNullOrWhiteSpace(Request["Lote"]))
                {
                    objJson = new { success = false, Message = "Asegúrese de ingresar el lote." };
                    return Json(objJson, JsonRequestBehavior.AllowGet);
                }

                //Validar la selección de una regalía
                if (string.IsNullOrWhiteSpace(Request["Regalia"]))
                {
                    objJson = new { success = false, Message = "Asegúrese de seleccionar la regalía" };
                    return Json(objJson, JsonRequestBehavior.AllowGet);
                }
                #endregion

                //Archivo
                HttpPostedFileBase Archivo = Request.Files[0];
                //Obtener la extensión del archivo
                string strExtensionArchivo = Path.GetExtension(Archivo.FileName);
                //Validar si la extensión es permitida
                if(!strExtensionArchivo.Equals(".xls") && !strExtensionArchivo.Equals(".xlsx"))
                {
                    objJson = new { success = false, Message = "Solo se permiten extensiones xls y xlsx" };
                    return Json(objJson, JsonRequestBehavior.AllowGet);
                }

                //Recuperar el lote ingresado. Los convertimos a mayuscula por si el javascript no lo puede realizar
                string strLote = Request["Lote"].ToUpper();

                //Recuperar la regalia selecciona. La convertimos a entero
                int intRegalia = Convert.ToInt32(Request["Regalia"]);
                //Recuperar información de la regalia
                var regaliaSeleccionada =
                (
                    from a in db.tmRegalias
                    join b in db.tmLineas
                    on a.intIdLnea equals b.intIdLnea
                    where a.intIdRglia == intRegalia
                    orderby a.intAno descending, a.intMes descending
                    select new
                    {
                        intIdRglia = a.intIdRglia,
                        varCdLnea = b.varCdLnea,
                        intAno = a.intAno,
                        intMes = a.intMes
                    }
                ).FirstOrDefault();

                //Subir archivo a la carpeta tmp
                string strRutaArchivo = Server.MapPath("~/tmp/") + Archivo.FileName;
                Archivo.SaveAs(strRutaArchivo);

                // Se trunca la tabla temporal
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE temporal.tmpListaComprasImportadas");

                //Realizar bulkcopy
                //Recuperdar DataTable
                DataTable  dtExcel = Helpers.Excel.GetData(strRutaArchivo, "ComEx");

                //Mapear columnas
                List<SqlBulkCopyColumnMapping> lstColumnas = new List<SqlBulkCopyColumnMapping>();
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "tipoMaterial", SourceOrdinal = 0 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "gramosBrutos", SourceOrdinal = 1 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "ley", SourceOrdinal = 2 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "leyAG", SourceOrdinal = 3 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "leyPT", SourceOrdinal = 4 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "valor", SourceOrdinal = 5 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "nit", SourceOrdinal = 6 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "procedencia", SourceOrdinal = 7 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "municipio", SourceOrdinal = 8 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "departamento", SourceOrdinal = 9 });                
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "porcMinero", SourceOrdinal = 10 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "proyecto", SourceOrdinal = 11 });
                lstColumnas.Add(new SqlBulkCopyColumnMapping() { DestinationColumn = "llave", SourceOrdinal = 12 });


                //Copiar los datos al servidor
                SqlServer clsSql = new SqlServer();
                clsSql.BulkCopy("temporal.tmpListaComprasImportadas", dtExcel, lstColumnas);

                //Ejecutar procedimiento spInsertarTmpComprasAnexpo
                db.spInsertarTmpCompras(strLote, intRegalia, intErrores, intBuenos, intTotalRegistros, dcFactorOnza, intRetencion, dcBseRglias, dcBseRgliasAG, dcBseRgliasPT, dcFactor, dcTotalBrutos, dcTotalFinos, dcTotalFinosAG, dcTotalFinosPT, dcTotalRegalias);

                ////Recuperar los datos con el procedimiento spTmpCompras
                //List<spTmpCompras_Result> lstTmpCompras = db.spTmpCompras().ToList();

                //Devolver mensaje
                objJson = new
                {
                    success = true,
                    Message = "Ordenes de compras cargadas correctamente.",
                    strLote = strLote,
                    regalia = regaliaSeleccionada,
                    intErrores = intErrores.Value,
                    intBuenos = intBuenos.Value,
                    intTotalRegistros = intTotalRegistros.Value,
                    dcFactorOnza = dcFactorOnza.Value,
                    intRetencion = intRetencion.Value,
                    dcBseRglias = dcBseRglias.Value,
                    dcBseRgliasAG = dcBseRgliasAG.Value,
                    dcBseRgliasPT = dcBseRgliasPT.Value,
                    dcFactor = dcFactor.Value,
                    dcTotalBrutos = dcTotalBrutos.Value,
                    dcTotalFinos = dcTotalFinos.Value,
                    dcTotalFinosAG = dcTotalFinosAG.Value,
                    dcTotalFinosPT = dcTotalFinosPT.Value,
                    dcTotalRegalias = dcTotalRegalias.Value
                };

                //Eliminar archivo
                System.IO.File.Delete(strRutaArchivo);
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ArchivosController.ProcesarArchivo", usr.varLgin);
                objJson = new { success = false, Message = string.Format("Error al cargar las ordenes de compras.\n\r {0}", ex.Message + " " + ex.InnerException) };
            }

            return Json(objJson, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTempCompras()
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
                //Recuperar los datos con el procedimiento spTmpCompras
                List<spTmpCompras_Result> lstTmpCompras = db.spTmpCompras().ToList();
                return Json(new { data = lstTmpCompras }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ArchivosController.GetTempCompras", usr.varLgin);
                objJson = new { success = false, Message = ex.Message + " " + ex.InnerException };
            }

            return Json(objJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidarNumFactura(int intIdtmpCompras, string factura, string prefijo) {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null) {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try {
                    ObjectParameter bitValid = new ObjectParameter("bitValid", typeof(bool));
                    ObjectParameter mensaje = new ObjectParameter("mensaje", typeof(string));

                    //Ejecuta el procedimiento de validación
                    db.spValidarNumFacturaInterface(intIdtmpCompras, factura, prefijo, bitValid, mensaje);

                    result = Json(new { success = true, bitValid = bitValid.Value, mensaje = mensaje.Value });                    
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ArchivosController.ValidarNumFactura", usr.varLgin);
                    result = Json(new { success = false, Message = string.Format("Error al validar la factura.\n\r {0}", ex.Message + " " + ex.InnerException) });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        public JsonResult SubirOrdenesCompra() {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null) {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try {
                    //Ejecuta el procedimiento de creación de ordenes de compra
                    db.spInsertarTrasladoCompras(usr.varLgin);

                    result = Json(new { success = true, Message = "Se crearon las ordenes de compra correctamente" });    
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "ArchivosController.SubirOrdenesCompra", usr.varLgin);
                    result = Json(new { success = false, Message = string.Format("Error al cargar las ordenes de compra.\n\r {0}", ex.Message + " " + ex.InnerException) });
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
