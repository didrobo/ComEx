$(document).on('ready', function () {

    //Código para signalR
    var ReportesHub = $.connection.Reportes;

    //Conetar con signalR Hub
    $.connection.hub.start().done(function () { });

    //Llamar al método del cliente
    ReportesHub.client.Mostrar = function (Mensaje) {
        BlockUI(Mensaje);
    };

    ReportesHub.client.Ocultar = function () {
        UnblockUI();
    };

    ReportesHub.client.OcultarConMensaje = function (Mensaje) {
        UnblockUI();
        alertify.alert("CPs Manuales", Mensaje).set({ 'closable': false, transition: 'fade' });
    };

    crearTabla();
});

function crearTabla() {

    configExportServer('GetCps', '#table-cps');

    tablaCPs = $('#table-cps').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        serverSide: true,
        ajax: {
            type: "POST",
            url: 'GetCps',
            contentType: 'application/json; charset=utf-8',
            data: function (data) {
                //data.columns[0].search.operator = "=";                
                data.listFiltros = listFiltrosDataTable;
                //console.log(data);

                return data = JSON.stringify(data);
            }
        },
        processing: true,
        lengthMenu: [[50, 200, 500], [50, 200, 500]],
        scrollX: true,
        scrollY: 420,
        colReorder: true,
        responsive: true,
        scrollCollapse: true,
        scroller: {
            loadingIndicator: true
        },
        pageLength: 50,
        paging: true,
        autoWidth: true,
        deferRender: true,
        createdRow: function (row, data, dataIndex) {
            if (data['bitAnldo']) {
                $(row).addClass('anuladoClass');
            }
        },
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            {
                "class": "details-control fa-plus-circle",
                "orderable": false,
                "data": null,
                "defaultContent": ""
            },
            { title: "intIdCP", data: "intIdCP", visible: false },
            { title: "intIdPrvdor", data: "intIdPrvdor", visible: false },
            { title: "Lote", data: "varNumLte" },
            { title: "Cargue", data: "intCnsctvoCrgue" },
            { title: "Proyecto", data: "varPrycto" },
            { title: "Llave", data: "varLlve" },
            { title: "Cd. CP", data: "varCdCP" },
            { title: "Fec. CP", data: "fecCP", render: dateJsonToString },
            { title: "Nmro. CP", data: "varNmroCP" },
            { title: "F. Aprb. CP", data: "fecAprbcionCP", render: dateJsonToString },
            { title: "Proveedor", data: "varNmbre" },
            { title: "intIdPlanCI", data: "intIdPlanCI", visible: false },
            { title: "Peso Bruto", data: "numBrtos", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Finos", data: "numFnos", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Cd. Plan CI", data: "varCdPlanCI" },
            { title: "Plan CI", data: "varDscrpcionPlanCI" },
            { title: "intIdCPMdfccion", data: "intIdCPMdfccion", visible: false },
            {
                title: "Anulado", data: 'bitAnldo',
                render: function (anulado)
                {
                    if (anulado)
                    {
                        return "Si";
                    }
                    else
                    {
                        return "No";
                    }
                }
            },
            { title: "CP Mdfccion", data: "varCPMdfccion" },
            { title: "F. Mdfccion", data: "fecMdfccion", render: dateJsonToString },
            { title: "Motivo Mdfccion", data: "varMtvoMdfccion" },
            {
                title: "Archivo", data: "varRtaArchvoAdjnto",
                render: function (ruta) {
                    if (ruta === null) {
                        return '';
                    }
                    else {
                        return '<a href="' + ruta + '" target="_blank">Ver PDF</a>';
                    }
                }
            },
            { title: "CP Manual", data: "varNmroCPManual" },
            { title: "Fec. CP Manual", data: "fecCPManual", render: dateJsonToString }
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
                text: '<i id="icn_anularCP" class="buttonCrud del fa fa-minus-circle disabled_control"></i>',
                titleAttr: 'Anular CP',
                enabled: false,
                action: function (e, dt, node, config) {
                    anularCP();
                }
            },
            {
                text: '<i id="icn_modificarCP" class="buttonCrud fa fa-external-link disabled_control"></i>',
                titleAttr: 'Crear Modificación',
                enabled: false,
                action: function (e, dt, node, config) {
                    modificarCP();
                }
            },
            {
                text: '<i id="icn_generarXmlCP" class="buttonCrud fa fa-file-code-o"></i>',
                titleAttr: 'Generar XML CPs',
                action: function (e, dt, node, config) {
                    generarXmlCP();
                }
            },
            {
                text: '<i id="icn_cpsManuales" class="buttonCrud fa fa-file-pdf-o"></i>',
                titleAttr: 'Generar CPs Manuales',
                action: function (e, dt, node, config) {

                    alertify.confirm("Generar CPs Manuales", "Se genererán los CP manuales para los CP filtrados.<br /> ¿Desea continuar?", function () {
                        GenerarCPsManulaes();
                    }, null);                    
                }
            }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: [[2, 'desc']],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();
            var json = api.ajax.json();

            $(api.column(14).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numBrtos));
            $(api.column(15).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numFnos));

        }
    });

    // Array to track the ids of the details displayed rows
    var detailRows = [];

    // Este es evento es para mostrar el detalle de la fila usando el botoón (+)
    $('#table-cps tbody').on('click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tablaCPs.row(tr);
        var idx = $.inArray(tr.attr('id'), detailRows);

        if (row.child.isShown()) {
            tr.removeClass('details');
            row.child.hide();

            // Remove from the 'open' array
            detailRows.splice(idx, 1);

            $(this).removeClass('fa-minus-circle');
            $(this).addClass('fa-plus-circle');
        }
        else {
            tr.addClass('details');
            row.child(getTblDetalle(row.data())).show();

            drawTblDetalle(row.data());

            // Add to the 'open' array
            if (idx === -1) {
                detailRows.push(tr.attr('id'));
            }

            $(this).removeClass('fa-plus-circle');
            $(this).addClass('fa-minus-circle');
        }
    });

    // On each draw, loop over the `detailRows` array and show any child rows
    tablaCPs.on('draw', function () {
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger('click');
        });
    });

    tablaCPs.on('select', function (e, dt, type, indexes) {
        var selectedRows = tablaCPs.rows({ selected: true }).count();
        var bitAnldo = tablaCPs.rows({ selected: true }).data()[0].bitAnldo;

        if (!bitAnldo) {
            $("#icn_anularCP").removeClass("disabled_control");
            $("#icn_modificarCP").removeClass("disabled_control");

            tablaCPs.button(2).enable(selectedRows > 0);
            tablaCPs.button(3).enable(selectedRows > 0);
        }               
    });

    tablaCPs.on('deselect', function (e, dt, type, indexes) {
        var selectedRows = tablaCPs.rows({ selected: true }).count();

        $("#icn_anularCP").addClass("disabled_control");
        $("#icn_modificarCP").addClass("disabled_control");
        tablaCPs.button(2).enable(selectedRows > 0);
        tablaCPs.button(3).enable(selectedRows > 0);
    });

    $('#table-cps_filter input').unbind();
    $('#table-cps_filter input').bind('keyup', function (e) {
        if (e.keyCode === 13) {
            tablaCPs.search(this.value).draw();
        }
    });
}

