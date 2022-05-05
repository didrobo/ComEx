var mensaje = $("#Mensaje");
var principal = $("#Principal");
var informacion = $("#Informacion");
var page_hd = $("#page-hd");
var tablaPrincipal = $("#table-remisionDocs");
var fecDesde = $("#fecDesde");
var fecHasta = $("#fecHasta");

$(document).on('ready', function () {

    $("#btnProcesar").on("click", btnProcesar_Click);

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });
});

$(function () {
    $('#fecDesde').datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });
    $('#fecHasta').datetimepicker({
        useCurrent: false, //Important! See issue #1075
        locale: "es",
        format: "DD/MM/YYYY"
    });
    $("#fecDesde").on("dp.change", function (e) {
        $('#fecHasta').data("DateTimePicker").minDate(e.date).locale("es").format("DD/MM/YYYY");
    });
    $("#fecHasta").on("dp.change", function (e) {
        $('#fecDesde').data("DateTimePicker").maxDate(e.date).locale("es").format("DD/MM/YYYY");
    });
});

function btnProcesar_Click(e) {

    //Variables
    var success = true;

    //Validar la fecha desde
    if (fecDesde.val() === "" || fecDesde.val() === " " || fecDesde === undefined || fecDesde === null) {
        fecDesde.parent().children(".ocultar").fadeIn();
        fecDesde.parent().addClass("has-error");
        success = false;
    }

    //Validar la fecha hasta
    if (fecHasta.val() === "" || fecHasta.val() === " " || fecHasta === undefined || fecHasta === null) {
        fecHasta.parent().children(".ocultar").fadeIn();
        fecHasta.parent().addClass("has-error");
        success = false;
    }

    //Si no hay algún error agregamos los datos y ejecutamos el ajax
    if (success) {
        principal.fadeOut(1000);
        page_hd.fadeOut();
        //Mostrar la información
        informacion.fadeIn(1000);

        var desde = fecDesde.val();
        var hasta = fecHasta.val();

        //Se crea la tabla
        configExportServer('GetRemisionDocs', tablaPrincipal, [desde, hasta]);

        tablaRemisionDocs = $(tablaPrincipal).DataTable({
            dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
            serverSide: true,
            ajax: {
                type: "POST",
                url: 'GetRemisionDocs',
                contentType: 'application/json; charset=utf-8',
                data: function (data) {
                    data.listFiltros = listFiltrosDataTable;
                    data.parametros = [desde, hasta];

                    return data = JSON.stringify(data);
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
                { title: "intIdRmsion", data: "intIdRmsion", visible: false },
                { title: "intIdPrvdor", data: "intIdPrvdor", visible: false },
                { title: "Documento", data: "varCdPrvdor" },
                { title: "Proveedor", data: "varNmbre" },
                { title: "Mes", data: "intMes" },
                { title: "Fec. Envío", data: "fecEnvio", render: dateJsonToString },
                { title: "Guía Envío", data: "varNumGuiaEnvio" },
                { title: "Ciudad", data: "varDscrpcionCiudad" },
                { title: "F. Recibido", data: "fecRcbdo", render: dateJsonToString },
                { title: "F. Entrega Cli.", data: "fecEntrgaCliente", render: dateJsonToString },                
                { title: "Quien Recibe", data: "varQuienRcbe" },
                { title: "F. Despacho", data: "fecDspcho", render: dateJsonToString },
                { title: "Guía Despacho", data: "varNumGuiaDspacho" }
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
                    text: '<i id="icn_actualizarDatosEnvio" class="buttonCrud fa fa-chevron-circle-right"></i>',
                    titleAttr: 'Actualizar Datos Envío',
                    action: function (e, dt, node, config) {
                        actualizarDatosEnvio();
                    }
                },
                {
                    text: '<i id="icn_generarXml" class="buttonCrud fa fa-file-code-o"></i>',
                    titleAttr: 'Generar XML',
                    action: function (e, dt, node, config) {
                        generarXml();
                    }
                },
                {
                    text: '<i id="icn_formatoSobres" class="buttonCrud fa fa-file-pdf-o"></i>',
                    titleAttr: 'Formato de Sobres',
                    action: function (e, dt, node, config) {
                        GenerarFormatoSobres();
                    }
                }
            ],
            order: [[0, 'desc']]
        });
    }
}

