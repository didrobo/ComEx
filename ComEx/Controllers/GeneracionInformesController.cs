using ComEx.Context;
using ComEx.Helpers;
using ComEx.Models.Entities;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComEx.Controllers
{
    public class GeneracionInformesController : Controller
    {
        // GET: GeneracionInformes
        public ActionResult ArchivosContables()
        {
            return View();
        }

        public ActionResult ElaboracionFacturas()
        {
            return View();
        }

        public JsonResult GenerarArchivoContable(Propiedades[] datosArchivoContable)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    string cmbTipoArchivo = datosArchivoContable.Where(w => w.name.Equals("cmbTipoArchivo")).Select(s => s.value).FirstOrDefault();
                    string cmbLote = datosArchivoContable.Where(w => w.name.Equals("cmbLote")).Select(s => s.value).FirstOrDefault().ToUpper();

                    using (db_comexEntities db = new db_comexEntities())
                    {
                        EjecutarSpLlenadoDatos(db.Database.Connection.ConnectionString, cmbTipoArchivo, cmbLote);
                        result = EjecutarConsulta(db.Database.Connection.ConnectionString, cmbTipoArchivo, cmbLote);
                    }

                    //result = Json(new { success = true, mensaje });
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "GeneracionInformesController.GenerarArchivoContable", usr.varLgin);
                    result = Json(new { success = false, error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }

            return result;
        }

        private void EjecutarSpLlenadoDatos(string connString, string tipoArchivo, string lote)
        {
            SqlConnection connection = new SqlConnection(connString);
            connection.Open();

            SqlCommand command = new SqlCommand(getNombreSP(tipoArchivo), connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@nroLote", SqlDbType.VarChar).Value = lote;

            command.ExecuteNonQuery();

            connection.Close();
        }

        private string getNombreSP(string tipoArchivo)
        {
            if (tipoArchivo.Equals("Notas Contables"))
            {
                return "[anexpo].[ContableNotas]";
            }
            else if (tipoArchivo.Equals("Compras y Ventas"))
            {
                return "[anexpo].[Contable]";
            }
            else
            {
                return "";
            }
        }

        private JsonResult EjecutarConsulta(string connString, string tipoArchivo, string lote)
        {
            JsonResult result = new JsonResult();
            bool success = false;

            SqlConnection connection = new SqlConnection(connString);
            connection.Open();

            DataSet dsConsulta = new DataSet("Consulta");
            SqlCommand command = new SqlCommand("[anexpo].[ContableConsulta]", connection);
            command.CommandType = CommandType.StoredProcedure;

            foreach (string comprobante in GetComprobante(tipoArchivo))
            {
                command.Parameters.Add("@nroLote", SqlDbType.VarChar).Value = lote;
                command.Parameters.Add("@comprobante", SqlDbType.VarChar).Value = comprobante;

                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dsConsulta, "tblConsulta");

                if (dsConsulta.Tables["tblConsulta"].Rows.Count > 0)
                {
                    EscribirArchivo(dsConsulta.Tables["tblConsulta"], GetNombreArchivo(comprobante) + lote);
                    success = true;
                }
                else
                {
                    result = Json(new { success = false, error = "No se encontraron registros para el\nLote: " + lote + " Comprobante: " + comprobante });
                    success = false;
                }

                command.Parameters.RemoveAt("@nroLote");
                command.Parameters.RemoveAt("@comprobante");
            }

            connection.Close();

            if (success)
            {
                result = Json(new { success = true, mensaje = "Se generaron los archivos para las " + tipoArchivo });
            }

            return result;
        }

        private string[] GetComprobante(string tipoArchivo)
        {
            if (tipoArchivo.Equals("Notas Contables"))
            {
                return new string[] { "00024", "00025" };
            }
            else if (tipoArchivo.Equals("Compras y Ventas"))
            {
                return new string[] { "00003", "00004" };
            }
            else
            {
                return null;
            }
        }

        private string GetNombreArchivo(string comprobante)
        {
            if (comprobante.Equals("00003"))
            {
                return "3compras_";
            }
            else if (comprobante.Equals("00004"))
            {
                return "4ventas_";
            }
            else if (comprobante.Equals("00024"))
            {
                return "24Credito_";
            }
            else if (comprobante.Equals("00025"))
            {
                return "25Debito_";
            }
            else
            {
                return null;
            }
        }

        private void EscribirArchivo(System.Data.DataTable tabla, string nombreArchivo)
        {
            try
            {
                string strNombreArchivo = nombreArchivo + ".txt";
                string strRutaArchivo = Server.MapPath("~/Archivos/Contables");
                string strRuta_Archivo = string.Format(@"{0}/{1}", strRutaArchivo, strNombreArchivo);

                //Validar si existe la ruta.
                if (!Directory.Exists(strRutaArchivo))
                {
                    Directory.CreateDirectory(strRutaArchivo);
                }

                if (System.IO.File.Exists(strRuta_Archivo))
                {
                    System.IO.File.Delete(strRuta_Archivo);
                }

                using (StreamWriter sw = new StreamWriter(strRuta_Archivo, true))
                {
                    foreach (DataRow row in tabla.Rows)
                    {
                        List<string> items = new List<string>();
                        foreach (DataColumn col in tabla.Columns)
                        {
                            items.Add(row[col.ColumnName].ToString());
                        }
                        string linea = string.Join("\t", items.ToArray());
                        sw.WriteLine(linea);
                    }
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GenerarElaboracionFacturas(DateTime fecDesde, DateTime fecHasta)
        {
            JsonResult result = new JsonResult();

            if (Session["UsuarioLogueado"] != null)
            {
                tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];

                try
                {
                    using (db_comexEntities db = new db_comexEntities())
                    {
                        result = CrearExcelElabFacturas(db.Database.Connection.ConnectionString, fecDesde, fecHasta);
                    }
                }
                catch (Exception ex)
                {
                    Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "GeneracionInformesController.GenerarElaboracionFacturas", usr.varLgin);
                    result = Json(new { success = false, error = ex.Message + " " + ex.InnerException });
                }
            }
            else
            {
                RedirectToAction("LogOut", "Home");
            }
            return result;
        }

        private JsonResult CrearExcelElabFacturas(string connString, DateTime fecDesde, DateTime fecHasta)
        {
            JsonResult result = new JsonResult();

            SqlConnection connection = new SqlConnection(connString);
            connection.Open();

            DataSet dsConsulta = new DataSet("Consulta");

            string query = "SELECT CodInterno, Factura, Fecha, Documento, Cliente, Material, Gramos, Regalias, ValorPagar, ValorAntesRetencion, Ley1429, Finos, " +
                "Ley, ValorUnitario, numSbttal, numVlorRglias, Retencion, intCnsctvoCrgue, varCiudad, varDrccion, idRegimen, Regimen, varCdCP, fecCP, varNmroCP, " +
                "intIdRglia, VlRegalia, AñoCompra, MesCompra, DiaCompra, CiudadRegalias, FacExpo, varNmroDEX, fecAprbcionDEX, Lote, NombreProveedor, PaisProveedor, " +
                "Exportacion FROM anexpo.ComprasDetalles WHERE Fecha >= @fechaInicial AND Fecha <= @fechaFinal AND Regimen = 'RC'";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddRange(
                new List<SqlParameter>() {
                    new SqlParameter("@fechaInicial", fecDesde),
                    new SqlParameter("@fechaFinal", fecHasta)
                }.ToArray()
            );

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dsConsulta, "tblConsulta");

            if (dsConsulta.Tables["tblConsulta"].Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application oExcel;
                Workbooks oLibros;
                Workbook oLibro;
                Sheets oHojas;
                Worksheet oHoja;
                Range oCeldas;

                string sFile, sTemplate;
                string strRutaArchivo = Server.MapPath("~/Archivos/Facturas");

                sTemplate = string.Format(@"{0}/{1}", Server.MapPath("~/Report"), "PlantillaGeneracionFacturas.xltx");

                foreach (DataRow row in dsConsulta.Tables["tblConsulta"].Rows)
                {
                    oExcel = new Microsoft.Office.Interop.Excel.Application();
                    string ruta = strRutaArchivo + @"\" + row["Lote"].ToString() + @"\";

                    if (!Directory.Exists(ruta))
                    {
                        Directory.CreateDirectory(ruta);
                    }

                    sFile = ruta + row["CodInterno"].ToString() + "_" + row["Cliente"].ToString().Replace(" ", "_").Replace(".", "") + ".xls";

                    oExcel.Visible = false;
                    oExcel.DisplayAlerts = false;

                    oLibros = oExcel.Workbooks;
                    oLibros.Open(sTemplate);
                    oLibro = oLibros.Item[1];
                    oHojas = oLibro.Worksheets;
                    oHoja = (Worksheet)oHojas.Item[1];
                    oHoja.Name = "Hoja1";
                    oCeldas = oHoja.Cells;

                    LlenarDatosExcel(oCeldas, row);

                    oHoja.SaveAs(sFile);
                    oLibro.Close();

                    oLibros = null;
                    oLibro = null;
                    oHojas = null;
                    oHoja = null;
                    oCeldas = null;
                    oExcel.Quit();
                    oExcel = null;
                    GC.Collect();
                }

                result = Json(new { success = true, mensaje = "Se crearon los archivos de las facturas correctamente" });
            }
            else
            {
                result = Json(new { success = false, error = "No se encontraron registros en el rango de fechas seleccionado" });
                return result;
            }

            return result;
        }

        private void LlenarDatosExcel(Range oCeldas, DataRow fila)
        {
            oCeldas[5, 1] = (DateTime)fila["Fecha"]; //Fecha Factura
            oCeldas[5, 2] = fila["Cliente"].ToString(); //Nombre
            oCeldas[5, 3] = fila["Material"].ToString(); //Material
            oCeldas[5, 4] = (decimal)fila["Gramos"]; //Gramos Brutos
            oCeldas[5, 5] = (decimal)fila["Finos"]; //Gramos Finos
            oCeldas[5, 6] = (decimal)fila["ValorAntesRetencion"]; //Valor
            oCeldas[5, 7] = (decimal)fila["Regalias"]; //Regalías
            oCeldas[5, 8] = (decimal)fila["numSbttal"]; //Total Costo
            oCeldas[5, 9] = (decimal)fila["Retencion"]; //Retención
            oCeldas[5, 10] = (decimal)fila["ValorPagar"]; //Valor a pagar

            oCeldas[8, 1] = oCeldas[8, 1].Value + fila["Factura"].ToString(); //Número Factura

            oCeldas[12, 1] = string.Format("{0} gramos de {1}", fila["Gramos"].ToString(), fila["Material"].ToString()); //Descripción

            oCeldas[12, 3] = (decimal)fila["numSbttal"]; //Total Costo
            oCeldas[14, 3] = (decimal)fila["numSbttal"]; //Subtotal
            oCeldas[16, 3] = (decimal)fila["numSbttal"]; //Total
        }
    }
}