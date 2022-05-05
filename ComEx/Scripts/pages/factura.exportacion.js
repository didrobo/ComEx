var frmTmvFacturasExportacionEncabezadoCrear = $("#frmTmvFacturasExportacionEncabezadoCrear");
var btnGuardar = $("#btnGuardar");
var frmTmvFacturasExportacionEncabezadoEditar = $("#frmTmvFacturasExportacionEncabezadoEditar");
var btnEditar = $("#btnEditar");

var divFormNotasCredito = $("#divFormNotasCredito");
var divTablaNotasCredito = $("#divTablaNotasCredito");

$(document).on('ready', function () {
    crearTabla();

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });

    frmTmvFacturasExportacionEncabezadoCrear_Submit();
    frmTmvFacturasExportacionEncabezadoEditar_Submit();
});

function crearTabla() {

    configExportServer('GetFacturaExportacion', '#table-facturas-exportacion');

    tablaFactExport = $('#table-facturas-exportacion').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        serverSide: true,
        ajax: {
            type: "POST",
            url: 'GetFacturaExportacion',
            contentType: 'application/json; charset=utf-8',
            data: function (data) {
                data.listFiltros = listFiltrosDataTable;
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
            {
                orderable: false,
                data: null,
                defaultContent: '<i class="buttonCrud btn_table_azul fa fa-book btnNotasCredito" style="cursor: pointer;" title="Notas Crédito"></i>'
            },
            { title: "intIdFctraExprtcion", data: "intIdFctraExprtcion", visible: false },
            { title: "Aux", data: "varNmroAuxliar" },
            { title: "Factura", data: "varNmroFctra" },
            { title: "Exportación", data: "varDcmntoTrnsprte" },
            { title: "Lote", data: "varNmroExprtcion" },
            { title: "Fec. Factura", data: "fecFctra", render: dateJsonToString },
            { title: "F. Vencimiento", data: "fecVncmientoFctra", render: dateJsonToString },
            { title: "F. Exportación", data: "fecExprtcion", render: dateJsonToString },
            { title: "Comprador", data: "varCmprdor" },
            { title: "Tasa Cambio", data: "numTsaCmbio", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Subtotal", data: "numSbttal", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Incoterm", data: "varCdIncterms" },
            { title: "Moneda", data: "varCdMnda" },
            { title: "Vr. FOB", data: "numVlorFOB", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Fletes", data: "numFltes", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Iva", data: "numGstos", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Seguro", data: "numSgroUS", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },          
            { title: "Peso Bruto", data: "numPsoBrto", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Peso Neto", data: "numPsoNto", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },          
            {
                title: "Anulada",
                data: "bitAnlda",
                render: function (Anulada) {
                    if (Anulada) {
                        return "Si";
                    }
                    else {
                        return "No";
                    }
                }
            },
            {
                title: "Vnta Ncnal",
                data: "bitVntaNcional",
                render: function (VntaNcional) {
                    if (VntaNcional) {
                        return "Si";
                    }
                    else {
                        return "No";
                    }
                }
            }
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
                text: '<i class="buttonCrud fa fa-file-o"></i>',
                titleAttr: 'Nuevo',
                action: function (e, dt, node, config) {
                    window.open('Create', '_self');
                }
            },
            {
                text: '<i class="buttonCrud fa fa-file disabled_control" id="btnNuevoDetalle"></i>',
                titleAttr: 'Nuevo Detalle Factura',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdFctraExprtcion = dt.row({ selected: true }).data().intIdFctraExprtcion;
                    window.open(ResolveUrl("~/tmvFacturasExportacionDetalle/Create/" + intIdFctraExprtcion), '_self');
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditar"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdFctraExprtcion = dt.row({ selected: true }).data().intIdFctraExprtcion;
                    window.open(ResolveUrl("~/tmvFacturasExportacionEncabezado/Edit/" + intIdFctraExprtcion), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdFctraExprtcion = dt.row({ selected: true }).data().intIdFctraExprtcion;
                    alertify.confirm("Eliminar Factura de Exportación", "¿Realmente desea eliminar la factura de exportación seleccionada?", function () {
                        //Llamar método para eliminar
                        EliminarFactura(intIdFctraExprtcion, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);

                                //Boton Nuevo Detalle
                                $("#btnNuevoDetalle").removeClass("disabled_control");
                                tablaFactExport.button(2).enable(true);

                                //Boton Editar
                                $("#btnEditar").removeClass("disabled_control");
                                tablaFactExport.button(3).enable(true);

                                //Boton Eliminar
                                $("#btnBorrar").removeClass("disabled_control");
                                tablaFactExport.button(4).enable(true);
                            }
                        });
                    }, null);
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
            }        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: [[7, 'desc']],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();
            var json = api.ajax.json();

            $(api.column(12).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numSbttal));
            $(api.column(15).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numVlorFOB));
            $(api.column(16).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numFltes));
            $(api.column(17).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numGstos));
            $(api.column(18).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numSgroUS));
            $(api.column(19).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numPsoBrto));
            $(api.column(20).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numPsoNto));

        }
    });

    // Array to track the ids of the details displayed rows
    var detailRows = [];

    // Este es evento es para mostrar el detalle de la fila usando el botoón (+)
    $('#table-facturas-exportacion tbody').on('click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tablaFactExport.row(tr);
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
    tablaFactExport.on('draw', function () {
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger('click');
        });
    });

    $('#table-facturas-exportacion_filter input').unbind();
    $('#table-facturas-exportacion_filter input').bind('keyup', function (e) {
        if (e.keyCode === 13) {
            tablaFactExport.search(this.value).draw();
        }
    });

    tablaFactExport.on('select', function (e, dt, type, indexes) {
        //Boton Nuevo Detalle
        $("#btnNuevoDetalle").removeClass("disabled_control");
        tablaFactExport.button(2).enable(true);

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaFactExport.button(3).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaFactExport.button(4).enable(true);
    });

    tablaFactExport.on('deselect', function (e, dt, type, indexes) {
        //Boton Nuevo Detalle Compra
        $("#btnNuevoDetalle").addClass("disabled_control");
        tablaFactExport.button(2).enable(false);

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaFactExport.button(3).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaFactExport.button(4).enable(false);
    });


    tablaFactExport.on('click', '.btnNotasCredito', function (e) {
        var data = tablaFactExport.row($(this).parents('tr')).data();

        LlenarComboTipoDcto();
        LimpiarCamposFormNotasCredito();
        divTablaNotasCredito.fadeIn();
        divFormNotasCredito.fadeOut();

        $('#modalNotasCredito').modal({
            backdrop: 'static',
            keyboard: false
        });

        $('#modalNotasCredito').on('shown.bs.modal', function () {
            cargarTablaNotasCredito(data.intIdFctraExprtcion);
        });
    });

}

