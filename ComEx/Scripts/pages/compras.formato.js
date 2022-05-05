$(document).on('ready', function () {
    //Código para signalR
    var ReportesHub = $.connection.Reportes;

    //Conetar con signalR Hub
    $.connection.hub.start().done(function () { });

    //Llamar al método del cliente
    ReportesHub.client.Mostrar = function (Mensaje) {
        BlockUI(Mensaje);
    }

    ReportesHub.client.Ocultar = function () {
        UnblockUI();
    }

    ReportesHub.client.OcultarConMensaje = function (Mensaje) {
        UnblockUI();
        alertify.alert("Reportes", Mensaje).set({ 'closable': false, 'closable': false, transition: 'fade' });
    }

    if ($("#hfReporte").val() === 'RptOrdenCompra' || $("#hfReporte").val() === 'RptDocEquivalente' || $("#hfReporte").val() === 'RptDocEquivalenteNuevo') {
        ExportarOrdenCompraPdf();
    }
});

function ExportarOrdenCompraPdf() {
    var jqoBarraRpt = $('div#rptvPrincipal_ctl09>div:first-child');  // Buscando el div que contiene la barra de herramientas del ReportViewer

    if (jqoBarraRpt && jqoBarraRpt.length > 0    // Verificando que el DIV barra de herramientas fue encontrado,
        && jqoBarraRpt.find('#btnExportarPdf').length <= 0) {    // y verificando que el botón de exportar no existe ya

        // Colocando el botón de exportar, con una estructura similar a la que tiene el botón original en el ReportViewer
        jqoBarraRpt.append(
            '<div class="ToolbarPageNav WidgetSet" style="display:inline-block;padding-top:8px;cursor:pointer">' +
            '<table cellpadding= "0" cellspacing= "0" style= "display:inline;"> ' +
            '<tr>' +
            '<td>' +
            '<div id="btnExportarPdf">' +
            '<table title="Exportar a PDF">' +
            '<tr>' +
            '<td>' +
            '<i class="buttonCrud fa fa-file-pdf-o fa-2x"></i> ' +
            '</td>' +
            '</tr> ' +
            '</table>' +
            '</div>' +
            '</td>' +
            '</tr>' +
            '</table> ' +
            '<div>');

        var btnExportarPdf = $("#btnExportarPdf");
        btnExportarPdf.on("click", btnExportarPdf_Click);

    }

    console.log($("#hfReporte").val());
}

function btnExportarPdf_Click() {

    $('#btnExportarPdf').off('click');

    var url = "";
    var reporte = $("#hfReporte").val();

    if (reporte === 'RptOrdenCompra') {
        url = '../tmCompras/GenerarOrdenCompra';
    }
    else if (reporte === 'RptDocEquivalente') {
        url = '../tmCompras/GenerarDocEquivalente';
    }
    else if (reporte === 'RptDocEquivalenteNuevo') {
        url = '../tmCompras/GenerarDocEquivalenteNuevo';
    }

    $.ajax({
        url: url,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        beforeSend: function () {
            BlockUI("Procesando...");
        },
        complete: function () {
            UnblockUI();
            $('#btnExportarPdf').on("click", btnExportarPdf_Click);
        }
    });
}