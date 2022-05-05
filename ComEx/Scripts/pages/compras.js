var cbProveedor = $("#intIdPrvdor");
var cbResolucion = $("#intIdRslcion");
var cbTituloMinero = $("#intIdTtloMnro");
var frmTmComprasCrear = $("#frmTmComprasCrear");
var btnGuardar = $("#btnGuardar");
var frmTmComprasEditar = $("#frmTmComprasEditar");
var btnEditar = $("#btnEditar");

$(document).on('ready', function () {
    crearTabla();

    cargarComboProveedor();

    //Eventos
    cbProveedor.on("change", intIdPrvdor_Change);
    

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });

    //Ejecutar método
    cbProveedor.change();
    frmTmComprasCrear_Submit();
    frmTmComprasEditar_Submit();
});

function crearTabla() {

    configExportServer('GetCompras', '#table-compras');
    
    tablaCompras = $('#table-compras').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        serverSide: true,
        ajax: {
            type: "POST",
            url: 'GetCompras',
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
        scrollY: 400,
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
            { title: "intIdCmpra", data: "intIdCmpra", visible: false },
            { title: "Lote", data: "varNumLte" },
            { title: "Cargue", data: "intCnsctvoCrgue" },
            { title: "Aux", data: "varCdAuxliar" },
            { title: "Factura", data: "varNmroFctra" },
            { title: "Fec. Factura", data: "fecCmpra", render: dateJsonToString },
            { title: "Proyecto", data: "varPrycto" },
            { title: "Llave", data: "varLlve" },
            { title: "intIdPrvdor", data: "intIdPrvdor", visible: false },
            { title: "Cd. CP", data: "varCdCP" },
            { title: "Fec. CP", data: "fecCp", render: dateJsonToString },
            { title: "Nmro. CP", data: "varNmroCP" },
            { title: "F. Aprb. CP", data: "fecAprbcionCP", render: dateJsonToString },
            { title: "Documento", data: "varNitPrvdor" },
            { title: "Proveedor", data: "varNmbre" },
            { title: "Peso Bruto", data: "numBrtos", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Finos", data: "numFnos", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Vr. Total", data: "numVlorTtal", render: $.fn.dataTable.render.number(',', '.', 0, '$'), className: 'text-right' },
            { title: "Vr. Iva", data: "numVlorIva", render: $.fn.dataTable.render.number(',', '.', 0, '$'), className: 'text-right' },
            { title: "Vr. Regalías", data: "numVlorRglias", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Regalías", data: "varRegalias" },
            { title: "Ciudad Regalías", data: "varDscrpcionCiudadRglias" },
            { title: "Dpto. Regalías", data: "varDscrpcionDprtmntoRglias" },
            { title: "intIdRtncionfuente", data: "intIdRtncionfuente", visible: false },                      
            {
                title: "Anulada",
                data: "bitAnldo",
                render: function (Anulada) {
                    if (Anulada) {
                        return "Si";
                    }
                    else {
                        return "No";
                    }
                }
            },
            { title: "intIdCP", data: "intIdCP", visible: false },
            { title: "numVlorRtncionFuente", data: "numVlorRtncionFuente", render: $.fn.dataTable.render.number(',', '.', 0, '$'), visible: false, className: 'text-right' },
            { title: "numPrcntjeRtncionFte", data: "numPrcntjeRtncionFte", render: $.fn.dataTable.render.number(',', '.', 2, ''), visible: false, className: 'text-right' },
            { title: "intIdAdquirdaTpoCmpra", data: "intIdAdquirdaTpoCmpra", visible: false },
            { title: "Adquiridas Tipo Compra", data: "varDscrpcionAdquirdaTpoCmpra", visible: false },
            { title: "intIdRslcion", data: "intIdRslcion", visible: false },
            { title: "Resolución Fact", data: "varNmroRslcion" },            
            { title: "intIdCiudadRglias", data: "intIdCiudadRglias", visible: false },                    
            { title: "intIdRglia", data: "intIdRglia", visible: false },    
            { title: "intIdTtloMnro", data: "intIdTtloMnro", visible: false },
            { title: "Placa", data: "varPlcaTtloMnro" },
            {
                title: "Acgdo Ley 1429",
                data: "bitAcgdosLey1429",
                render: function (Ley) {
                    if (Ley) {
                        return "Si";
                    }
                    else {
                        return "No";
                    }
                }
            },
            { title: "Tipo Compra", data: "varDscrpcionTpoCmpra" }
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
                titleAttr: 'Nuevo Detalle Compra',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdCmpra = dt.row({ selected: true }).data().intIdCmpra;
                    window.open(ResolveUrl("~/tmComprasDetalle/Create/" + intIdCmpra), '_self');
                }
            },
             {
                 text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditar"></i>',
                 titleAttr: 'Editar',
                 enabled: false,
                 action: function (e, dt, node, config) {
                     var intIdCmpra = dt.row({ selected: true }).data().intIdCmpra;
                     window.open(ResolveUrl("~/tmCompras/Edit/" + intIdCmpra), '_self');
                 }
             },
             {
                 text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                 titleAttr: 'Borrar',
                 enabled: false,
                 action: function (e, dt, node, config) {
                     var intIdCmpra = dt.row({ selected: true }).data().intIdCmpra;
                     alertify.confirm("Eliminar Orden de Compra", "¿Realmente desea eliminar la orden de compra seleccionada?", function () {
                         //Llamar método para eliminar
                         EliminarCompra(intIdCmpra, function (resultado) {
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
             },
             {
                 text: '<i id="icn_borrarCargue" class="buttonCrud fa fa-eraser"></i>',
                 titleAttr: 'Borrar Cargue',
                 action: function (e, dt, node, config) {
                     borrarCargue();
                 }
             },
             {
                 text: '<i id="icn_verReporte" class="buttonCrud fa fa-file-pdf-o"></i>',
                 titleAttr: 'Ver Formatos',
                 action: function (e, dt, node, config) {                     
                     VerFormatos();
                 }
            },
            {
                text: '<i id="icn_print" class="buttonCrud fa fa-print"></i>',
                titleAttr: 'Impresión Masiva de Documentos',
                action: function (e, dt, node, config) {
                    ImprimirDocs();
                }
            },
            {
                text: '<i id="icn_cambiarLote" class="buttonCrud fa fa-exchange"></i>',
                titleAttr: 'Cambiar Lote',
                action: function (e, dt, node, config) {
                    cambiarLote();
                }
            },
            {
                text: '<i id="icn_cambiarFecAux" class="buttonCrud fa fa-calendar"></i>',
                titleAttr: 'Cambiar Fechas Aux Cargue',
                action: function (e, dt, node, config) {
                    cambiarFecAuxCargue();
                }
            }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: [[4, 'desc']],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();
            var json = api.ajax.json();

            $(api.column(17).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numBrtos));
            $(api.column(18).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numFnos));
            $(api.column(19).footer()).html($.fn.dataTable.render.number(',', '.', 0, '$').display(json.totals.numVlorTtal));
            $(api.column(20).footer()).html($.fn.dataTable.render.number(',', '.', 0, '$').display(json.totals.numVlorIva));
            $(api.column(21).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numVlorRglias));


        }
    });

    // Array to track the ids of the details displayed rows
    var detailRows = [];

    // Este es evento es para mostrar el detalle de la fila usando el botoón (+)
    $('#table-compras tbody').on('click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tablaCompras.row(tr);
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
    tablaCompras.on('draw', function () {
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger('click');
        });
    });

    $('#table-compras_filter input').unbind();
    $('#table-compras_filter input').bind('keyup', function (e) {
        if (e.keyCode === 13) {
            tablaCompras.search(this.value).draw();
        }
    });

    tablaCompras.on('select', function (e, dt, type, indexes) {
        //Boton Nuevo Detalle Compra
        $("#btnNuevoDetalle").removeClass("disabled_control");
        tablaCompras.button(2).enable(true);

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaCompras.button(3).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaCompras.button(4).enable(true);
    });

    tablaCompras.on('deselect', function (e, dt, type, indexes) {
        //Boton Nuevo Detalle Compra
        $("#btnNuevoDetalle").addClass("disabled_control");
        tablaCompras.button(2).enable(false);

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaCompras.button(3).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaCompras.button(4).enable(false);
    });

}

