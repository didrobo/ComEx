var frmTmvDEXEncabezadoCrear = $("#frmTmvDEXEncabezadoCrear");
var btnGuardar = $("#btnGuardar");
var frmTmvDEXEncabezadoEditar = $("#frmTmvDEXEncabezadoEditar");
var btnEditar = $("#btnEditar");

var divFormCorrecciones = $("#divFormCorrecciones");
var divTablaCorrecciones = $("#divTablaCorrecciones");

$(document).on('ready', function () {
    crearTabla();

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });

    frmTmvDEXEncabezadoCrear_Submit();
    frmTmvDEXEncabezadoEditar_Submit();
});

function crearTabla() {

    configExportServer('GetDex', '#table-dex');

    tablaDex = $('#table-dex').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        serverSide: true,
        ajax: {
            type: "POST",
            url: 'GetDex',
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
                defaultContent: '<i class="buttonCrud btn_table_azul fa fa-book btnCorrecciones" style="cursor: pointer;" title="Correcciones"></i>'
            },
            { title: "intIdDEX", data: "intIdDEX", visible: false },
            { title: "Aux", data: "varCdAuxliar" },
            { title: "Fec. Aux", data: "fecAuxliar", render: dateJsonToString },
            { title: "DEX", data: "varNmroDEX" },
            { title: "Fec. DEX", data: "fecAprbcionDEX", render: dateJsonToString },
            { title: "Fec. Embarque", data: "fecEmbrque", render: dateJsonToString },
            { title: "Comprador", data: "varCmprdor" },
            { title: "Aduana", data: "varDscrpcionAduana" },
            {
                title: "Archivo", data: "varRtaArchvoAdjnto",
                render: function (ruta) {
                    if (ruta === null || ruta === '') {
                        return '';
                    }
                    else {
                        return '<a href="' + ruta + '" target="_blank">Ver PDF</a>';
                    }
                }
            },
            { title: "Formulario Anterior", data: "varNmroFrmlrioAntrior" },
            { title: "Vr. FOB", data: "numVlorFOB", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Fletes", data: "numFltes", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },            
            { title: "Seguro", data: "numSgro", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Otros Gastos", data: "numOtrosGstos", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Peso Bruto", data: "numTtalPsoBrto", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Peso Neto", data: "numTtalPsoNto", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Ttal. Series", data: "numTtalSries", render: $.fn.dataTable.render.number(',', '.', 0, ''), className: 'text-right' },
            { title: "Bultos", data: "numBltos", render: $.fn.dataTable.render.number(',', '.', 0, ''), className: 'text-right' },
            { title: "Declarante", data: "varDclrnte" },
            { title: "Destino Final", data: "varDtnoFinal" },
            { title: "Aceptación", data: "numNumroAcptcion" },
            { title: "Fec. Aceptación", data: "fecAcptcion", render: dateJsonToString },
            { title: "SAE", data: "varSlctudAtrzcionEmbrque" },
            { title: "Fec. SAE", data: "fecAtrzcionEmbrque", render: dateJsonToString },
            { title: "Manifiesto Carga", data: "varMnfiestoCrga" },
            { title: "Fec. Manifiesto", data: "fecMnfiestoCrga", render: dateJsonToString },
            { title: "Transporte", data: "varDscrpcionTrnsprte" },
            { title: "Plan CI", data: "varDscrpcionPlanCI" },
            { title: "Doc. Transporte", data: "varNmroDcmntoTrnsprte" },
            { title: "Fec. Doc. Transporte", data: "fecDcmntoTrnsprte", render: dateJsonToString },
            { title: "Comentaios", data: "varCmntrio" }
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
                    var intIdDEX = dt.row({ selected: true }).data().intIdDEX;
                    window.open(ResolveUrl("~/tmvDEXDetalle/Create/" + intIdDEX), '_self');
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditar"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdDEX = dt.row({ selected: true }).data().intIdDEX;
                    window.open(ResolveUrl("~/tmvDEXEncabezado/Edit/" + intIdDEX), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdDEX = dt.row({ selected: true }).data().intIdDEX;
                    alertify.confirm("Eliminar DEX", "¿Realmente desea eliminar el DEX seleccionado?", function () {
                        //Llamar método para eliminar
                        EliminarDEX(intIdDEX, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);

                                //Boton Nuevo Detalle
                                $("#btnNuevoDetalle").removeClass("disabled_control");
                                tablaDex.button(2).enable(true);

                                //Boton Editar
                                $("#btnEditar").removeClass("disabled_control");
                                tablaDex.button(3).enable(true);

                                //Boton Eliminar
                                $("#btnBorrar").removeClass("disabled_control");
                                tablaDex.button(4).enable(true);
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
            }],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: [[5, 'desc']],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();
            var json = api.ajax.json();

            $(api.column(13).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numVlorFOB));
            $(api.column(14).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numFltes));
            $(api.column(15).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numSgro));
            $(api.column(16).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numOtrosGstos));
            $(api.column(17).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numTtalPsoBrto));
            $(api.column(18).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numTtalPsoNto));
            $(api.column(19).footer()).html($.fn.dataTable.render.number(',', '.', 0, '').display(json.totals.numTtalSries));
            $(api.column(20).footer()).html($.fn.dataTable.render.number(',', '.', 0, '').display(json.totals.numBltos));            

        }
    });

    // Array to track the ids of the details displayed rows
    var detailRows = [];

    // Este es evento es para mostrar el detalle de la fila usando el botoón (+)
    $('#table-dex tbody').on('click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tablaDex.row(tr);
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
    tablaDex.on('draw', function () {
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger('click');
        });
    });

    $('#table-dex_filter input').unbind();
    $('#table-dex_filter input').bind('keyup', function (e) {
        if (e.keyCode === 13) {
            tablaDex.search(this.value).draw();
        }
    });

    tablaDex.on('select', function (e, dt, type, indexes) {
        //Boton Nuevo Detalle
        $("#btnNuevoDetalle").removeClass("disabled_control");
        tablaDex.button(2).enable(true);

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaDex.button(3).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaDex.button(4).enable(true);
    });

    tablaDex.on('deselect', function (e, dt, type, indexes) {
        //Boton Nuevo Detalle Compra
        $("#btnNuevoDetalle").addClass("disabled_control");
        tablaDex.button(2).enable(false);

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaDex.button(3).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaDex.button(4).enable(false);
    });

    tablaDex.on('click', '.btnCorrecciones', function (e) {
        var data = tablaDex.row($(this).parents('tr')).data();

        LimpiarCamposFormCorrecciones();
        divTablaCorrecciones.fadeIn();
        divFormCorrecciones.fadeOut();

        $('#modalCorrecciones').modal({
            backdrop: 'static',
            keyboard: false
        });

        $('#modalCorrecciones').on('shown.bs.modal', function () {
            cargarTablaCorrecciones(data.intIdDEX);
        });
    });

}