function getTblDetalle(d) {
    // la variable d contiene un objeto tmCompras
    return '<table id="table-detalle-cps-' + d.intIdCP + '" class="table table-striped table-bordered table-hover display compact" style="border-spacing: 0px; border-collapse: separate; width: 20%;" />';
}

function drawTblDetalle(d) {
    var tablaCPDet = $('#table-detalle-cps-' + d.intIdCP).DataTable({
        serverSide: true,
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmCPDetalle/GetCPsDetalle/' + d.intIdCP),
            contentType: 'application/json; charset=utf-8',
            data: function (data) { return data = JSON.stringify(data); }
        },
        processing: true,
        responsive: true,
        paging: false,
        autoWidth: true,
        searching: false,
        ordering: false,
        colReorder: true,
        info: false,
        columns: [
            {
                defaultContent: '<i class="buttonCrud fa fa-undo btnDevolucion" style="cursor: pointer;" title="Devoluciones"></i>',
                data: null
            },
            { title: "intIdCPDtlle", data: "intIdCPDtlle", visible: false },
            { title: "intIdCp", data: "intIdCp", visible: false },
            { title: "CI", data: "varCI" },
            { title: "Cnt. CP", data: "numCntdadCp", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
            { title: "Cnt. Consumida", data: "numCntdadCnsmda", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
            { title: "Vr. CP", data: "numVlorCpPorCI", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Cnt. Anulada", data: "numCntdadCpAnlda", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
            { title: "Vr. Anulado", data: "numVlorCpPorCiAnlda", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' }
        ]
    });

    $('#table-detalle-cps-' + d.intIdCP + ' tbody').on('click', '.btnDevolucion', function (e) {
        var data = tablaCPDet.row($(this).parents('tr')).data();
        //window.open(ResolveUrl("~/tmComprasDetalle/Edit/" + data.intIdCPDtlle), '_self');
        $('#modalDevoluciones').modal({
            backdrop: 'static',
            keyboard: false
        });

        $('#modalDevoluciones').on('shown.bs.modal', function () {
            $("#varCodigo").focus();
            cargarTablaItemFactCP(data.intIdCPDtlle);
        });
    });
}

function anularCP() {
    
    var intIdCP = tablaCPs.rows({ selected: true }).data()[0].intIdCP;
    var parameter = { "intIdCP": intIdCP }

    $.ajax({
        type: 'POST',
        url: ResolveUrl("~/tmCPEncabezado/AnularCP"),
        data: JSON.stringify(parameter),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        success: function (resultado) {
            if (resultado.success) {
                tablaCPs.rows({ selected: true }).data()[0].bitAnldo = true;
                tablaCPs.rows({ selected: true }).data().draw();
                pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                    + resultado.mensaje + '</div>')
                                .fadeIn(100);
            }
            else {
                pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                                .fadeIn(100);
            }
        },
        error: function (data, success, error) {
            pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                                .fadeIn(100);
        }
    });
}

function modificarCP() {
    
    var intIdCP = tablaCPs.rows({ selected: true }).data()[0].intIdCP;
    var parameter = { "intIdCP": intIdCP }

    alertify.confirm("Crear Modificación de CP", "¿Realmente desea crea una modificación para el CP seleccionado?", function () {
        
        $.ajax({
            type: 'POST',
            url: ResolveUrl("~/tmCPEncabezado/ModificarCP"),
            data: JSON.stringify(parameter),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            cache: false,
            success: function (resultado) {
                if (resultado.success) {
                    tablaCPs.rows({ selected: true }).data().draw();
                    pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                        + resultado.mensaje + '</div>')
                                    .fadeIn(100);
                }
                else {
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                                    .fadeIn(100);
                }
            },
            error: function (data, success, error) {
                pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                                    .fadeIn(100);
            }
        });

    }, null);

}

function cargarTablaItemFactCP(intIdCPDtlle) {

    if ($.fn.DataTable.isDataTable('#table-itemsCompraCP')) {
        $('#bodyModal').empty().append('<table class="table table-striped table-bordered table-hover display compact" id="table-itemsCompraCP" style="border-spacing: 0px; border-collapse: separate; width: 100%;"></table>');
    };

    $('#table-itemsCompraCP').DataTable({
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmComprasDetalle/GetComprasDetalleCP/' + intIdCPDtlle),
            contentType: 'application/json; charset=utf-8',
            data: function (data) { return data = JSON.stringify(data); },
            complete: function () {
                $(".Numero").on("keypress", Numero_KeyPress);
                $(".Decimal").on("keypress", Decimal_KeyPress);
            }
        },
        processing: true,
        searching: false,
        ordering: false,
        info: false,
        colReorder: true,
        scrollX: true,
        scrollY: 200,
        responsive: true,
        scrollCollapse: true,
        scroller: {
            loadingIndicator: true
        },
        paging: false,
        autoWidth: true,
        deferRender: true,
        columns: [
            { title: "intIdCmpraDtlle", data: "intIdCmpraDtlle", visible: false },
            { title: "intIdCmpra", data: "intIdCmpra", visible: false },
            { title: "intIdInsmo", data: "intIdInsmo", visible: false },
            { title: "intIdCPDtlle", data: "intIdCPDtlle", visible: false },
            { title: "Ítem", data: "intCdItem" },
            { title: "CI", data: "varCdInsmo" },
            { title: "Insumo", data: "varDscrpcionInsmo" },
            { title: "Peso Bruto", data: "numCntdadFctra", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Cnt. CP", data: "numCntdadCP", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
            { title: "Vr. Unitario", data: "numVlorUntrio", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Vr. Total", data: "numSbttal", render: $.fn.dataTable.render.number(',', '.', 0, '$'), className: 'text-right' },
            { title: "Cnt. Acum. Devol.", data: "numCantAcumNotasCrdto", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
            { title: "Vr. Acum. Devol.", data: "numVlorAcumNotasCrdto", render: $.fn.dataTable.render.number(',', '.', 0, '$'), className: 'text-right' },
            {
                title: 'Cnt. Devolución',
                render: function (data, type, full) {
                    return '<input class="Decimal" id="cnt_' + full.intIdCmpraDtlle + '" style="width: 70px;" value="0" />';
                }
            },
            {
                title: 'Vr. Devolución',
                render: function (data, type, full) {
                    return '<input class="Decimal" id="vr_' + full.intIdCmpraDtlle + '" style="width: 70px;" value="0" />';
                }
            }
        ],
        order: []
    });
}

// Botón del Modal para crear las devoluciones
$("#btnCrearDevol").click(function () {

    $("#btnCrearDevol").button('loading');

    //Se seleccionan los input
    var varCodigo = $("#varCodigo");
    var valid_varCodigo = $("#valid_varCodigo");

    var fecFecha = $("#fecFecha");
    var valid_fecFecha = $("#valid_fecFecha");

    //Se valida que el código no esté vacío
    if (varCodigo.val() !== "")
    {
        //Se remueven las propiedades de error del Código
        varCodigo.parent().removeClass("has-error");
        valid_varCodigo.empty();

        //Se valida que la fecha no esté vacía
        if (fecFecha.val() !== "")
        {
            //Se remueven las propiedades de error de la fecha
            fecFecha.parent().removeClass("has-error");
            valid_fecFecha.empty();

            var table = $('#table-itemsCompraCP').DataTable();
            var devol = [];
            var listDevDetalle = [];

            // Se recorre el Datatable para guardar y validar las cantidades y valores de devolución ingresadas
            table.rows().every(function (rowIdx, tableLoop, rowLoop) {
                var d = this.data();
                var cant = Number($("#cnt_" + d.intIdCmpraDtlle).val().replace(",", "."));
                var valor = Number($("#vr_" + d.intIdCmpraDtlle).val().replace(",", "."));

                if ((cant !== "" && cant !== "0") || (valor !== "" && valor !== "0")) {

                    if ((valor <= d.numSbttal) && (cant <= d.numCntdadCP)) {

                        //Se crea un Array con los valores ingresados
                        listDevDetalle.push({
                            "intIdCmpraDtlle": d.intIdCmpraDtlle,
                            "numCntdad": cant,
                            "numVlorUntrio": d.numVlorUntrio,
                            "numSbttal": valor
                        });                        
                    } else {
                        listDevDetalle = [];
                        $('#message').html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>La cantidad debe ser menor o igual que la Cantidad CP y<br />El valor debe ser menor o igual que el Valor Total</div>')
                               .fadeIn();
                    }
                }
                else {
                    listDevDetalle = [];
                    $('#message').html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>Debe ingresar una cantidad o un valor de devolución</div>')
                                .fadeIn();                    
                }
            });

            //Si hubieron detalles buenos
            if (listDevDetalle.length > 0) {

                devol = {
                    "varNtaCrdto": varCodigo.val(),
                    "fecNtaCrdto": fecFecha.val(),
                    "tmNotasCreditoDetalle": listDevDetalle
                };

                CrearNotaCredito(devol);
            } else {
                $("#btnCrearDevol").button('reset');
            }
        }
        else
        {
            fecFecha.parent().addClass("has-error");
            valid_fecFecha.empty().append("Requerido");
            $("#btnCrearDevol").button('reset');
        }
        
    }
    else
    {
        varCodigo.parent().addClass("has-error");
        valid_varCodigo.empty().append("Requerido");
        $("#btnCrearDevol").button('reset');
    }

});

function CrearNotaCredito(notaCredito) {

    var parameter = { "tmNotasCreditoEncabezado": notaCredito }

    //console.log(JSON.stringify(parameter));
    $.ajax({
        type: 'POST',
        url: ResolveUrl("~/tmNotasCreditoDetalle/CrearNotaCredito"),
        data: JSON.stringify(parameter),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        success: function (resultado) {
            if (resultado.success) {

                $("#btnCrearDevol").button('reset');
                $('#modalDevoluciones').modal('hide');

                $('#table-detalle-cps-' + resultado.intIdCP).DataTable().data().draw();

                pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                    + resultado.mensaje + '</div>')
                                .fadeIn(100);
            }
            else {
                $("#btnCrearDevol").button('reset');
                pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                                .fadeIn(100);
            }
        },
        error: function (data, success, error) {
            $("#btnCrearDevol").button('reset');
            pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                                .fadeIn(100);
        }
    });
}

// Función que muestra el modal para generar XML de CPs
function generarXmlCP() {
    
    var htmlBody = $("#divFormCrearXmlCP").html();

    // Mostrar Modal
    showBSModal({
        title: "Generar XML CPs",
        body: htmlBody,
        size: "medium",
        actions: [
            {
                label: 'Crear XML',
                cssClass: 'btn-success btnCrearXml',
                onClick: function (e) {
                    $(".modal #formCrearXmlCP").submit();
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
            inicializarControlesFormXmlCPs();
            $('.modal #formCrearXmlCP').on('submit', function (e) {
                crearXmlCP();
                e.preventDefault();
            });
        }
    });
}

//Función para inicializar los controles del formulario de generación de XML de CPs
function inicializarControlesFormXmlCPs() {
    var fecha = new Date();
    var month = fecha.getMonth() + 1;
    var day = fecha.getDate();
    var year = fecha.getFullYear();
    var date = year + "-" + ('0' + month).slice(-2) + "-" + ('0' + day).slice(-2);

    $(".modal #intAno").val(parseInt(year));
    $(".modal #intConcepto").val(1);
    $(".modal #intFormato").val(640);
    $(".modal #intVersion").val(1);
    $(".modal #intNumEnvio").val(1);
    $(".modal #fecFechaEnvio").val(date);
    $(".modal #fecFechaInicial").val(date);
    $(".modal #fecFechaFinal").val(date);
}

//Función crear XML CP
function crearXmlCP() {
    var formCrearXmlCP = $(".modal #formCrearXmlCP");

    //Validar
    formCrearXmlCP.validate({
        rules: {
            intAno: {
                required: true,
                min: 1995,
                max: 2100
            },
            intConcepto: {
                required: true,
                min: 1
            },
            intFormato: "required",
            intVersion: {
                required: true,
                min: 1,
                max: 99
            },
            intNumEnvio: {
                required: true,
                min: 1
            },
            fecFechaEnvio: "required",
            fecFechaInicial: "required",
            fecFechaFinal: "required",
        },
        messages: {
            intAno: {
                required: "Debe ingrear el año.",
                min: "Debe ingresar un año mayor o igual que 1995",
                max: "Debe ingresar un año menor o igual que 2100"
            },
            intConcepto: {
                required: "Debe ingrear el concepto.",
                min: "Debe ingresar un concepto mayor o igual que 1"
            },
            intFormato: "Debe ingrear el formato.",
            intVersion: {
                required: "Debe ingrear la versión.",
                min: "Debe ingresar una versión mayor o igual que 1",
                max: "Debe ingresar una versión menor o igual que 99"
            },
            intNumEnvio: {
                required: "Debe ingrear el número de envío.",
                min: "Debe ingresar un número de envío mayor o igual que 1"
            },
            fecFechaEnvio: "Debe ingrear la fecha de envío.",
            fecFechaInicial: "Debe ingrear la fecha inicial.",
            fecFechaFinal: "Debe ingrear la fecha final."
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            formData = formCrearXmlCP.serializeArray();

            $.ajax({
                url: ResolveUrl('~/tmCPEncabezado/CrearXmlCP'),
                type: "POST",
                data: JSON.stringify({ datosEncXmlCP: formData, listFiltros: listFiltrosDataTable }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                cache: false,
                beforeSend: function () {
                    $(".btnCrearXml").parents('.modal').modal('hide');
                    BlockUI("Creando XML...");
                    //$(".btnCrearXml").button('Creando XML...');
                },
                success: function (resultado) {
                    if (resultado.success) {

                        var link = document.createElement('a');
                        link.setAttribute('href', ResolveUrl(resultado.urlArchivo));
                        link.setAttribute('target', "_blank");
                        link.setAttribute('download', "");
                        link.click();
                        //window.open(ResolveUrl(resultado.urlArchivo), "_blank", "");

                        $(".btnCrearXml").button('reset');                        

                        pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                            + resultado.mensaje + '</div>')
                                        .fadeIn(100);
                    }
                    else {
                        $(".btnCrearXml").button('reset');
                        pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                                        .fadeIn(100);
                    }
                    //UnblockUI();                    
                },
                error: function (data, success, error) {
                    $(".btnCrearXml").button('reset');
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                                        .fadeIn(100);
                    //UnblockUI();
                    //$(".btnCrearXml").parents('.modal').modal('hide');
                },
                complete: function () {
                    UnblockUI();
                }
            });
        }
    });
}

function GenerarCPsManulaes() {

    $.ajax({
        url: ResolveUrl('~/tmCPEncabezado/GenerarCPsManuales'),
        type: "POST",
        data: JSON.stringify({ listFiltros: listFiltrosDataTable }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false
    });
}