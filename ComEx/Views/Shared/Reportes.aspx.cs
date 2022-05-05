using ComEx.Context;
using ComEx.Helpers;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComEx.Views.Shared
{
    public partial class Reportes : Page
    {
        private tcUsuarios Usr { get; set; }

        //protected void Page_Load(object sender, EventArgs e)
        //{

        //    if (Session["UsuarioLogueado"] != null)
        //    {
        //        tcUsuarios usr = (tcUsuarios)Session["UsuarioLogueado"];
        //    }

        //    if (!IsPostBack)
        //    {
        //        switch(Request.QueryString["Reporte"])
        //        {
        //            case "RptOrdenCambio":
        //                RptOrdenCambio();
        //            break;
        //        }
        //    }
        //}

        //private void RptOrdenCambio()
        //{
        //    //Variables
        //    List<vFormatoOrdenesCompraCI> lstFormatoOrdenCompra = null;
        //    db_comexEntities db = null;
        //    try
        //    {
        //        using (db = new db_comexEntities())
        //        {
        //            lstFormatoOrdenCompra = db.vFormatoOrdenesCompraCI.ToList();
        //            rptvPrincipal.LocalReport.ReportPath = Server.MapPath("~/Report/OrdenCompra.rdlc");
        //            rptvPrincipal.LocalReport.DataSources.Clear();
        //            //ReportDataSource dtReporte = new ReportDataSource("dtOrdenCompra", lstFormatoOrdenCompra);
        //            rptvPrincipal.LocalReport.DataSources.Add(new ReportDataSource("dtOrdenCompra", lstFormatoOrdenCompra));
        //            rptvPrincipal.LocalReport.Refresh();
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Log.GetInstancia().EscribirLog(ex.Message + "   " + ex.InnerException, "Error", "Reportes.RptOrdenCambio", Usr.varLgin);
        //    }
        //    finally
        //    {
        //        if(db != null)
        //        {
        //            if(db.Database.Connection.State == ConnectionState.Open)
        //            {
        //                db.Database.Connection.Close();
        //            }
        //        }

        //        lstFormatoOrdenCompra = null;
        //        db = null;
        //    }
        //}
    }
}