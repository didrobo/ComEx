var frmTmInsumosCrear = $("#frmTmInsumosCrear");
var frmTmInsumosEditar = $("#frmTmInsumosEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");

$(document).on('ready', function () {
    crearTabla();

    frmTmInsumosCrear_Submit();
    frmTmInsumosEditar_Submit();
});

function crearTabla() {

    tablaInsumos = $('#table-insumos').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmInsumos/GetInsumos'),
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
            { title: "intIdInsmo", data: "intIdInsmo", visible: false },
            { title: "Código", data: "varCdInsmo" },
            { title: "Descripción", data: "varDscrpcionInsmo" },
            { title: "Tipo Material", data: "tmLineas.varDscrpcionLnea" },
            { title: "Pos. Arancelaria", data: "tmPosicionesArancelarias.varPscionArnclria" },
            { title: "Unidad", data: "tmUnidades.varCdUndad" },
            { title: "Unidad Dian", data: "tmUnidades1.varCdUndad" },
            { title: "Unidad Compra", data: "tmUnidades2.varCdUndad" },
            { title: "Factor", data: "numFctorMncmex", render: $.fn.dataTable.render.number(',', '.', 8, ''), className: 'text-right' },
            { title: "Estado Mercancía", data: "tmComprasTipo.varDscrpcionCmpra" },
            {
                title: "Insumo Nacional",
                data: "bitInsmoNcional",
                render: function (Nacional) {
                    if (Nacional) {
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
                    var intIdInsmo = dt.row({ selected: true }).data().intIdInsmo;
                    window.open(ResolveUrl("~/tmInsumos/Edit/" + intIdInsmo), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdInsmo = dt.row({ selected: true }).data().intIdInsmo;
                    alertify.confirm("Eliminar Insumo", "¿Realmente desea eliminar el insumo seleccionado?", function () {
                        //Llamar método para eliminar
                        EliminarInsumo(intIdInsmo, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaInsumos.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaInsumos.button(2).enable(false);
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

    tablaInsumos.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaInsumos.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaInsumos.button(2).enable(true);
    });

    tablaInsumos.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaInsumos.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaInsumos.button(2).enable(false);
    });
}

function frmTmInsumosCrear_Submit() {

    //Validar
    frmTmInsumosCrear.validate({
        rules: {
            varCdInsmo: "required",
            varDscrpcionInsmo: "required",
            intIdLnea: "required",
            intIdPscionArnclria: "required",
            intIdUndad: "required",
            intIdUndadDian: "required",
            intIdUndadCmpra: "required",
            intIdTpoCmpra: "required",
            numFctorMncmex: "required"
        },
        messages: {
            varCdInsmo: "Asegúrese de ingresar el código",
            varDscrpcionInsmo: "Asegúrese de ingresar la descripción",
            intIdLnea: "Asegúrese de ingresar el tipo de material",
            intIdPscionArnclria: "Asegúrese de ingresar la posición arancelaria",
            intIdUndad: "Asegúrese de ingresar la unidad",
            intIdUndadDian: "Asegúrese de ingresar la unidad Dian",
            intIdUndadCmpra: "Asegúrese de ingresar la unidad de compra",
            intIdTpoCmpra: "Asegúrese de ingresar el estado de la mercancía",
            numFctorMncmex: "Asegúrese de ingresar el factor"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmTmInsumosCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmInsumos/SetTmInsumos'),
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

function frmTmInsumosEditar_Submit() {

    //Validar
    frmTmInsumosEditar.validate({
        rules: {
            varDscrpcionInsmo: "required",
            intIdLnea: "required",
            intIdPscionArnclria: "required",
            intIdUndad: "required",
            intIdUndadDian: "required",
            intIdUndadCmpra: "required",
            intIdTpoCmpra: "required",
            numFctorMncmex: "required"
        },
        messages: {
            varDscrpcionInsmo: "Asegúrese de ingresar la descripción",
            intIdLnea: "Asegúrese de ingresar el tipo de material",
            intIdPscionArnclria: "Asegúrese de ingresar la posición arancelaria",
            intIdUndad: "Asegúrese de ingresar la unidad",
            intIdUndadDian: "Asegúrese de ingresar la unidad Dian",
            intIdUndadCmpra: "Asegúrese de ingresar la unidad de compra",
            intIdTpoCmpra: "Asegúrese de ingresar el estado de la mercancía",
            numFctorMncmex: "Asegúrese de ingresar el factor"
        },
        submitHandler: function (Frm) {

            var formData = new FormData();
            $.each(frmTmInsumosEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmInsumos/Update'),
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

function EliminarInsumo(intIdInsmo, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmInsumos/Delete'),
        type: "POST",
        data: { id: intIdInsmo },
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