function getTblDetalle(d) {
    // la variable d contiene un objeto tmvFacturasExportacionEncabezado
    return '<table id="table-detalle-facturas-' + d.intIdFctraExprtcion + '" class="table table-striped table-bordered table-hover display compact" style="border-spacing: 0px; border-collapse: separate; width: 20%;" />';
}

function drawTblDetalle(d) {
    var tablaFacturaDet = $('#table-detalle-facturas-' + d.intIdFctraExprtcion).DataTable({
        serverSide: true,
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmvFacturasExportacionDetalle/GetFacturasDetalle/' + d.intIdFctraExprtcion),
            contentType: 'application/json; charset=utf-8',
            data: function (data) { return data = JSON.stringify(data); }
        },
        processing: true,
        responsive: true,
        paging: false,
        colReorder: true,
        autoWidth: true,
        searching: false,
        ordering: false,
        info: false,
        columns: [
            {
                defaultContent: '<i class="buttonCrud del fa fa-trash-o btnBorrarDetalle" style="cursor: pointer;"></i> <i class="buttonCrud fa fa-pencil-square-o btnEditarDetalle" style="cursor: pointer;"></i>',
                data: null
            },
            { title: "intIdFctraExprtcionDtlle", data: "intIdFctraExprtcionDtlle", visible: false },
            { title: "intIdFctraExprtcion", data: "intIdFctraExprtcion", visible: false },
            { title: "Ítem", data: "intItem" },
            { title: "Producto", data: "tmvProductosFacturacion.varDscrpcionEspñol" },
            { title: "Cantidad", data: "numCntdad", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Vr. Unitario", data: "numVlorUntrio", render: $.fn.dataTable.render.number(',', '.', 5, '$'), className: 'text-right' },
            { title: "Vr. Total", data: "numSbttal", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "P. Bruto", data: "numPsoBrto", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "P. Neto", data: "numPsoNto", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' }
        ]
    });

    $('#table-detalle-facturas-' + d.intIdFctraExprtcion + ' tbody').on('click', '.btnBorrarDetalle', function (e) {
        var data = tablaFacturaDet.row($(this).parents('tr')).data();
        alertify.confirm("Eliminar Detalle Factura de Exportación", "¿Realmente desea eliminar el detalle de la factura seleccionado?", function () {
            //Llamar método para eliminar
            EliminarFacturaDetalle(data.intIdFctraExprtcionDtlle, function (resultado) {
                tablaFacturaDet.row($(this)).remove().draw(false);
            });
            tablaFacturaDet.row($(this).parents('tr')).remove().draw(false);
        }, null);
    });

    $('#table-detalle-facturas-' + d.intIdFctraExprtcion + ' tbody').on('click', '.btnEditarDetalle', function (e) {
        var data = tablaFacturaDet.row($(this).parents('tr')).data();
        window.open(ResolveUrl("~/tmvFacturasExportacionDetalle/Edit/" + data.intIdFctraExprtcionDtlle), '_self');
    });
}

function frmTmvFacturasExportacionEncabezadoCrear_Submit() {
     
    //Validar
    frmTmvFacturasExportacionEncabezadoCrear.validate({
        rules: {
            varNmroAuxliar: "required",
            varNmroFctra: "required",
            fecFctra: "required",
            fecVncmientoFctra: "required",
            intIdCmprdor: "required",
            intIdImprtdorExprtdor: "required",
            numTsaCmbio: "required",
            numSbttal: "required",
            numVlorFOB: "required",
            numFltes: "required",
            numGstos: "required",
            numSgroUS: "required",
            numPsoBrto: "required",
            numPsoNto: "required"
        },
        messages: {
            varNmroAuxliar: "Asegúrese de ingresar el auxiliar",
            varNmroFctra: "Asegúrese de ingresar el número de factura",
            fecFctra: "Asegúrese de ingresar la fecha",
            fecVncmientoFctra: "Asegúrese de ingresar la fecha de vencimiento",
            intIdCmprdor: "Asegúrese de seleccionar el comprador",
            intIdImprtdorExprtdor: "Asegúrese de seleccionar el exportador",
            numTsaCmbio: "Asegúrese de ingresar la tasa de cambio",
            numSbttal: "Asegúrese de ingresar el subtotal",
            numVlorFOB: "Asegúrese de ingresar el valor FOB",
            numFltes: "Asegúrese de ingresar los fletes",
            numGstos: "Asegúrese de ingresar los gastos",
            numSgroUS: "Asegúrese de ingresar el seguro",
            numPsoBrto: "Asegúrese de ingresar el peso bruto",
            numPsoNto: "Asegúrese de ingresar el peso neto"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmvFacturasExportacionEncabezadoCrear.serializeArray(), function (index, value) {
                if (value.name != "bitAnlda" && value.name != "bitVntaNcional") {
                    formData.append(value.name, value.value);
                }
            });

            var bitAnlda = $("#bitAnlda").is(":checked") == true ? true : false;
            var bitVntaNcional = $("#bitVntaNcional").is(":checked") == true ? true : false;

            formData.append("bitAnlda", bitAnlda);
            formData.append("bitVntaNcional", bitVntaNcional);
            $.ajax({
                url: ResolveUrl('~/tmvFacturasExportacionEncabezado/SetTmFacturaExportacion'),
                type: "POST",
                contentType: false,
                cache: false,
                dataType: "json",
                processData: false,
                data: formData,
                beforeSend: function () {
                    BlockUI("Guardando...");
                    console.info("Enviando infromación...");
                    btnGuardar.button('loading');
                },
                success: function (data) {
                    if (data.Success) {
                        location.href = ResolveUrl(data.Url);

                        //Limpiar el botón
                        btnGuardar.button('reset');

                        UnblockUI();
                    }
                    else {
                        UnblockUI();
                        console.error(data.Message);

                        //Limpiar el botón
                        btnGuardar.button('reset');
                    }

                }
            });
        }
    });
}