function getTblDetalle(d) {
    // la variable d contiene un objeto tmCompras
    return '<table id="table-detalle-compras-' + d.intIdCmpra + '" class="table table-striped table-bordered table-hover display compact" style="border-spacing: 0px; border-collapse: separate; width: 20%;" />';
}

function drawTblDetalle(d) {
    var tablaCompraDet = $('#table-detalle-compras-' + d.intIdCmpra).DataTable({
        serverSide: true,
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmComprasDetalle/GetComprasDetalle/' + d.intIdCmpra),
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
            { title: "intIdCmpraDtlle", data: "intIdCmpraDtlle", visible: false },
            { title: "intIdCmpra", data: "intIdCmpra", visible: false },
            { title: "Ítem", data: "intCdItem" },
            { title: "intIdInsmo", data: "intIdInsmo", visible: false },
            { title: "CI", data: "varCdInsmo" },
            { title: "Insumo", data: "varDscrpcionInsmo" },
            { title: "Peso Bruto", data: "numCntdadFctra", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Finos", data: "numFnos", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Cnt. CP", data: "numCntdadCP", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
            { title: "Vr. Unitario", data: "numVlorUntrio", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Vr. Total", data: "numSbttal", render: $.fn.dataTable.render.number(',', '.', 0, '$'), className: 'text-right' },
            { title: "Vr. Regalías", data: "numRglias", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Regalías", data: "varRegalias" },
            { title: "Ley", data: "numLey", render: $.fn.dataTable.render.number(',', '.', 0, ''), className: 'text-right' },
            { title: "% Iva", data: "numPrcntjeIva", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "% Minería", data: "numPorcMnria", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Adquirida", data: "varDscrpcionAdquirda" },
            { title: "Cnt. Acum. Devol.", data: "numCantAcumNotasCrdto", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
            { title: "Vr. Acum. Devol.", data: "numVlorAcumNotasCrdto", render: $.fn.dataTable.render.number(',', '.', 0, '$'), className: 'text-right' }
        ]
    });

    $('#table-detalle-compras-' + d.intIdCmpra + ' tbody').on('click', '.btnBorrarDetalle', function (e) {
        var data = tablaCompraDet.row($(this).parents('tr')).data();
        alertify.confirm("Eliminar Detalle Orden de Compra", "¿Realmente desea eliminar el detalle de la orden de compra seleccionada?", function () {
            //Llamar método para eliminar
            EliminarCompraDetalle(data.intIdCmpraDtlle, function (resultado) {
                tablaCompraDet.row($(this)).remove().draw(false);
            });
            tablaCompraDet.row($(this).parents('tr')).remove().draw(false);
        }, null);
    });

    $('#table-detalle-compras-' + d.intIdCmpra + ' tbody').on('click', '.btnEditarDetalle', function (e) {
        var data = tablaCompraDet.row($(this).parents('tr')).data();
        window.open(ResolveUrl("~/tmComprasDetalle/Edit/" + data.intIdCmpraDtlle), '_self');
    });
}

//Función para recuperar las resoluciones y titulo minero cuando se seleccione un Proveedor
function intIdPrvdor_Change()
{
    $.ajax({
        type: "GET",
        url: ResolveUrl('~/tmCompras/GetDatosProveedor?IdProveedor=' + cbProveedor.val()),
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            console.info("Enviando infromación...");
        },
        success: function (data) {
            if (data.Success)
            {
                if (data.lstResolucionProveedor.length > 0)
                {
                    //Llenar con la resolución
                    $.each(data.lstResolucionProveedor, function (index, value) {
                        cbResolucion.append('<option value="' + value.intIdRslcion + '">' + value.NmroRslcion + '</option>');
                    });

                    //Llenar con el titulo minero
                    $.each(data.lstTituloMinero, function (index, value) {
                        cbTituloMinero.append('<option value="' + value.intIdTtloMnro + '">' + value.varPlca + '</option>');
                    });

                    //Habilitar el ComboBox de Resolución
                    cbResolucion.prop("disabled", false);
                    cbResolucion.selectpicker('refresh');

                    //Habilitar el ComboBox de Titulo Minero
                    cbTituloMinero.prop("disabled", false);
                    cbTituloMinero.selectpicker('refresh');
                }
                else
                {
                    //Desabilitar el ComboBox de Resolución
                    DesabilitarResolucion();

                    //Desabilitar el ComboBox de Titulo Minero
                    DesabilitarTituloMinero();
                }
            }
            else
            {
                console.error(data.Message);
                //Desabilitar el ComboBox de Resolución
                DesabilitarResolucion();

                //Desabilitar el ComboBox de Titulo Minero
                DesabilitarTituloMinero();
            }
        }
    });
}

//Desabilitar el ComboBox de Resolución
function DesabilitarResolucion()
{
    cbResolucion.prop("disabled", true);
    cbResolucion.empty();
    cbResolucion.append('<option value="">Seleccionar...</option>');
    cbResolucion.selectpicker('refresh');
}

//Desabilitar el ComboBox de Titulo Minero
function DesabilitarTituloMinero()
{
    cbTituloMinero.prop("disabled", true);
    cbTituloMinero.empty();
    cbTituloMinero.append('<option value="">Seleccionar...</option>');
    cbTituloMinero.selectpicker('refresh');
}

function frmTmComprasCrear_Submit()
{
    //Validar
    frmTmComprasCrear.validate({
        rules: {
            varCdAuxliar: "required",
            varNmroFctra: "required",
            fecCmpra: "required",
            intIdPrvdor: "required",
            intCnsctvoCrgue: "required",
            varNumLte: "required",
            intIdCiudadRglias: "required",
            intIdRglia: "required"
        },
        messages: {
            varCdAuxliar: "Asegúrese de ingresar el auxiliar.",
            varNmroFctra: "Asegúrese de ingresar el número de la factura.",
            fecCmpra: "Asegúrese de ingresar la fecha de compra.",
            intIdPrvdor: "Asegúrese de seleccionar aún proveedor.",
            intCnsctvoCrgue: "Asegúrese de ingresar el cargue.",
            varNumLte: "Asegúrese de ingresar el lote.",
            intIdCiudadRglias: "Asegúrese de seleccionar la ciudad.",
            intIdRglia: "Asegúrese de seleccionar una regalía."
        },
        submitHandler: function (Frm)
        {
            var formData = new FormData();
            $.each(frmTmComprasCrear.serializeArray(), function (index, value) {
                if (value.name !== "bitAcgdosLey1429" && value.name !== "bitAnldo") {
                    formData.append(value.name, value.value);
                }
            });

            var bitAcgdosLey1429 = $("#bitAcgdosLey1429").is(":checked") === true ? true : false;
            var bitAnldo = $("#bitAnldo").is(":checked") === true ? true : false;

            formData.append("bitAcgdosLey1429", bitAcgdosLey1429);
            formData.append("bitAnldo", bitAnldo);
            $.ajax({
                url: ResolveUrl('~/tmCompras/SetTmCompras'),
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
                    
                },
                //error: function (jqXHR) {
                //    mensaje.html('<div class="alert alert-danger" role="alert">' + jqXHR.statusText + '</div>')
                //        .fadeIn(1000);
                //}
            });
        }
    });
}

function EliminarCompra(intIdCmpra, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmCompras/Delete'),
        type: "POST",
        data: { id: intIdCmpra },
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

function EliminarCompraDetalle(intIdCmpraDtlle, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmComprasDetalle/Delete'),
        type: "POST",
        data: { id: intIdCmpraDtlle },
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

function frmTmComprasEditar_Submit() {
    //Validar
    frmTmComprasEditar.validate({
        rules: {
            varCdAuxliar: "required",
            varNmroFctra: "required",
            fecCmpra: "required",
            intIdPrvdor: "required",
            intCnsctvoCrgue: "required",
            varNumLte: "required",
            intIdCiudadRglias: "required",
            intIdRglia: "required",
        },
        messages: {
            varCdAuxliar: "Asegúrese de ingresar el auxiliar.",
            varNmroFctra: "Asegúrese de ingresar el número de la factura.",
            fecCmpra: "Asegúrese de ingresar la fecha de compra.",
            intIdPrvdor: "Asegúrese de seleccionar aún proveedor.",
            intCnsctvoCrgue: "Asegúrese de ingresar el cargue.",
            varNumLte: "Asegúrese de ingresar el lote.",
            intIdCiudadRglias: "Asegúrese de seleccionar la ciudad.",
            intIdRglia: "Asegúrese de seleccionar una regalía."
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmComprasEditar.serializeArray(), function (index, value) {
                if (value.name !== "bitAcgdosLey1429" && value.name !== "bitAnldo") {
                    formData.append(value.name, value.value);
                }
            });

            var bitAcgdosLey1429 = $("#bitAcgdosLey1429").is(":checked") === true ? true : false;
            var bitAnldo = $("#bitAnldo").is(":checked") === true ? true : false;

            formData.append("bitAcgdosLey1429", bitAcgdosLey1429);
            formData.append("bitAnldo", bitAnldo);

            $.ajax({
                url: ResolveUrl('~/tmCompras/Update'),
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

function cargarComboProveedor() {
    $(".select2_intIdPrvdor").select2({
        language: "es",
        ajax: {
            url: ResolveUrl('~/tmProveedor/GetProveedorByNameOrNit'),
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
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.varNitPrvdor + ' | ' + item.varNmbre,
                            id: item.intIdPrvdor
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

//Fución para eliminar cargues 
function borrarCargue() {

    // Mostrar Modal
    showBSModal({
        title: "Eliminar Cargue",
        body: '<div class="row"><div class="col-md-12"><div class="form-group"><label>Cargue</label><select class="select2_intIdCrgue form-control"></select><span class="field-validation-valid text-danger" id="val_message"></span></div></div></div>',
        size: "small",
        actions: [
            {
                label: 'Eliminar',
                cssClass: 'btn-success btn_borrar_cargue',
                onClick: function (e) {
                    ejecutarBorrarCargue();
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
            cargarComboCargue();
        }
    });
}

function cargarComboCargue() {
    $(".select2_intIdCrgue").select2({
        language: "es",
        placeholder: "Seleccionar cargue...",
        allowClear: true,
        ajax: {
            url: ResolveUrl('~/tmCompras/GetListCargues'),
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
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: "Cargue: " + item.cargue + " # Compras: " + item.cantCmpras,
                            id: item.cargue
                        };
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 3
    });
}

//Función que ejecuta la operación de borrar cargue
function ejecutarBorrarCargue() {
    
    var intCnsctvoCrgue = $(".select2_intIdCrgue").val();

    if (intCnsctvoCrgue !== "" && intCnsctvoCrgue !== null) {
        $("#val_message").empty();
        
        var parameter = { "intCnsctvoCrgue": intCnsctvoCrgue };
        $.ajax({
            type: 'POST',
            url: ResolveUrl("~/tmCompras/BorrarCargue"),
            data: JSON.stringify(parameter),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            cache: false,
            beforeSend: function () {
                console.info("Enviando infromación...");
                $(".btn_borrar_cargue").button('loading');
            },
            success: function (resultado) {
                if (resultado.success) {
                    tablaCompras.draw();
                    $(".select2_intIdCrgue").parents('.modal').modal('hide');

                    pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                        + resultado.mensaje + '</div>')
                        .fadeIn(100);

                    //Limpiar el botón
                    $(".btn_borrar_cargue").button('reset');
                }
                else {
                    $(".select2_intIdCrgue").parents('.modal').modal('hide');
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                        .fadeIn(100);

                    //Limpiar el botón
                    $(".btn_borrar_cargue").button('reset');
                }
            },
            error: function (data, success, error) {
                $(".select2_intIdCrgue").parents('.modal').modal('hide');
                pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                    .fadeIn(100);

                //Limpiar el botón
                $(".btn_borrar_cargue").button('reset');
            }
        });
    }
    else {
        $("#val_message").empty().append("Seleccione un cargue");
    }
}

function VerFormatos() {
    // Mostrar Modal
    showBSModal({
        title: "Seleccionar Formato",
        body: '<div class="row"><div class="col-md-12"><div class="form-group"><label>Formato</label><select class="select2_formato form-control"><option value="OrdenesCompra">Ordenes de Compra</option><option value="DocEquivalente">Documento Equivalente</option><option value="DocEquivalenteNuevo">Documento Equivalente Nuevo</option><option value="RemisionCP">Remisiones de CPs</option></select><span class="field-validation-valid text-danger" id="val_message"></span></div></div></div>',
        size: "small",
        actions: [
            {
                label: 'Ver Formato',
                cssClass: 'btn-success btn_ver_formato',
                onClick: function (e) {
                    abrirFormato();
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
            cargarComboFormatos();
        }
    });
}

function cargarComboFormatos() {
    $(".select2_formato").select2({
        language: "es",
        placeholder: "Seleccionar formato...",
        allowClear: true
    });
}

function abrirFormato() {

    var formato = $(".select2_formato").val();

    switch (formato) {
        case "OrdenesCompra":
            GenerarFormatoOC();
            break;
        case "DocEquivalente":
            GenerarFormatoDocEquivalente();
            break;
        case "DocEquivalenteNuevo":
            GenerarFormatoDocEquivalenteNuevo();
            break;
        case "RemisionCP":
            GenerarFormatoRemisionCP();
            break;
    }
}

function GenerarFormatoOC() {
    $.ajax({
        url: ResolveUrl('~/OperacionesMasivas/SetSessionFilter'),
        type: "POST",
        data: JSON.stringify({ listFiltros: listFiltrosDataTable }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var newWindow = window.open('Reporte', '_blank');
            }
        }
    });
}

function GenerarFormatoDocEquivalente() {
    $.ajax({
        url: ResolveUrl('~/OperacionesMasivas/SetSessionFilter'),
        type: "POST",
        data: JSON.stringify({ listFiltros: listFiltrosDataTable }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var newWindow = window.open('FormatoDocEquivalente', '_blank');
            }
        }
    });
}

function GenerarFormatoDocEquivalenteNuevo() {
    $.ajax({
        url: ResolveUrl('~/OperacionesMasivas/SetSessionFilter'),
        type: "POST",
        data: JSON.stringify({ listFiltros: listFiltrosDataTable }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var newWindow = window.open('FormatoDocEquivalenteNuevo', '_blank');
            }
        }
    });
}

function GenerarFormatoRemisionCP() {
    $.ajax({
        url: ResolveUrl('~/OperacionesMasivas/SetSessionFilter'),
        type: "POST",
        data: JSON.stringify({ listFiltros: listFiltrosDataTable }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var newWindow = window.open('FormatoRemisionCP', '_blank');
            }
        }
    });
}

function ImprimirDocs() {
    $.ajax({
        url: ResolveUrl('~/OperacionesMasivas/SetSessionFilter'),
        type: "POST",
        data: JSON.stringify({ listFiltros: listFiltrosDataTable }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        success: function (data) {
            if (data.success) {
                var newWindow = window.open(ResolveUrl('~/OperacionesMasivas/ImpresionDocumentos/compras'), '_blank');
            }
        }
    });
}

// Función para cambiar Lotes
function cambiarLote() {

    // Mostrar Modal
    showBSModal({
        title: "Cambiar Lote",
        body: '<div class="row"><div class="col-md-12"><div class="form-group"><label>Lote Anterior</label><select class="select2_varNumLte form-control"></select><span class="field-validation-valid text-danger" id="val_message"></span></div><div class="form-group"><label>Lote Nuevo</label><input id="varNumLte_nuevo" class="form-control" /><span class="field-validation-valid text-danger" id="val_message_nuevoLote"></span></div></div></div>',
        size: "small",
        actions: [
            {
                label: 'Cambiar',
                cssClass: 'btn-success btn_cambiar_lote',
                onClick: function (e) {
                    ejecutarCambiarLote();
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
            cargarComboLote();
        }
    });
}

function cargarComboLote() {
    $(".select2_varNumLte").select2({
        language: "es",
        placeholder: "Seleccionar lote...",
        allowClear: true,
        ajax: {
            url: ResolveUrl('~/tmCompras/GetListLotes'),
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
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: "Lote: " + item.lote + " # Compras: " + item.cantCmpras,
                            id: item.lote
                        };
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 3
    });
}

//Función que ejecuta la operación de cambiar Lote
function ejecutarCambiarLote() {

    var varNumLte = $(".select2_varNumLte").val();
    var varNumLte_nuevo = $("#varNumLte_nuevo").val();

    if (varNumLte !== "" && varNumLte !== null) {
        $("#val_message").empty();

        if (varNumLte_nuevo !== "" && varNumLte_nuevo !== null) {

            $("#val_message_nuevoLote").empty();

            var parameter = { "varNumLte": varNumLte, "varNumLte_nuevo": varNumLte_nuevo };
            $.ajax({
                type: 'POST',
                url: ResolveUrl("~/tmCompras/CambiarLote"),
                data: JSON.stringify(parameter),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                cache: false,
                beforeSend: function () {
                    console.info("Enviando infromación...");
                    $(".btn_cambiar_lote").button('loading');
                },
                success: function (resultado) {
                    if (resultado.success) {
                        tablaCompras.draw();
                        $(".select2_varNumLte").parents('.modal').modal('hide');

                        pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                            + resultado.mensaje + '</div>')
                            .fadeIn(100);

                        //Limpiar el botón
                        $(".btn_cambiar_lote").button('reset');
                    }
                    else {
                        $(".select2_varNumLte").parents('.modal').modal('hide');
                        pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                            .fadeIn(100);

                        //Limpiar el botón
                        $(".btn_cambiar_lote").button('reset');
                    }
                },
                error: function (data, success, error) {
                    $(".select2_varNumLte").parents('.modal').modal('hide');
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                        .fadeIn(100);

                    //Limpiar el botón
                    $(".btn_cambiar_lote").button('reset');
                }
            });
        }
        else {
            $("#val_message_nuevoLote").empty().append("Digite el nuevo lote");
        }
    }
    else {
        $("#val_message").empty().append("Seleccione un lote");
    }
}

// Función para cambiar la fec auxiliar de los cargues
function cambiarFecAuxCargue() {

    // Mostrar Modal
    showBSModal({
        title: "Cambiar Fecha Aux. Cargue",
        body: '<div class="row"><div class="col-md-12"><div class="form-group"><label>Cargue</label><select class="select2_intIdCrgue form-control"></select><span class="field-validation-valid text-danger" id="val_message"></span></div><div class="form-group"><label>Fecha Aux.</label><input id="fecAux" class="form-control calendario" type="datetime" /><span class="field-validation-valid text-danger" id="val_message_fecAux"></span></div></div></div>',
        size: "small",
        actions: [
            {
                label: 'Cambiar',
                cssClass: 'btn-success btn_cambiar_fec_aux',
                onClick: function (e) {
                    ejecutarCambiarFecAuxCargue();
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

            $("#fecAux").datetimepicker({
                locale: "es",
                format: "DD/MM/YYYY"
            });

            cargarComboCargue();
        }
    });
}

//Función que ejecuta la operación de cambiar fecha aux de un cargue
function ejecutarCambiarFecAuxCargue() {

    var intIdCrgue = $(".select2_intIdCrgue").val();
    var fecAux = $("#fecAux").val();

    if (intIdCrgue !== "" && intIdCrgue !== null) {
        $("#val_message").empty();

        if (fecAux !== "" && fecAux !== null) {

            $("#val_message_fecAux").empty();

            var parameter = { "intIdCrgue": intIdCrgue, "fecAux": fecAux };
            $.ajax({
                type: 'POST',
                url: ResolveUrl("~/tmCompras/CambiarFecAuxCargue"),
                data: JSON.stringify(parameter),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                cache: false,
                beforeSend: function () {
                    console.info("Enviando infromación...");
                    $(".btn_cambiar_fec_aux").button('loading');
                },
                success: function (resultado) {
                    if (resultado.success) {
                        tablaCompras.draw();
                        $(".select2_intIdCrgue").parents('.modal').modal('hide');

                        pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                            + resultado.mensaje + '</div>')
                            .fadeIn(100);

                        //Limpiar el botón
                        $(".btn_cambiar_fec_aux").button('reset');
                    }
                    else {
                        $(".select2_intIdCrgue").parents('.modal').modal('hide');
                        pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                            .fadeIn(100);

                        //Limpiar el botón
                        $(".btn_cambiar_fec_aux").button('reset');
                    }
                },
                error: function (data, success, error) {
                    $(".select2_intIdCrgue").parents('.modal').modal('hide');
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
                        .fadeIn(100);

                    //Limpiar el botón
                    $(".btn_cambiar_fec_aux").button('reset');
                }
            });
        }
        else {
            $("#val_message_fecAux").empty().append("Seleccione la fecha auxiliar");
        }
    }
    else {
        $("#val_message").empty().append("Seleccione un cargue");
    }
}