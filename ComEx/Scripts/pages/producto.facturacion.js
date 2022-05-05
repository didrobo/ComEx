var frmTmvProductosFacturacionCrear = $("#frmTmvProductosFacturacionCrear");
var frmTmvProductosFacturacionEditar = $("#frmTmvProductosFacturacionEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");

$(document).on('ready', function () {
    crearTabla();

    frmTmvProductosFacturacionCrear_Submit();
    frmTmvProductosFacturacionEditar_Submit();
});

function crearTabla() {

    tablaProductoFacturacion = $('#table-producto-facturacion').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmvProductosFacturacion/GetProductoFacturacion'),
            contentType: 'application/json; charset=utf-8',
            data: function (data) {
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
            { title: "intIdPrdctoFctrcion", data: "intIdPrdctoFctrcion", visible: false },
            { title: "Código", data: "varCdPrdcto" },
            { title: "Descripción", data: "varDscrpcionEspñol" },
            { title: "P. Arancelaria", data: "tmPosicionesArancelarias.varPscionArnclria" },
            { title: "Unidad", data: "tmUnidades.varCdUndad" },
            { title: "Tipo Material", data: "tmLineas.varDscrpcionLnea" }
        ],
        buttons: [
            {
                text: '<i class="buttonCrud fa fa-file-o"></i>',
                titleAttr: 'Nuevo',
                action: function (e, dt, node, config) {
                    window.open('Create', '_self');
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditar"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdPrdctoFctrcion = dt.row({ selected: true }).data().intIdPrdctoFctrcion;
                    window.open(ResolveUrl("~/tmvProductosFacturacion/Edit/" + intIdPrdctoFctrcion), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdPrdctoFctrcion = dt.row({ selected: true }).data().intIdPrdctoFctrcion;
                    alertify.confirm("Eliminar Producto de Facturación", "¿Realmente desea eliminar el producto de facturación seleccionado?", function () {
                        //Llamar método para eliminar
                        EliminarProductoFacturacion(intIdPrdctoFctrcion, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaProductoFacturacion.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaProductoFacturacion.button(2).enable(false);
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
            }
        ],
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: []
    });

    tablaProductoFacturacion.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaProductoFacturacion.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaProductoFacturacion.button(2).enable(true);
    });

    tablaProductoFacturacion.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaProductoFacturacion.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaProductoFacturacion.button(2).enable(false);
    });
}

function frmTmvProductosFacturacionCrear_Submit() {

    //Validar
    frmTmvProductosFacturacionCrear.validate({
        rules: {
            varCdPrdcto: "required",
            varDscrpcionEspñol: "required",
            intIdLnea: "required",
            intIdPscionArnclria: "required",
            intIdUndad: "required"
        },
        messages: {
            varCdPrdcto: "Asegúrese de ingresar el código",
            varDscrpcionEspñol: "Asegúrese de ingresar la descripción",
            intIdLnea: "Asegúrese de ingresar el tipo de material",
            intIdPscionArnclria: "Asegúrese de ingresar la posición arancelaria",
            intIdUndad: "Asegúrese de ingresar la unidad"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmTmvProductosFacturacionCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmvProductosFacturacion/SetTmvProductoFacturacion'),
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

function frmTmvProductosFacturacionEditar_Submit() {

    //Validar
    frmTmvProductosFacturacionEditar.validate({
        rules: {
            varCdPrdcto: "required",
            varDscrpcionEspñol: "required",
            intIdLnea: "required",
            intIdPscionArnclria: "required",
            intIdUndad: "required"
        },
        messages: {
            varCdPrdcto: "Asegúrese de ingresar el código",
            varDscrpcionEspñol: "Asegúrese de ingresar la descripción",
            intIdLnea: "Asegúrese de ingresar el tipo de material",
            intIdPscionArnclria: "Asegúrese de ingresar la posición arancelaria",
            intIdUndad: "Asegúrese de ingresar la unidad"
        },
        submitHandler: function (Frm) {

            var formData = new FormData();
            $.each(frmTmvProductosFacturacionEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmvProductosFacturacion/Update'),
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

function EliminarProductoFacturacion(intIdPrdctoFctrcion, ActualizarTabla) {

    $.ajax({
        url: ResolveUrl('~/tmvProductosFacturacion/Delete'),
        type: "POST",
        data: { id: intIdPrdctoFctrcion },
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