//Función para actualizar la fecha y guía de envío
function actualizarDatosEnvio() {

    // Mostrar Modal
    showBSModal({
        title: "Actualizar Datos Envío",
        body: '<div class="row"><div class="col-md-12"><div class="form-group"><label>Guía Envío</label><input id="varNumGuiaEnvio" class="form-control" type="text" /><span class="field-validation-valid text-danger" id="val_message"></span></div><div class="form-group"><label>Fecha Envío.</label><input id="fecEnvio" class="form-control calendario" type="datetime" /><span class="field-validation-valid text-danger" id="val_message_fecEnvio"></span></div></div></div>',
        size: "small",
        actions: [
            {
                label: 'Actualizar',
                cssClass: 'btn-success btn_actualizar_datos',
                onClick: function (e) {
                    ejecutarActualizarDatosEnvio();
                }
            },
            {
                label: 'Cerrar',
                cssClass: 'btn-danger',
                onClick: function (e) {
                    $(e.target).parents('.modal').modal('hide');
                }
            }
        ],
        onShow: function (e) {

            $("#fecEnvio").datetimepicker({
                locale: "es",
                format: "DD/MM/YYYY"
            });

        }
    });
}

//Función que ejecuta la operación de actualizar datos de envío
function ejecutarActualizarDatosEnvio() {

    var varNumGuiaEnvio = $("#varNumGuiaEnvio").val();
    var fecEnvio = $("#fecEnvio").val();

    if (varNumGuiaEnvio !== "" && varNumGuiaEnvio !== null) {
        $("#val_message").empty();

        if (fecEnvio !== "" && fecEnvio !== null) {

            $("#val_message_fecEnvio").empty();
            var desde = fecDesde.val();
            var hasta = fecHasta.val();

            var parameter = { "listFiltros": listFiltrosDataTable, "desde": desde, "hasta": hasta, "varNumGuiaEnvio": varNumGuiaEnvio, "fecEnvio": fecEnvio };
            $.ajax({
                type: 'POST',
                url: ResolveUrl("~/tmRemisionDocumentos/ActualizarDatosEnvio"),
                data: JSON.stringify(parameter),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                cache: false,
                beforeSend: function () {
                    console.info("Enviando infromación...");
                    $(".btn_actualizar_datos").button('loading');
                },
                success: function (resultado) {
                    if (resultado.success) {
                        tablaRemisionDocs.draw();
                        $("#varNumGuiaEnvio").parents('.modal').modal('hide');

                        pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                            + resultado.mensaje + '</div>')
                            .fadeIn(100);

                        //Limpiar el botón
                        $(".btn_actualizar_datos").button('reset');
                    }
                    else {
                        $("#varNumGuiaEnvio").parents('.modal').modal('hide');
                        pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                            .fadeIn(100);

                        //Limpiar el botón
                        $(".btn_actualizar_datos").button('reset');
                    }
                },
                error: function (data, success, error) {
                    $("#varNumGuiaEnvio").parents('.modal').modal('hide');
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                        .fadeIn(100);

                    //Limpiar el botón
                    $(".btn_actualizar_datos").button('reset');
                }
            });
        }
        else {
            $("#val_message_fecEnvio").empty().append("Seleccione la fecha de envío");
        }
    }
    else {
        $("#val_message").empty().append("Ingrese el número de guía");
    }
}

function generarXml() {

    var desde = fecDesde.val();
    var hasta = fecHasta.val();

    var parameter = { "listFiltros": listFiltrosDataTable, "desde": desde, "hasta": hasta };

    $.ajax({
        url: ResolveUrl('~/tmRemisionDocumentos/CrearXml'),
        type: "POST",
        data: JSON.stringify(parameter),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        beforeSend: function () {
            BlockUI("Creando XML...");
        },
        success: function (resultado) {
            if (resultado.success) {

                var link = document.createElement('a');
                link.setAttribute('href', ResolveUrl(resultado.urlArchivo));
                link.setAttribute('target', "_blank");
                link.setAttribute('download', "");
                link.click();

                pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                    + resultado.mensaje + '</div>')
                    .fadeIn(100);
            }
            else {
                pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                    .fadeIn(100);
            }
            //UnblockUI();                    
        },
        error: function (data, success, error) {
            pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                .fadeIn(100);
        },
        complete: function () {
            UnblockUI();
        }
    });
}

function GenerarFormatoSobres() {
    $.ajax({
        url: ResolveUrl('~/OperacionesMasivas/SetSessionFilter'),
        type: "POST",
        data: JSON.stringify({ listFiltros: listFiltrosDataTable }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var newWindow = window.open('FormatoSobres', '_blank');
            }
        }
    });
}