function getTblDetalle(d) {
    // la variable d contiene un objeto tmvDEXEncabezado
    return '<table id="table-detalle-dex-' + d.intIdDEX + '" class="table table-striped table-bordered table-hover display compact" style="border-spacing: 0px; border-collapse: separate; width: 20%;" />';
}

function drawTblDetalle(d) {
    var tablaDexDet = $('#table-detalle-dex-' + d.intIdDEX).DataTable({
        serverSide: true,
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmvDEXDetalle/GetDexDetalle/' + d.intIdDEX),
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
            { title: "intIdDEXDtlle", data: "intIdDEXDtlle", visible: false },
            { title: "intIdDEX", data: "intIdDEX", visible: false },
            { title: "Ítem", data: "varCdItem" },
            { title: "Producto", data: "varDscrpcionPrdcto" },
            { title: "Cnt. Exportada", data: "numCntdadExprtda", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Vr. FOB", data: "numVlorFOB", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "P. Bruto", data: "numPsoBrto", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "P. Neto", data: "numPsoNto", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { 
				title: "Comentarios", 
				data: "varCmntriosDEX",
                render: function (comentario) {
                    if (comentario !== undefined && comentario !== null) {
                        return comentario.substring(0, 200);
                    }
                    else {
                        return "";
                    }
                }
			}
        ]
    });

    $('#table-detalle-dex-' + d.intIdDEX + ' tbody').on('click', '.btnBorrarDetalle', function (e) {
        var data = tablaDexDet.row($(this).parents('tr')).data();
        alertify.confirm("Eliminar Detalle DEX", "¿Realmente desea eliminar el detalle del DEX seleccionado?", function () {
            //Llamar método para eliminar
            EliminarDexDetalle(data.intIdDEXDtlle, function (resultado) {
                tablaDexDet.row($(this)).remove().draw(false);
            });
            tablaDexDet.row($(this).parents('tr')).remove().draw(false);
        }, null);
    });

    $('#table-detalle-dex-' + d.intIdDEX + ' tbody').on('click', '.btnEditarDetalle', function (e) {
        var data = tablaDexDet.row($(this).parents('tr')).data();
        window.open(ResolveUrl("~/tmvDEXDetalle/Edit/" + data.intIdDEXDtlle), '_self');
    });
}