function EliminarFactura(intIdFctraExprtcion, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmvFacturasExportacionEncabezado/Delete'),
        type: "POST",
        data: { id: intIdFctraExprtcion },
        beforeSend: function () {
            console.info("Enviando infromación...");
        },
        success: function (data) {
            if (data.Success) {
                ActualizarTabla(true);
            }
            else {
                console.error(data.Message);
                ActualizarTabla(false);
            }
        },
    });
}

function EliminarFacturaDetalle(intIdFctraExprtcionDtlle, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmvFacturasExportacionDetalle/Delete'),
        type: "POST",
        data: { id: intIdFctraExprtcionDtlle },
        beforeSend: function () {
            console.info("Enviando infromación...");
        },
        success: function (data) {
            if (data.Success) {
                ActualizarTabla(true);
            }
            else {
                console.error(data.Message);
                ActualizarTabla(false);
            }
        },
    });
}

function frmTmvFacturasExportacionEncabezadoEditar_Submit() {

    //Validar
    frmTmvFacturasExportacionEncabezadoEditar.validate({
        rules: {
            varNmroAuxliar: "required",
            varNmroFctra: "required",
            fecFctra: "required",
            fecVncmientoFctra: "required",
            intIdCmprdor: "required",
            intIdImprtdorExprtdor: "required",
            numTsaCmbio: "required",
            numSbttal: "required",
            numVlorFOB: "required",
            numFltes: "required",
            numGstos: "required",
            numSgroUS: "required",
            numPsoBrto: "required",
            numPsoNto: "required"
        },
        messages: {
            varNmroAuxliar: "Asegúrese de ingresar el auxiliar",
            varNmroFctra: "Asegúrese de ingresar el número de factura",
            fecFctra: "Asegúrese de ingresar la fecha",
            fecVncmientoFctra: "Asegúrese de ingresar la fecha de vencimiento",
            intIdCmprdor: "Asegúrese de seleccionar el comprador",
            intIdImprtdorExprtdor: "Asegúrese de seleccionar el exportador",
            numTsaCmbio: "Asegúrese de ingresar la tasa de cambio",
            numSbttal: "Asegúrese de ingresar el subtotal",
            numVlorFOB: "Asegúrese de ingresar el valor FOB",
            numFltes: "Asegúrese de ingresar los fletes",
            numGstos: "Asegúrese de ingresar los gastos",
            numSgroUS: "Asegúrese de ingresar el seguro",
            numPsoBrto: "Asegúrese de ingresar el peso bruto",
            numPsoNto: "Asegúrese de ingresar el peso neto"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmvFacturasExportacionEncabezadoEditar.serializeArray(), function (index, value) {
                if (value.name != "bitAnlda" && value.name != "bitVntaNcional") {
                    formData.append(value.name, value.value);
                }
            });

            var bitAnlda = $("#bitAnlda").is(":checked") == true ? true : false;
            var bitVntaNcional = $("#bitVntaNcional").is(":checked") == true ? true : false;

            formData.append("bitAnlda", bitAnlda);
            formData.append("bitVntaNcional", bitVntaNcional);

            $.ajax({
                url: ResolveUrl('~/tmvFacturasExportacionEncabezado/Update'),
                type: "POST",
                contentType: false,
                cache: false,
                dataType: "json",
                processData: false,
                data: formData,
                beforeSend: function () {
                    BlockUI("Actualizando...");
                    console.info("Enviando infromación...");
                    btnEditar.button('loading');
                },
                success: function (data) {
                    if (data.Success) {
                        location.href = ResolveUrl(data.Url);

                        //Limpiar el botón
                        btnEditar.button('reset');
                    }
                    else {
                        console.error(data.Message);

                        //Limpiar el botón
                        btnEditar.button('reset');
                    }

                    UnblockUI();
                }
            });
        }
    });
}

