<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="ComEx.Views.Shared.Reportes" %>--%>
<%--<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>--%>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%--<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>--%>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html>
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Reportes</title>
    <script runat="server">        
        protected void Page_Load(object sender, EventArgs e)
        {
            ComEx.Context.tcUsuarios clsUsuario = null;
            List<ComEx.Models.DTFilters> listFiltros = null;

            if (Session["UsuarioLogueado"] != null)
            {
                clsUsuario = (ComEx.Context.tcUsuarios)Session["UsuarioLogueado"];
            }

            if (Session["listFiltros"] != null)
            {
                listFiltros = (List<ComEx.Models.DTFilters>)Session["listFiltros"];
            }            

            if (!IsPostBack)
            {
                
                switch (RouteData.Values["Reporte"].ToString())
                {
                    case "RptOrdenCompra":
                        string strReporte = Server.MapPath("~/Report/OrdenCompra.rdlc");
                        string strFirma = Server.MapPath("~/img/Reportes/OrdenCompra/firma.png");
                        rptvPrincipal = ComEx.Helpers.Reportes.RptOrdenCompra(rptvPrincipal, clsUsuario, strReporte, strFirma, listFiltros);
                        break;
                }

                rptvPrincipal.LocalReport.Refresh();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="rptvPrincipal" runat="server" AsyncRendering="false" SizeToReportContent="true"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