function frmTmvDEXEncabezadoCrear_Submit() {

    //Validar
    frmTmvDEXEncabezadoCrear.validate({
        rules: {
            varCdAuxliar: "required",
            fecAuxliar: "required",
            intIdImprtdorExprtdor: "required",
            intIdCmprdor: "required",
            numVlorFOB: "required",
            intIdPlanCI: "required",
            numTtalPsoNto: "required",
            numTtalPsoBrto: "required"
        },
        messages: {
            varCdAuxliar: "Asegúrese de ingresar el auxiliar",
            fecAuxliar: "Asegúrese de ingresar la fecha auxiliar",
            intIdImprtdorExprtdor: "Asegúrese de ingresar el exportador",
            intIdCmprdor: "Asegúrese de ingresar el comprador",
            numVlorFOB: "Asegúrese de ingresar el valor FOB",
            intIdPlanCI: "Asegúrese de ingresar el plan CI",
            numTtalPsoNto: "Asegúrese de ingresar el peso neto",
            numTtalPsoBrto: "Asegúrese de ingresar el peso bruto"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmvDEXEncabezadoCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });


            $.ajax({
                url: ResolveUrl('~/tmvDEXEncabezado/SetTmvDEXEncabezado'),
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

function EliminarDEX(intIdDEX, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmvDEXEncabezado/Delete'),
        type: "POST",
        data: { id: intIdDEX },
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

function EliminarDexDetalle(intIdDEXDtlle, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmvDEXDetalle/Delete'),
        type: "POST",
        data: { id: intIdDEXDtlle },
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

function frmTmvDEXEncabezadoEditar_Submit() {

    //Validar
    frmTmvDEXEncabezadoEditar.validate({
        rules: {
            varCdAuxliar: "required",
            fecAuxliar: "required",
            intIdImprtdorExprtdor: "required",
            intIdCmprdor: "required",
            numVlorFOB: "required",
            intIdPlanCI: "required",
            numTtalPsoNto: "required",
            numTtalPsoBrto: "required"
        },
        messages: {
            varCdAuxliar: "Asegúrese de ingresar el auxiliar",
            fecAuxliar: "Asegúrese de ingresar la fecha auxiliar",
            intIdImprtdorExprtdor: "Asegúrese de ingresar el exportador",
            intIdCmprdor: "Asegúrese de ingresar el comprador",
            numVlorFOB: "Asegúrese de ingresar el valor FOB",
            intIdPlanCI: "Asegúrese de ingresar el plan CI",
            numTtalPsoNto: "Asegúrese de ingresar el peso neto",
            numTtalPsoBrto: "Asegúrese de ingresar el peso bruto"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmvDEXEncabezadoEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });


            $.ajax({
                url: ResolveUrl('~/tmvDEXEncabezado/Update'),
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

function cargarTablaCorrecciones(intIdDEX) {

    $("#hfIntIdDEX").val(intIdDEX);

    if ($.fn.DataTable.isDataTable('#table-correcciones')) {
        $('#bodyModal').empty().append('<table class="table table-striped table-bordered table-hover display compact" id="table-correcciones" style="border-spacing: 0px; border-collapse: separate; width: 100%;"></table>');
    };

    var tablaCorrecciones = $('#table-correcciones').DataTable({
        dom: "<'row'<'col-md-12'B>><'row'<'col-md-12'tr>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmvDexCorreciones/GetCorreccionesDEX/' + intIdDEX),
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
            { title: "intIdDEXCorrcions", data: "intIdDEXCorrcions", visible: false },
            { title: "intIdDEX", data: "intIdDEX", visible: false },
            { title: "DEX", data: "varNmroDEX" },
            { title: "Fec. DEX", data: "fecAprbcionDEX", render: dateJsonToString },
            { title: "DEX Corregido", data: "varNmroDEXCrrgdo" },
            { title: "Fec. DEX Corregido", data: "fecAprbcionDEXCrrgdo", render: dateJsonToString },
            {
                title: "Archivo", data: "varRtaArchvoAdjnto",
                render: function (ruta) {
                    if (ruta === null || ruta === '') {
                        return '';
                    }
                    else {
                        return '<a href="' + ruta + '" target="_blank">Ver PDF</a>';
                    }
                }
            }
        ],
        buttons: [
            {
                text: '<i class="buttonCrud fa fa-file-o"></i>',
                titleAttr: 'Nuevo',
                action: function (e, dt, node, config) {

                    LimpiarCamposFormCorrecciones();

                    $("#hfOperacion").val("Nuevo");
                    $("#bitActva").attr('checked', 'checked')

                    divTablaCorrecciones.fadeOut();
                    divFormCorrecciones.fadeIn();
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditarCorreccion"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdDEXCorrcions = dt.row({ selected: true }).data().intIdDEXCorrcions;
                    var varNmroDEX = dt.row({ selected: true }).data().varNmroDEX;
                    var fecAprbcionDEX = dt.row({ selected: true }).data().fecAprbcionDEX;
                    var varNmroDEXCrrgdo = dt.row({ selected: true }).data().varNmroDEXCrrgdo;
                    var fecAprbcionDEXCrrgdo = dt.row({ selected: true }).data().fecAprbcionDEXCrrgdo;
                    var varRtaArchvoAdjnto = dt.row({ selected: true }).data().varRtaArchvoAdjnto;
                    

                    var find = '/';
                    var re = new RegExp(find, 'g');

                    $("#hfOperacion").val("Editar");
                    $("#hfIntIdDEXCorrcions").val(intIdDEXCorrcions);

                    $("#varNmroDEX").val(varNmroDEX);
                    $("#fecAprbcionDEX").val(dateJsonToString(fecAprbcionDEX).replace(re, "-"));
                    $("#varNmroDEXCrrgdo").val(varNmroDEXCrrgdo);
                    $("#fecAprbcionDEXCrrgdo").val(dateJsonToString(fecAprbcionDEXCrrgdo).replace(re, "-"));
                    $("#varRtaArchvoAdjnto").val(varRtaArchvoAdjnto);
                    

                    divTablaCorrecciones.fadeOut();
                    divFormCorrecciones.fadeIn();
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrarCorreccion"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdDEXCorrcions = dt.row({ selected: true }).data().intIdDEXCorrcions;
                    alertify.confirm("Eliminar Corrección", "¿Realmente desea eliminar la corrección seleccionada?", function () {
                        //Llamar método para eliminar
                        EliminarCorreccion(intIdDEXCorrcions, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditarCorreccion").addClass("disabled_control");
                                tablaCorrecciones.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrarCorreccion").addClass("disabled_control");
                                tablaCorrecciones.button(2).enable(false);
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

    tablaCorrecciones.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditarCorreccion").removeClass("disabled_control");
        tablaCorrecciones.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrarCorreccion").removeClass("disabled_control");
        tablaCorrecciones.button(2).enable(true);
    });

    tablaCorrecciones.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditarCorreccion").addClass("disabled_control");
        tablaCorrecciones.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrarCorreccion").addClass("disabled_control");
        tablaCorrecciones.button(2).enable(false);
    });
}

function EliminarCorreccion(intIdDEXCorrcions, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmvDexCorreciones/Delete'),
        type: "POST",
        data: { id: intIdDEXCorrcions },
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

//Botón cancelar modal correcciones
$("#btnCancelarCorrecc").click(function () {
    divTablaCorrecciones.fadeIn();
    divFormCorrecciones.fadeOut();
});

//Botón guardar modal correcciones
$("#btnGuardarCorrecc").click(function () {

    //Campos
    var intIdDEXCorrcions = $("#hfIntIdDEXCorrcions").val();
    var intIdDEX = $("#hfIntIdDEX").val();
    var varNmroDEX = $("#varNmroDEX").val();
    var varNmroDEXCrrgdo = $("#varNmroDEXCrrgdo").val();
    var varRtaArchvoAdjnto = $("#varRtaArchvoAdjnto").val();
    var fecAprbcionDEX = $("#fecAprbcionDEX").val();
    var fecAprbcionDEXCrrgdo = $("#fecAprbcionDEXCrrgdo").val();
    var operacion = $("#hfOperacion").val();

    var valido = validarFormCorrecciones();

    if (valido) {

        var formData = new FormData();

        formData.append("intIdDEXCorrcions", intIdDEXCorrcions);
        formData.append("intIdDEX", intIdDEX);
        formData.append("varNmroDEX", varNmroDEX);
        formData.append("varNmroDEXCrrgdo", varNmroDEXCrrgdo);
        formData.append("varRtaArchvoAdjnto", varRtaArchvoAdjnto);
        formData.append("fecAprbcionDEX", fecAprbcionDEX);
        formData.append("fecAprbcionDEXCrrgdo", fecAprbcionDEXCrrgdo);        
        formData.append("operacion", operacion);

        $.ajax({
            url: ResolveUrl('~/tmvDexCorreciones/CUCorreccion'),
            type: "POST",
            contentType: false,
            cache: false,
            dataType: "json",
            processData: false,
            data: formData,
            beforeSend: function () {
                console.info("Enviando infromación...");
                $("#btnGuardarCorrecc").button('loading');
            },
            success: function (data) {
                if (data.Success) {
                    //Limpiar el botón
                    $("#btnGuardarCorrecc").button('reset');

                    LimpiarCamposFormCorrecciones();
                    cargarTablaCorrecciones(intIdDEX);

                    divTablaCorrecciones.fadeIn();
                    divFormCorrecciones.fadeOut();
                }
                else {
                    console.error(data.Message);

                    $("#messageCorreccion").html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                        + resultado.mensaje + '</div>')
                        .fadeIn(100);

                    //Limpiar el botón
                    $("#btnGuardarCorrecc").button('reset');
                }
            }
        });
    }
});

function validarFormCorrecciones() {

    var varNmroDEX = $("#varNmroDEX");
    var fecAprbcionDEX = $("#fecAprbcionDEX");
    var varNmroDEXCrrgdo = $("#varNmroDEXCrrgdo");
    var fecAprbcionDEXCrrgdo = $("#fecAprbcionDEXCrrgdo");

    //Validaciones
    var valid_varNmroDEX = $("#valid_varNmroDEX");
    var valid_fecAprbcionDEX = $("#valid_fecAprbcionDEX");
    var valid_varNmroDEXCrrgdo = $("#valid_varNmroDEXCrrgdo");
    var valid_fecAprbcionDEXCrrgdo = $("#valid_fecAprbcionDEXCrrgdo");

    var valido = true;

    //Se valida el DEX
    if (varNmroDEX.val() !== "") {
        //Se remueven las propiedades de error 
        varNmroDEX.parent().removeClass("has-error");
        valid_varNmroDEX.empty();
        valido = true;
    }
    else {
        varNmroDEX.parent().addClass("has-error");
        valid_varNmroDEX.empty().append("Requerido");
        $("#btnGuardarCorrecc").button('reset');
        valido = false;
    }

    //Se valida el campo fecha de dex
    if (fecAprbcionDEX.val() !== "") {
        //Se remueven las propiedades de error 
        fecAprbcionDEX.parent().removeClass("has-error");
        valid_fecAprbcionDEX.empty();
        valido = true;
    }
    else {
        fecAprbcionDEX.parent().addClass("has-error");
        valid_fecAprbcionDEX.empty().append("Requerido");
        $("#btnGuardarCorrecc").button('reset');
        valido = false;
    }

    //Se valida el dex corregido
    if (varNmroDEXCrrgdo.val() !== "") {
        //Se remueven las propiedades de error 
        varNmroDEXCrrgdo.parent().removeClass("has-error");
        valid_varNmroDEXCrrgdo.empty();
        valido = true;
    }
    else {
        varNmroDEXCrrgdo.parent().addClass("has-error");
        valid_varNmroDEXCrrgdo.empty().append("Requerido");
        $("#btnGuardarNotaCred").button('reset');
        valido = false;
    }

    //Se valida el campo fecha de dex corregido
    if (fecAprbcionDEXCrrgdo.val() !== "") {
        //Se remueven las propiedades de error 
        fecAprbcionDEXCrrgdo.parent().removeClass("has-error");
        valid_fecAprbcionDEXCrrgdo.empty();
        valido = true;
    }
    else {
        fecAprbcionDEXCrrgdo.parent().addClass("has-error");
        valid_fecAprbcionDEXCrrgdo.empty().append("Requerido");
        $("#btnGuardarCorrecc").button('reset');
        valido = false;
    }

    return valido;
}

function LimpiarCamposFormCorrecciones() {

    $("#varNmroDEX").val("");
    $("#fecAprbcionDEX").val("");
    $("#varNmroDEXCrrgdo").val("");
    $("#fecAprbcionDEXCrrgdo").val("");
    $("#varRtaArchvoAdjnto").val("");

}