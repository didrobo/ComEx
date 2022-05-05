<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContenedorReport.aspx.cs" Inherits="ComEx.Report.ContenedorReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Reportes</title>
    <link href="../bower_components/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="../bower_components/alertifyjs/css/alertify.css" media="all" rel="stylesheet" type="text/css" />

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
                string strReporte, strFirma, strLogo;
                hfReporte.Value = Request.QueryString["Reporte"].ToString();
                switch (Request.QueryString["Reporte"].ToString())
                {
                    case "RptOrdenCompra":
                        strReporte = Server.MapPath("~/Report/OrdenCompra.rdlc");
                        strFirma = Server.MapPath("~/img/Reportes/OrdenCompra/firma.png");
                        rptvPrincipal = ComEx.Helpers.Reportes.RptOrdenCompra(rptvPrincipal, clsUsuario, strReporte, strFirma, listFiltros);
                        break;
                    case "RptDocEquivalente":
                        strReporte = Server.MapPath("~/Report/DocumentoEquivalente.rdlc");
                        strLogo = Server.MapPath("~/img/Logo_Anexpo.png");
                        rptvPrincipal = ComEx.Helpers.Reportes.RptDocEquivalente(rptvPrincipal, clsUsuario, strReporte, listFiltros, strLogo);
                        break;
                    case "RptDocEquivalenteNuevo":
                        strReporte = Server.MapPath("~/Report/DocumentoEquivalenteNuevo.rdlc");
                        strLogo = Server.MapPath("~/img/Logo_Anexpo.png");
                        rptvPrincipal = ComEx.Helpers.Reportes.RptDocEquivalenteNuevo(rptvPrincipal, clsUsuario, strReporte, listFiltros, strLogo);
                        break;
                    case "RptRemisionCP":
                        strReporte = Server.MapPath("~/Report/RemisionCPS.rdlc");
                        strFirma = Server.MapPath("~/img/Reportes/RemisionCP/{0}");
                        rptvPrincipal = ComEx.Helpers.Reportes.RptRemisionCP(rptvPrincipal, clsUsuario, strReporte, listFiltros, strFirma);
                        break;
                    case "RptSobres":

                        DateTime desde = DateTime.Now, hasta = DateTime.Now;

                        if (Session["RemisionDocs_desde"] != null)
                        {
                            desde = Convert.ToDateTime(Session["RemisionDocs_desde"]);
                        }
                        if (Session["RemisionDocs_hasta"] != null)
                        {
                            hasta = Convert.ToDateTime(Session["RemisionDocs_hasta"]);
                        }

                        strReporte = Server.MapPath("~/Report/Sobres.rdlc");
                        rptvPrincipal = ComEx.Helpers.Reportes.RptSobres(rptvPrincipal, clsUsuario, strReporte, listFiltros, desde, hasta);
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
            <asp:HiddenField runat="server" ID="hfReporte" />
            <rsweb:ReportViewer ID="rptvPrincipal" runat="server" AsyncRendering="false" SizeToReportContent="true"></rsweb:ReportViewer>
        </div>
    </form>
    <footer>

        <script>
            var baseUrl = "";
        </script>

        <script src="../bower_components/jquery/dist/jquery.min.js"></script>
        <script src="../Scripts/jquery.signalR-2.2.2.min.js"></script>
        <script src="/signalr/hubs"></script>
        <script src="../bower_components/BlockUI/blockUI.js"></script>
        <script src="../bower_components/alertifyjs/js/alertify.min.js"></script>
        <script src="../Scripts/global.js"></script>
        <script src="../Scripts/pages/compras.formato.js"></script>
    </footer>
</body>
</html>
