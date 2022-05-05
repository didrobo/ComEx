var mensaje = $("#Mensaje");
var principal = $("#Principal");
var informacion = $("#Informacion");
var page_hd = $("#page-hd");
var tablaPrincipal = $("#table-impresionMasiva");
var intDEX = $("#cmbDex");

$(document).on('ready', function () {

    var hdnPageOrigen = $("#hdnPageOrigen").val();
    
    if (hdnPageOrigen == "compras") {
        mostrarTablaArchivos();
    }
    else {
        cargarComboDEX();
        $("#btnProcesarImpresion").on("click", btnProcesar_Click);
    }
});

function cargarComboDEX() {
    $("#cmbDex").select2({
        allowClear: true,
        language: "es",
        ajax: {
            url: ResolveUrl('~/OperacionesMasivas/GetListDEX'),
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    paramSearch: params.term
                };
            },
            processResults: function (data) {
                // parse the results into the format expected by Select2
                // since we are using custom formatting functions we do not need to
                // alter the remote JSON data, except to indicate that infinite
                // scrolling can be used
                //console.log(data);

                return {
                    results: $.map(data.dex, function (item) {
                        return {
                            text: item.varCdAuxliar + ' - ' + dateJsonToString(item.fecAuxliar),
                            id: item.intIdDEX
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 3
    });
}

function btnProcesar_Click(e) {

    //Variables
    var success = true;
    //Validar el año
    if (intDEX.val() === "" || intDEX.val() === " " || intDEX === undefined || intDEX === null || intDEX.val() === null) {
        intDEX.parent().children(".ocultar").fadeIn();
        intDEX.parent().addClass("has-error");
        success = false;
    }

    //Si no hay algún error agregamos los datos y ejecutamos el ajax
    if (success) {
        mostrarTablaArchivos();        
    }
}

function mostrarTablaArchivos() {
    principal.fadeOut(1000);
    page_hd.fadeOut();
    //Mostrar la información
    informacion.fadeIn(1000);

    var dex = intDEX.val();

    //Se crea la tabla
    configExportServer('GetListDocumentosImpresion', tablaPrincipal, [dex]);

    tablaPrincipal = $(tablaPrincipal).DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        serverSide: true,
        ajax: {
            type: "POST",
            url: ResolveUrl('~/OperacionesMasivas/GetListDocumentosImpresion'),
            contentType: 'application/json; charset=utf-8',
            data: function (data) {
                data.listFiltros = listFiltrosDataTable;
                data.parametros = [dex];

                return data = JSON.stringify(data);
            },
            complete: function (data) {
                //console.log(data);
                if (data.responseJSON.recordsFiltered === "0") {
                    disableControlsProcess()
                }
            }
        },
        processing: true,
        lengthMenu: [[50, 200, 500], [50, 200, 500]],
        colReorder: true,
        scrollX: true,
        scrollY: 420,
        responsive: true,
        scrollCollapse: true,
        scroller: {
            loadingIndicator: true
        },
        pageLength: 50,
        paging: true,
        autoWidth: true,
        deferRender: true,
        columns: [
            { title: "intId", data: "intId", visible: false },
            { title: "intIdDEX", data: "intIdDEX", visible: false },
            { title: "intIdCP", data: "intIdCP", visible: false },
            { title: "Documento", data: "varNumDoc" },
            { title: "Tipo Doc.", data: "varTipoDocumento" },
            {
                title: "Archivo", data: "varRtaArchvoAdjnto",
                render: function (ruta) {
                    return '<a href="' + ruta + '" target="_blank">Ver PDF</a>';
                }
            },
            { title: "Existe", data: "varExiste" },
            { title: "Régimen", data: "varNbreRgmen" }
        ],
        buttons: [
            {
                text: '<i class="buttonCrud fa fa-filter"></i>',
                titleAttr: 'Filtrar',
                action: function (e, dt, node, config) {
                    filtrarDataTable(dt);
                }
            },
            {
                extend: "excelHtml5",
                text: '<i class="buttonCrud fa fa-file-excel-o"></i>',
                titleAttr: 'Exportar Excel',
                exportOptions: {
                    modifier: {
                        search: 'applied',
                        order: 'applied'
                    }
                }
            },
            {
                text: '<i id="icn_print" class="buttonCrud fa fa-print"></i>',
                titleAttr: 'Imprimir Documentos',
                action: function (e, dt, node, config) {
                    imprimirDocs();
                }
            }
        ]
    });
}

function imprimirDocs() {

    var dex = intDEX.val();

    var data = { listFiltros: listFiltrosDataTable, parametros: [dex] }

    $.ajax({
        type: 'POST',
        url: ResolveUrl("~/OperacionesMasivas/ImprimirDocumentos"),
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        beforeSend: function () {

            disableControlsProcess();

            mensaje.html('<br /><div class="alert alert-info text-center"><i class="fa fa-cog fa-spin fa-3x fa-fw margin-bottom"></i> Imprimiendo documentos, por favor espere.</div>').fadeIn(100);
        },
        success: function (resultado) {
            if (resultado.success) {
                //console.log(resultado.fileToPrint);
                window.open(resultado.fileToPrint, '_blank');
                mensaje.html('<br /><div class="alert alert-success alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + resultado.Message + ' <a href="' + resultado.fileToPrint + '" target="_blank">Abrir</a></div>');
            }
            else {
                mensaje.html('<div class="alert alert-danger alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + resultado.Message + '</div>')
                    .fadeIn(1000);
            }
        },
        error: function (data, success, error) {
            mensaje.html('<div class="alert alert-danger alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + data.responseJSON.Message + '</div>')
                .fadeIn(1000);
        }
    });
}

function disableControlsProcess() {
    $("#icn_print").addClass("disabled_control");
    $("#icn_print").attr('disabled', 'disabled');
    //tablaPrincipal.buttons().disable();
    tablaPrincipal.button(2).enable(false);
}