function cargarTablaNotasCredito(intIdFctraExprtcion)
{
    $("#hfIntIdFctraExprtcion").val(intIdFctraExprtcion);

    if ($.fn.DataTable.isDataTable('#table-notas-credito')) {
        $('#bodyModal').empty().append('<table class="table table-striped table-bordered table-hover display compact" id="table-notas-credito" style="border-spacing: 0px; border-collapse: separate; width: 100%;"></table>');
    };

    var tablaNotasCredito = $('#table-notas-credito').DataTable({
        dom: "<'row'<'col-md-12'B>><'row'<'col-md-12'tr>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmvFacturasExportacionNotasCredito/GetNotasCreditoFactExp/' + intIdFctraExprtcion),
            contentType: 'application/json; charset=utf-8',
            data: function (data) { return data = JSON.stringify(data); },
            complete: function () {

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
        columns: [
            {
                data: null,
                defaultContent: '',
                className: 'select-checkbox',
                orderable: false
            },
            { title: "intIdFctraExprtcionNtaCrdto", data: "intIdFctraExprtcionNtaCrdto", visible: false },
            { title: "intIdFctraExprtcion", data: "intIdFctraExprtcion", visible: false },
            { title: "Nota Crédito", data: "varNmroNtaCrdto" },
            { title: "Fec. Asiento", data: "fecAsientoNtaCrdto", render: dateJsonToString },
            { title: "Tipo Descuento", data: "tmTiposDescuentos.varDscrpcionTpoDscuento" },
            { title: "Valor", data: "numVlorNtaCrdto", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Observaciones", data: "varObsrvciones" }
        ],
        buttons: [
            {
                text: '<i class="buttonCrud fa fa-file-o"></i>',
                titleAttr: 'Nuevo',
                action: function (e, dt, node, config) {

                    LimpiarCamposFormNotasCredito();

                    $("#hfOperacion").val("Nuevo");
                    $("#bitActva").attr('checked', 'checked')

                    divTablaNotasCredito.fadeOut();
                    divFormNotasCredito.fadeIn();
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditarNotaCredito"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdFctraExprtcionNtaCrdto = dt.row({ selected: true }).data().intIdFctraExprtcionNtaCrdto;
                    var varNmroNtaCrdto = dt.row({ selected: true }).data().varNmroNtaCrdto;
                    var fecAsientoNtaCrdto = dt.row({ selected: true }).data().fecAsientoNtaCrdto;
                    var intIdTpoDscuento = dt.row({ selected: true }).data().intIdTpoDscuento;
                    var numVlorNtaCrdto = dt.row({ selected: true }).data().numVlorNtaCrdto;
                    var varObsrvciones = dt.row({ selected: true }).data().varObsrvciones;


                    var find = '/';
                    var re = new RegExp(find, 'g');

                    $("#hfOperacion").val("Editar");
                    $("#hfIntIdFctraExprtcionNtaCrdto").val(intIdFctraExprtcionNtaCrdto);

                    $("#varNmroNtaCrdto").val(varNmroNtaCrdto);
                    $("#fecAsientoNtaCrdto").val(dateJsonToString(fecAsientoNtaCrdto).replace(re, "-"));
                    $("#intIdTpoDscuento").val(intIdTpoDscuento);                    
                    $("#numVlorNtaCrdto").val(numVlorNtaCrdto);
                    $("#varObsrvciones").val(varObsrvciones);


                    divTablaNotasCredito.fadeOut();
                    divFormNotasCredito.fadeIn();
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrarNotaCredito"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdFctraExprtcionNtaCrdto = dt.row({ selected: true }).data().intIdFctraExprtcionNtaCrdto;
                    alertify.confirm("Eliminar Nota Crédito", "¿Realmente desea eliminar la nota crédito seleccionada?", function () {
                        //Llamar método para eliminar
                        EliminarNotaCredito(intIdFctraExprtcionNtaCrdto, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditarNotaCredito").addClass("disabled_control");
                                tablaNotasCredito.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrarNotaCredito").addClass("disabled_control");
                                tablaNotasCredito.button(2).enable(false);
                            }
                        });
                    }, null);
                }
            }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: [[4, 'desc']]
    });

    tablaNotasCredito.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditarNotaCredito").removeClass("disabled_control");
        tablaNotasCredito.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrarNotaCredito").removeClass("disabled_control");
        tablaNotasCredito.button(2).enable(true);
    });

    tablaNotasCredito.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditarNotaCredito").addClass("disabled_control");
        tablaNotasCredito.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrarNotaCredito").addClass("disabled_control");
        tablaNotasCredito.button(2).enable(false);
    });
}

