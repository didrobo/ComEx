//Variables
var lote = $("#txtLote");
var file = $("#file");
var regalia = $('#cmbRegalias');
var mensaje = $("#Mensaje");
var principal = $("#Principal");
var tablaPrincipal = $("#tablaResultados");
var tablaSpInsertarTmpCompras = $("#table-resultado-spInsertarTmpCompras");
var informacion = $("#Informacion");
var page_hd = $("#page-hd");
var last_validate_fac = "";

$(document).on("ready", (function () {
    $("#file").fileinput({
        uploadUrl: "",
        uploadAsync: true,
        showUpload: false,
        showRemove: false,
        showPreview: false,
        language: "es",
        mainClass: "input-group-lg",
        maxFileSize: 10240,
        allowedFileExtensions: ['xlsx', 'xls']
    });

    $("#btnProcesar").on("click", btnProcesar_Click);
    lote.on("keydown", lote_KeyDown);
    file.on("change", file_Change);
    regalia.on("change", regalia_Change);
}));

function btnProcesar_Click(e)
{
    //Variables
    var success = true;
    var formData = new FormData();

    //Validaciones
    //Validar que el archivo contega algún dato.
    if (file.val() === "" || file.val() === " " || file === undefined || file === null || file.val().length < 2) {
        var parent = file.parent().parent().parent().parent().parent();
        parent.children(".ocultar").fadeIn();
        parent.children().addClass("has-error");
        success = false;
    }

    //Validar que lote contega datos.
    if(lote.val() === "" || lote.val() === " " || lote === undefined || lote === null || lote.val().length < 2)
    {
        lote.parent().children(".ocultar").fadeIn();
        lote.parent().addClass("has-error");
        success = false;
    }

    //Validar que se seleccione por los menos una regalia
    if (regalia.val() === "" || regalia.val() === " " || regalia === undefined || regalia === null)
    {
        regalia.parent().parent().children(".ocultar").fadeIn();
        regalia.parent().parent().addClass("has-error");
        success = false;
    }

    //Si no hay algún error agregamos los datos y ejecutamos el ajax
    if (success) {
        //Agregar los datos en el formData
        formData.append("Archivo", file[0].files[0]);
        formData.append("Lote", lote.val().toUpperCase());
        formData.append("Regalia", regalia.val());

        //Ajax
        $.ajax({
            url: ResolveUrl("~/Archivos/ProcesarArchivo"),
            type: "POST",
            contentType: false,
            cache: false,
            dataType: "json",
            processData: false,
            data: formData,
            beforeSend: function () {
                principal.fadeOut(1000);
                mensaje.html('<div class="alert alert-info text-center"><i class="fa fa-cog fa-spin fa-3x fa-fw margin-bottom"></i> Procesando archivo, por favor espera.</div>')
            },
            success: function (resultado) {
                if (resultado.success)
                {
                    mensaje.html('<div class="alert alert-info" role="alert">' + resultado.Message + '</div>')
                        .fadeIn(1000).slideToggle(1000);
                    //console.log(resultado);
                    $("#Lote").html(ValorTd(resultado.strLote));
                    $("#Regalia").html(ValorTd(resultado.regalia.varCdLnea + "-" + resultado.regalia.intAno + "/" + resultado.regalia.intMes));
                    $("#Errores").html(ValorTd(resultado.intErrores));
                    $("#Errores").html(ValorTd(resultado.intErrores));
                    $("#Buenos").html(ValorTd(resultado.intBuenos));
                    $("#TotalRegistro").html(ValorTd(resultado.intTotalRegistros));
                    $("#FactorOnza").html(ValorTd(resultado.dcFactorOnza));
                    $("#Retencion").html(ValorTd(resultado.intRetencion));
                    $("#BaseRegalia").html(ValorTd(resultado.dcBseRglias.formatNumber(2, '.', ',')));
                    $("#BaseRegaliaAG").html(ValorTd(resultado.dcBseRgliasAG.formatNumber(2, '.', ',')));
                    $("#BaseRegaliaPT").html(ValorTd(resultado.dcBseRgliasPT.formatNumber(2, '.', ',')));
                    $("#Factor").html(ValorTd(resultado.dcFactor.formatNumber(2, '.', ',')));
                    $("#TotalBrutos").html(ValorTd(resultado.dcTotalBrutos.formatNumber(2, '.', ',')));
                    $("#TotalFinos").html(ValorTd(resultado.dcTotalFinos.formatNumber(2, '.', ',')));
                    $("#TotalFinosAG").html(ValorTd(resultado.dcTotalFinosAG.formatNumber(2, '.', ',')));
                    $("#TotalFinosPT").html(ValorTd(resultado.dcTotalFinosPT.formatNumber(2, '.', ',')));
                    $("#TotalRegalias").html(ValorTd(resultado.dcTotalRegalias.formatNumber(2, '.', ',')));
                    
                    //Llamar función para cargar los datos
                    TablaDatos();
                    
                    page_hd.fadeOut();
                    //Mostrar la información
                    informacion.fadeIn(1000);                    
                }
                else
                {
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

function lote_KeyDown() {
    if(lote.val().length > 0)
    {
        lote.parent().children(".ocultar").fadeOut();
        lote.parent().removeClass("has-error");
    }
}

function file_Change(e) {
    if (file.val().length > 2) {
        var parent = file.parent().parent().parent().parent().parent();
        parent.children(".ocultar").fadeOut();
        parent.children().removeClass("has-error");
    }
}

function regalia_Change() {
    if(regalia.val() !== "" || regalia.val() !== " " || regalia !== undefined || regalia !== null)
    {
        regalia.parent().parent().children(".ocultar").fadeOut();
        regalia.parent().parent().removeClass("has-error");
    }
}

function TablaDatos()
{
    tablaPrincipal = tablaPrincipal.DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-12'i>>",
        ajax: {
            type: "POST",
            url: ResolveUrl("~/Archivos/GetTempCompras"),
            datatype: "json",
            complete: function () {
                var registrosBuenos = $("#Buenos").find("span").html();
                if (registrosBuenos === 0) {
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
            //{ data: 'intIdtmpCompras' },
            //{ data: 'intIdIntrfceErrores' },
            { data: 'varNmbreError' },
            {
                data: 'prefijo',
                render: function (data, type, full)
                {
                    if (full.varNbreRgmen === 'SIMPLIFICADO')
                    {
                        return (full.prefijo === null ? "" : full.prefijo);
                    }
                    else
                    {
                        if (full.intIdIntrfceErrores === 0) {
                            return '<input class="mayuscula" id="prefijo_' + full.intIdtmpCompras + '" style="width: 70px;" type="text" value="" />';
                        } else {
                            return '<input style="width: 70px;" type="text" value="" disabled />';
                        }
                    }
                }
            },
            {
                data: 'factura',
                render: function (data, type, full) {
                    if (full.varNbreRgmen === 'SIMPLIFICADO') {
                        return (full.factura === null ? "" : full.factura);
                    }
                    else {
                        if (full.intIdIntrfceErrores === 0) {
                            return '<div class="form-group"><input class="input_fact" id="fact_' + full.intIdtmpCompras + '" onblur="OnBlurEvent(' + full.intIdtmpCompras + ')" style="width: 70px;" type="number" min="0" value="" /><i id="icon_' + full.intIdtmpCompras + '" title="Digite el número de la factura" style="padding-left: 5px;" class="del fa fa-exclamation-circle" aria-hidden="true"></i></div>';
                        } else {
                            return '<input style="width: 70px;" type="text" value="" disabled />';
                        }
                    }
                }
            },
            {
                data: 'fecha',
                render: dateJsonToString
            },
            { data: 'proyecto' },
            { data: 'llave' },
            { data: 'varDscrpcionInsmo' },
            { data: 'total', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'gramosBrutos', render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { data: 'valorXGramo', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'numLey' },
            { data: 'numLeyAG' },
            { data: 'numLeyPT' },
            { data: 'valor', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            //{ data: 'intIdPrvdor' },
            { data: 'varNmbre' },
            {
                data: 'bitAcgdosLey1429',
                render: function (ley)
                {
                    if (ley)
                    {
                        return "Si";
                    }
                    else
                    {
                        return "No";
                    }
                }
            },
            { data: 'varNbreRgmen' },
            //{ data: 'intIdCiudadRglias' },
            { data: 'varDscrpcionCiudad' },
            { data: 'numPorcMnria', render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { data: 'numPorcMnriaAG', render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { data: 'numPorcMnriaPT', render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { data: 'finos', render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { data: 'onzasFinas', render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { data: 'finosAG', render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { data: 'finosPT', render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { data: 'precioNetoSinRetencion', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'precioNetoConRetencion', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'totalFacturaSinRetencion', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'totalFacturaConRetencion', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'retencion', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'regalias', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'regaliasAG', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'regaliasPT', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'totalRegalias', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { data: 'totalMaterialMenosRegalias', render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            //{ data: 'intIdInsmo' }
        ],
        order: [],
        buttons: [
            {
                text: '<i id="icn_process" class="buttonCrud btn_table_verde fa fa-check-circle fa-2x"></i>',
                titleAttr: 'Procesar',
                action: function (e, dt, node, config) {
                    subirOrdenesCompra();
                }
            }
        ]
    });    
}

function ValorTd(Cantidad)
{
    return '<span class="label label-primary">' + Cantidad + '</span>';
}

function OnBlurEvent(intIdtmpCompras) {

    var prefijo = $("#prefijo_" + intIdtmpCompras).val().toUpperCase();
    var fact = $("#fact_" + intIdtmpCompras).val();

    if (fact !== "") {

        if (last_validate_fac !== (intIdtmpCompras + prefijo + fact)) {
            var parameter = { "intIdtmpCompras": intIdtmpCompras, "factura": fact, "prefijo": prefijo }

            $("#fact_" + intIdtmpCompras).addClass("loading_input");

            $.ajax({
                type: 'POST',
                url: ResolveUrl("~/Archivos/ValidarNumFactura"),
                data: JSON.stringify(parameter),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                cache: false,
                success: function (resultado) {
                    if (resultado.success) {
                        //console.log(resultado);
                        //del fa fa-exclamation-circle

                        last_validate_fac = intIdtmpCompras + prefijo + fact;

                        if (resultado.bitValid) {
                            $("#icon_" + intIdtmpCompras).removeClass("del");
                            $("#icon_" + intIdtmpCompras).removeClass("fa-exclamation-circle");

                            $("#icon_" + intIdtmpCompras).addClass("btn_table_verde");
                            $("#icon_" + intIdtmpCompras).addClass("fa-check");

                            $("#icon_" + intIdtmpCompras).attr("title", "");
                        }
                        else {
                            $("#icon_" + intIdtmpCompras).removeClass("btn_table_verde");
                            $("#icon_" + intIdtmpCompras).removeClass("fa-check");

                            $("#icon_" + intIdtmpCompras).addClass("del");
                            $("#icon_" + intIdtmpCompras).addClass("fa-exclamation-circle");

                            $("#fact_" + intIdtmpCompras).focus();
                            $("#icon_" + intIdtmpCompras).attr("title", resultado.mensaje);
                        }
                    }
                    else {
                        mensaje.html('<div class="alert alert-danger alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + resultado.Message + '</div>')
                            .fadeIn(1000);
                    }

                    $("#fact_" + intIdtmpCompras).removeClass("loading_input");
                },
                error: function (data, success, error) {
                    $("#fact_" + intIdtmpCompras).removeClass("loading_input");
                    mensaje.html('<div class="alert alert-danger alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + data.responseJSON.Message + '</div>')
                                .fadeIn(1000);
                }
            });
            
        } else {
            if ($("#icon_" + intIdtmpCompras).attr("title") !== "") {
                $("#fact_" + intIdtmpCompras).focus();
            }
        }
    } else {
        $("#icon_" + intIdtmpCompras).removeClass("btn_table_verde");
        $("#icon_" + intIdtmpCompras).removeClass("fa-check");

        $("#icon_" + intIdtmpCompras).addClass("del");
        $("#icon_" + intIdtmpCompras).addClass("fa-exclamation-circle");

        $("#icon_" + intIdtmpCompras).attr("title", "Digite el número de la factura");
    }
}

function subirOrdenesCompra() {
    
    $.ajax({
        type: 'POST',
        url: ResolveUrl("~/Archivos/SubirOrdenesCompra"),
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        beforeSend: function () {

            disableControlsProcess();

            mensaje.html('<br /><div class="alert alert-info text-center"><i class="fa fa-cog fa-spin fa-3x fa-fw margin-bottom"></i> Cargando ordenes de compra, por favor espera.</div>').fadeIn(100);
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

function disableControlsProcess() {
    $("#icn_process").addClass("disabled_control");
    $("#tablaResultados input").attr('disabled', 'disabled');
    tablaPrincipal.buttons().disable();
}