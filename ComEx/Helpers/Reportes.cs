using ComEx.Context;
using ComEx.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ComEx.Helpers
{
    public class Reportes
    {
        public static ReportViewer RptOrdenCompra(ReportViewer rptvPrincipal, tcUsuarios clsUsuario, string RutaNombreReporte, string strFirma, List<DTFilters> listFiltros)
        {
            //Variables
            List<vFormatoOrdenesCompraCI> lstFormatoOrdenCompra = new List<vFormatoOrdenesCompraCI>();
            List<vOrdenesCompraCI> listOrdenesCompraCI = new List<vOrdenesCompraCI>();

            db_comexEntities db = null;
            try
            {
                using (db = new db_comexEntities())
                {                    
                    //Si la tabla fue filtrada se aplican los filtros
                    if (listFiltros != null)
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            listOrdenesCompraCI = db.vOrdenesCompraCI.AsNoTracking().Where(x => x.fecCmpra.Value.Year == ano).ToList();
                        }
                        else
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.AsNoTracking().ToList();
                        }

                        IQueryable<vOrdenesCompraCI> results = listOrdenesCompraCI.AsQueryable();
                        listOrdenesCompraCI = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                        if (listOrdenesCompraCI.Count > 0)
                        {
                            lstFormatoOrdenCompra = db.vFormatoOrdenesCompraCI.ToList();

                            lstFormatoOrdenCompra = lstFormatoOrdenCompra.Where(
                                x => listOrdenesCompraCI.Select(
                                    s => s.intIdCmpra
                                ).Contains(x.intIdCmpra)
                            ).ToList();
                        }
                    }
                    else
                    {
                        lstFormatoOrdenCompra = db.vFormatoOrdenesCompraCI.ToList();
                    }
                   
                    rptvPrincipal.LocalReport.ReportPath = RutaNombreReporte;
                    rptvPrincipal.ShowExportControls = true;
                    rptvPrincipal.ShowPrintButton = false;
                    rptvPrincipal.ShowFindControls = false;
                    rptvPrincipal.LocalReport.DataSources.Clear();
                    //ReportDataSource dtReporte = new ReportDataSource("dtOrdenCompra", lstFormatoOrdenCompra);
                    rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtOrdenCompra", lstFormatoOrdenCompra));
                    //Agregar firma
                    rptvPrincipal.LocalReport.EnableExternalImages = true;
                    ReportParameter prmImagen = new ReportParameter();
                    prmImagen.Name = "Firma";
                    prmImagen.Values.Add(strFirma);
                    rptvPrincipal.LocalReport.SetParameters(prmImagen);
                }
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "Reportes.RptOrdenCompra", clsUsuario.varLgin);
            }

            return rptvPrincipal;
        }

        public static ReportViewer RptDocEquivalente(ReportViewer rptvPrincipal, tcUsuarios clsUsuario, string RutaNombreReporte, List<DTFilters> listFiltros, string strLogo)
        {
            //Variables
            List<vFormatoDocEquivalenteCI> lstFormatoDocEquivalente = new List<vFormatoDocEquivalenteCI>();
            List<vOrdenesCompraCI> listOrdenesCompraCI = new List<vOrdenesCompraCI>();

            db_comexEntities db = null;

            try
            {
                using (db = new db_comexEntities())
                {
                    //Si la tabla fue filtrada se aplican los filtros
                    if (listFiltros != null)
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            listOrdenesCompraCI = db.vOrdenesCompraCI.AsNoTracking().Where(x => x.fecCmpra.Value.Year == ano).ToList();
                        }
                        else
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.AsNoTracking().ToList();
                        }

                        IQueryable<vOrdenesCompraCI> results = listOrdenesCompraCI.AsQueryable();
                        listOrdenesCompraCI = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                        if (listOrdenesCompraCI.Count > 0)
                        {
                            lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCI.ToList();

                            lstFormatoDocEquivalente = lstFormatoDocEquivalente.Where(
                                x => listOrdenesCompraCI.Select(
                                    s => s.intIdCmpra
                                ).Contains(x.intIdCmpra)
                            ).ToList();
                        }
                    }
                    else
                    {
                        lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCI.ToList();
                    }

                    rptvPrincipal.LocalReport.ReportPath = RutaNombreReporte;
                    rptvPrincipal.ShowExportControls = true;
                    rptvPrincipal.ShowPrintButton = false;
                    rptvPrincipal.ShowFindControls = false;
                    rptvPrincipal.LocalReport.DataSources.Clear();
                    rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtDocEquivalente", lstFormatoDocEquivalente));
                    //Agregar Logo
                    rptvPrincipal.LocalReport.EnableExternalImages = true;
                    ReportParameter prmLogo = new ReportParameter();
                    prmLogo.Name = "Logo";
                    prmLogo.Values.Add(strLogo);
                    rptvPrincipal.LocalReport.SetParameters(prmLogo);
                }
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "Reportes.RptDocEquivalente", clsUsuario.varLgin);
            }

            return rptvPrincipal;
        }

        public static ReportViewer RptDocEquivalenteNuevo(ReportViewer rptvPrincipal, tcUsuarios clsUsuario, string RutaNombreReporte, List<DTFilters> listFiltros, string strLogo)
        {
            //Variables
            List<vFormatoDocEquivalenteCINuevo> lstFormatoDocEquivalente = new List<vFormatoDocEquivalenteCINuevo>();
            List<vOrdenesCompraCI> listOrdenesCompraCI = new List<vOrdenesCompraCI>();

            db_comexEntities db = null;

            try
            {
                using (db = new db_comexEntities())
                {
                    //Si la tabla fue filtrada se aplican los filtros
                    if (listFiltros != null)
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            listOrdenesCompraCI = db.vOrdenesCompraCI.AsNoTracking().Where(x => x.fecCmpra.Value.Year == ano).ToList();
                        }
                        else
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.AsNoTracking().ToList();
                        }

                        IQueryable<vOrdenesCompraCI> results = listOrdenesCompraCI.AsQueryable();
                        listOrdenesCompraCI = Funciones.FiltrarListDataTables(results, listFiltros).ToList();

                        if (listOrdenesCompraCI.Count > 0)
                        {
                            lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCINuevo.ToList();

                            lstFormatoDocEquivalente = lstFormatoDocEquivalente.Where(
                                x => listOrdenesCompraCI.Select(
                                    s => s.intIdCmpra
                                ).Contains(x.intIdCmpra)
                            ).ToList();
                        }
                    }
                    else
                    {
                        lstFormatoDocEquivalente = db.vFormatoDocEquivalenteCINuevo.ToList();
                    }

                    rptvPrincipal.LocalReport.ReportPath = RutaNombreReporte;
                    rptvPrincipal.ShowExportControls = true;
                    rptvPrincipal.ShowPrintButton = false;
                    rptvPrincipal.ShowFindControls = false;
                    rptvPrincipal.LocalReport.DataSources.Clear();
                    rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtDocEquivalenteNuevo", lstFormatoDocEquivalente));
                    //Agregar Logo
                    rptvPrincipal.LocalReport.EnableExternalImages = true;
                    ReportParameter prmLogo = new ReportParameter();
                    prmLogo.Name = "Logo";
                    prmLogo.Values.Add(strLogo);
                    rptvPrincipal.LocalReport.SetParameters(prmLogo);
                }
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "Reportes.RptDocEquivalenteNuevo", clsUsuario.varLgin);
            }

            return rptvPrincipal;
        }

        public static ReportViewer RptRemisionCP(ReportViewer rptvPrincipal, tcUsuarios clsUsuario, string RutaNombreReporte, List<DTFilters> listFiltros, string strFirma)
        {
            //Variables            
            List<vOrdenesCompraCI> listOrdenesCompraCI = new List<vOrdenesCompraCI>();

            db_comexEntities db = null;

            try
            {
                using (db = new db_comexEntities())
                {
                    //Si la tabla fue filtrada se aplican los filtros
                    if (listFiltros != null)
                    {
                        if (GlobalConfig.get().ChkFiltroPorAno)
                        {
                            int ano = GlobalConfig.get().AppAnoFiltro;
                            listOrdenesCompraCI = db.vOrdenesCompraCI.AsNoTracking().Where(x => x.fecCmpra.Value.Year == ano).ToList();
                        }
                        else
                        {
                            listOrdenesCompraCI = db.vOrdenesCompraCI.AsNoTracking().ToList();
                        }

                        IQueryable<vOrdenesCompraCI> results = listOrdenesCompraCI.AsQueryable();
                        listOrdenesCompraCI = Funciones.FiltrarListDataTables(results, listFiltros).ToList();
                    }
                    else
                    {
                        listOrdenesCompraCI = db.vOrdenesCompraCI.ToList();
                    }

                    //Consulta los parametros de configuracion
                    tcParametros tcParametros = db.tcParametros.First();

                    //Consulta el usuario
                    tcUsuarios tcUsuarios = db.tcUsuarios.Where(x => x.intIdUsuario == tcParametros.intIdUsrFrmaRmsionCP).First();

                    rptvPrincipal.LocalReport.ReportPath = RutaNombreReporte;
                    rptvPrincipal.ShowExportControls = true;
                    rptvPrincipal.ShowPrintButton = false;
                    rptvPrincipal.ShowFindControls = false;
                    rptvPrincipal.LocalReport.DataSources.Clear();
                    rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtRemisionCP", listOrdenesCompraCI));

                    //Agregar firma
                    rptvPrincipal.LocalReport.EnableExternalImages = true;
                    ReportParameter prmImagen = new ReportParameter();
                    prmImagen.Name = "ImgFirma";
                    strFirma = String.Format(strFirma, tcUsuarios.varImgFrma);
                    prmImagen.Values.Add(strFirma);
                    rptvPrincipal.LocalReport.SetParameters(prmImagen);

                    //Agregar Nombre firma
                    ReportParameter prmNomFirma = new ReportParameter();
                    prmNomFirma.Name = "NomFirma";
                    prmNomFirma.Values.Add(tcUsuarios.varNmbreUsuario);
                    rptvPrincipal.LocalReport.SetParameters(prmNomFirma);

                    //Agregar Cargo firma
                    ReportParameter prmCargoFirma = new ReportParameter();
                    prmCargoFirma.Name = "CargoFirma";
                    prmCargoFirma.Values.Add(tcUsuarios.varCrgo);
                    rptvPrincipal.LocalReport.SetParameters(prmCargoFirma);
                }
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "Reportes.RptRemisionCP", clsUsuario.varLgin);
            }

            return rptvPrincipal;
        }

        public static ReportViewer RptSobres(ReportViewer rptvPrincipal, tcUsuarios clsUsuario, string RutaNombreReporte, List<DTFilters> listFiltros, DateTime desde, DateTime hasta)
        {
            //Variables
            List<spTmRemisionDocumentos_Result> listRemisionDocumentos = new List<spTmRemisionDocumentos_Result>();
            List<vProveedores> listProveedores = new List<vProveedores>();

            db_comexEntities db = null;
            
            try
            {
                using (db = new db_comexEntities())
                {

                    listRemisionDocumentos = db.spTmRemisionDocumentos(desde, hasta).ToList();

                    if (listFiltros != null)
                    {
                        IQueryable<spTmRemisionDocumentos_Result> results = listRemisionDocumentos.AsQueryable();
                        listRemisionDocumentos = Funciones.FiltrarListDataTables(results, listFiltros).ToList();
                    }

                    if (listRemisionDocumentos.Count > 0)
                    {
                        listProveedores = db.vProveedores.ToList();

                        listProveedores = listProveedores.Where(
                            x => listRemisionDocumentos.Select(
                                s => s.intIdPrvdor
                            ).Contains(x.intIdPrvdor)
                        ).ToList();
                    }


                    rptvPrincipal.LocalReport.ReportPath = RutaNombreReporte;
                    rptvPrincipal.ShowExportControls = true;
                    rptvPrincipal.ShowPrintButton = false;
                    rptvPrincipal.ShowFindControls = false;
                    rptvPrincipal.LocalReport.DataSources.Clear();
                    rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtSobres", listProveedores));
                }
            }
            catch (Exception ex)
            {
                Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "Reportes.RptSobres", clsUsuario.varLgin);
            }

            return rptvPrincipal;
        }
    }
}