function EliminarNotaCredito(intIdFctraExprtcionNtaCrdto, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmvFacturasExportacionNotasCredito/Delete'),
        type: "POST",
        data: { id: intIdFctraExprtcionNtaCrdto },
        beforeSend: function () {
            console.info("Enviando infromación...");
        },
        success: function (data) {
            if (data.Success) {
                ActualizarTabla(true);
            }
            else {
                console.error(data.Message);
                ActualizarTabla(false);
            }
        },
    });
}

//Botón cancelar modal notas crédito
$("#btnCancelarNotaCred").click(function () {
    divTablaNotasCredito.fadeIn();
    divFormNotasCredito.fadeOut();
});

//Botón guardar modal notas crédito
$("#btnGuardarNotaCred").click(function () {

    //Campos
    var intIdFctraExprtcionNtaCrdto = $("#hfIntIdFctraExprtcionNtaCrdto").val();
    var intIdFctraExprtcion = $("#hfIntIdFctraExprtcion").val();
    var varNmroNtaCrdto = $("#varNmroNtaCrdto").val();
    var fecAsientoNtaCrdto = $("#fecAsientoNtaCrdto").val();
    var intIdTpoDscuento = $("#intIdTpoDscuento").val();
    var numVlorNtaCrdto = $("#numVlorNtaCrdto").val();
    var varObsrvciones = $("#varObsrvciones").val();
    var operacion = $("#hfOperacion").val();

    var valido = validarFormNotasCredito();

    if (valido) {

        var formData = new FormData();

        formData.append("intIdFctraExprtcionNtaCrdto", intIdFctraExprtcionNtaCrdto);
        formData.append("intIdFctraExprtcion", intIdFctraExprtcion);
        formData.append("varNmroNtaCrdto", varNmroNtaCrdto);
        formData.append("fecAsientoNtaCrdto", fecAsientoNtaCrdto);
        formData.append("intIdTpoDscuento", intIdTpoDscuento);
        formData.append("numVlorNtaCrdto", numVlorNtaCrdto);
        formData.append("varObsrvciones", varObsrvciones);
        formData.append("operacion", operacion);

        $.ajax({
            url: ResolveUrl('~/tmvFacturasExportacionNotasCredito/CUNotaCredito'),
            type: "POST",
            contentType: false,
            cache: false,
            dataType: "json",
            processData: false,
            data: formData,
            beforeSend: function () {
                console.info("Enviando infromación...");
                $("#btnGuardarNotaCred").button('loading');
            },
            success: function (data) {
                if (data.Success) {
                    //Limpiar el botón
                    $("#btnGuardarNotaCred").button('reset');

                    LimpiarCamposFormNotasCredito();
                    cargarTablaNotasCredito(intIdFctraExprtcion);

                    divTablaNotasCredito.fadeIn();
                    divFormNotasCredito.fadeOut();
                }
                else {
                    console.error(data.Message);

                    $("#messageNotasCredito").html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                        + resultado.mensaje + '</div>')
                        .fadeIn(100);

                    //Limpiar el botón
                    $("#btnGuardarNotaCred").button('reset');
                }
            }
        });
    }
});

