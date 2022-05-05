var mensaje = $("#Mensaje");
var principal = $("#Principal");
var informacion = $("#Informacion");
var page_hd = $("#page-hd");
var tablaPrincipal = $("#tablaResultados");
var planCI = $("#ddlPlanCI");

$(document).on("ready", (function () {
    $("#btnProcesar").on("click", btnProcesar_Click);
}));

function btnProcesar_Click(e) {

    //Variables
    var success = true;
    var formData = new FormData();

    //Validar que plan CI contega datos.
    if (planCI.val() === "" || planCI.val() === " " || planCI === undefined || planCI === null) {
        planCI.parent().children(".ocultar").fadeIn();
        planCI.parent().addClass("has-error");
        success = false;
    }

    //Si no hay algún error agregamos los datos y ejecutamos el ajax
    if (success) {

        //Agregar los datos en el formData
        formData.append("intIdPlanCI", planCI.val());
        formData.append("varDscrpcionPlanCI", $("#ddlPlanCI option:selected").text());

        $.ajax({
            url: ResolveUrl("~/DexPdf/ProcesarArchivos"),
            type: "POST",
            contentType: false,
            cache: false,
            dataType: "json",
            processData: false,
            data: formData,
            beforeSend: function () {
                principal.fadeOut(1000);
                mensaje.html('<div class="alert alert-info text-center"><i class="fa fa-cog fa-spin fa-3x fa-fw margin-bottom"></i> Procesando archivos, por favor espere.</div>')
            },
            success: function (resultado) {
                if (resultado.success) {
                    mensaje.html('<div class="alert alert-info" role="alert">' + resultado.Message + '</div>')
                        .fadeIn(1000).slideToggle(1000);
                    ////console.log(resultado);
                    $("#PlanCI").html(ValorTd(resultado.PlanCI));
                    $("#Errores").html(ValorTd(resultado.intErrores));
                    $("#Buenos").html(ValorTd(resultado.intBuenos));
                    $("#TotalRegistro").html(ValorTd(resultado.intTotalRegistros));

                    //Llamar función para cargar los datos
                    TablaDatos();

                    page_hd.fadeOut();
                    //Mostrar la información
                    informacion.fadeIn(1000);
                }
                else {
                    mensaje.html('<div class="alert alert-danger alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + resultado.Message + '</div>')
                        .fadeIn(1000);

                    principal.fadeIn(1000);
                }
            },
            error: function (jqXHR) {
                mensaje.html('<div class="alert alert-danger alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + jqXHR.statusText + '</div>')
                    .fadeIn(1000);
            }
        });
    }
}

function TablaDatos() {
    tablaPrincipal = tablaPrincipal.DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-12'i>>",
        ajax: {
            type: "POST",
            url: ResolveUrl("~/DexPdf/GetTempDEX"),
            datatype: "json",
            complete: function () {
                var registrosBuenos = $("#Buenos").find("span").html();
                if (registrosBuenos === "0") {
                    disableControlsProcess()
                }
            }
        },
        processing: true,
        colReorder: true,
        scrollX: true,
        scrollY: 420,
        responsive: true,
        scrollCollapse: true,
        scroller: {
            loadingIndicator: true
        },
        paging: false,
        autoWidth: true,
        deferRender: true,
        columns: [
            { data: 'error' },
            { data: 'Formulario' },
            {
                data: 'C997_fechaAceptacion',
                render: dateJsonToString
            },
            { data: 'CpsEnBorrador', className: 'text-right' },
            { data: 'CpsEnPdf', className: 'text-right' }
        ],
        order: [],
        buttons: [
            {
                text: '<i id="icn_process" class="buttonCrud btn_table_verde fa fa-check-circle fa-2x"></i>',
                titleAttr: 'Procesar',
                action: function (e, dt, node, config) {
                    subirDEX();
                }
            }
        ]
    });
}

function subirDEX() {

    $.ajax({
        type: 'POST',
        url: ResolveUrl("~/DexPdf/SubirDEX"),
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        beforeSend: function () {

            disableControlsProcess();

            mensaje.html('<br /><div class="alert alert-info text-center"><i class="fa fa-cog fa-spin fa-3x fa-fw margin-bottom"></i> Cargando DEX, por favor espere.</div>').fadeIn(100);
        },
        success: function (resultado) {
            if (resultado.success) {
                mensaje.html('<br /><div class="alert alert-success alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + resultado.Message + '</div>');
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

function ValorTd(Cantidad) {
    return '<span class="label label-primary">' + Cantidad + '</span>';
}

function disableControlsProcess() {
    $("#icn_process").addClass("disabled_control");
    $("#tablaResultados input").attr('disabled', 'disabled');
    tablaPrincipal.buttons().disable();
}