function validarFormNotasCredito() {

    var varNmroNtaCrdto = $("#varNmroNtaCrdto");
    var fecAsientoNtaCrdto = $("#fecAsientoNtaCrdto");
    var intIdTpoDscuento = $("#intIdTpoDscuento");
    var numVlorNtaCrdto = $("#numVlorNtaCrdto");
    var varObsrvciones = $("#varObsrvciones");

    //Validaciones
    var valid_varNmroNtaCrdto = $("#valid_varNmroNtaCrdto");
    var valid_fecAsientoNtaCrdto = $("#valid_fecAsientoNtaCrdto");
    var valid_intIdTpoDscuento = $("#valid_intIdTpoDscuento");
    var valid_numVlorNtaCrdto = $("#valid_numVlorNtaCrdto");
    var valid_varObsrvciones = $("#valid_varObsrvciones");

    var valido = true;

    //Se valida el número de la nota crédito
    if (varNmroNtaCrdto.val() !== "") {
        //Se remueven las propiedades de error 
        varNmroNtaCrdto.parent().removeClass("has-error");
        valid_varNmroNtaCrdto.empty();
        valido = true;
    }
    else {
        varNmroNtaCrdto.parent().addClass("has-error");
        valid_varNmroNtaCrdto.empty().append("Requerido");
        $("#btnGuardarNotaCred").button('reset');
        valido = false;
    }

    //Se valida el campo fecha asiento
    if (fecAsientoNtaCrdto.val() !== "") {
        //Se remueven las propiedades de error 
        fecAsientoNtaCrdto.parent().removeClass("has-error");
        valid_fecAsientoNtaCrdto.empty();
        valido = true;
    }
    else {
        fecAsientoNtaCrdto.parent().addClass("has-error");
        valid_fecAsientoNtaCrdto.empty().append("Requerido");
        $("#btnGuardarNotaCred").button('reset');
        valido = false;
    }

    //Se valida el tipo de dcto
    if (intIdTpoDscuento.val() !== null) {
        //Se remueven las propiedades de error 
        intIdTpoDscuento.parent().removeClass("has-error");
        valid_intIdTpoDscuento.empty();
        valido = true;
    }
    else {
        intIdTpoDscuento.parent().addClass("has-error");
        valid_intIdTpoDscuento.empty().append("Requerido");
        $("#btnGuardarNotaCred").button('reset');
        valido = false;
    }

    //Se valida el campo valor nota crédito
    if (numVlorNtaCrdto.val() !== "") {
        //Se remueven las propiedades de error 
        numVlorNtaCrdto.parent().removeClass("has-error");
        valid_numVlorNtaCrdto.empty();
        valido = true;
    }
    else {
        numVlorNtaCrdto.parent().addClass("has-error");
        valid_numVlorNtaCrdto.empty().append("Requerido");
        $("#btnGuardarNotaCred").button('reset');
        valido = false;
    }

    //Se valida el campo observaciones
    if (varObsrvciones.val() !== "") {
        //Se remueven las propiedades de error 
        varObsrvciones.parent().removeClass("has-error");
        valid_varObsrvciones.empty();
        valido = true;
    }
    else {
        varObsrvciones.parent().addClass("has-error");
        valid_varObsrvciones.empty().append("Requerido");
        $("#btnGuardarNotaCred").button('reset');
        valido = false;
    }

    return valido;
}

function LimpiarCamposFormNotasCredito() {

    $("#varNmroNtaCrdto").val("");
    $("#fecAsientoNtaCrdto").val("");
    $("#intIdTpoDscuento").val("");
    $("#numVlorNtaCrdto").val("");
    $("#varObsrvciones").val("");

}

function LlenarComboTipoDcto() {

    $.ajax({
        url: ResolveUrl('~/tmvFacturasExportacionNotasCredito/GetTipoDescuento'),
        type: "POST",
        contentType: false,
        cache: false,
        dataType: "json",
        processData: false,
        success: function (data) {           
            var combo = "";
            data.forEach(function (tipoDcto) {
                combo += "<option value='" + tipoDcto.intIdTpoDscuento + "'>" + tipoDcto.varDscrpcionTpoDscuento + "</option>";
            });

            $("#intIdTpoDscuento").html(combo);
        }
    });
}