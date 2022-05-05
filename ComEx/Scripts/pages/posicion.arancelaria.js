var frmtmPosicionesArancelariasCrear = $("#frmtmPosicionesArancelariasCrear");
var frmtmPosicionesArancelariasEditar = $("#frmtmPosicionesArancelariasEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");


$(document).on('ready', function () {
    crearTabla();

    frmtmPosicionesArancelariasCrear_Submit();
    frmtmPosicionesArancelariasEditar_Submit();
});

function crearTabla() {

    tablaPosicionArancelaria = $('#table-posicion-arancelaria').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmPosicionesArancelarias/GetPosicionArancelaria'),
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
            { title: "intIdPscionArnclria", data: "intIdPscionArnclria", visible: false },
            { title: "P. Arancelaria", data: "varPscionArnclria" },            
            { title: "Unidad", data: "tmUnidades.varCdUndad" },
            { title: "% Arancel", data: "numPrcntjeArncel", render: $.fn.dataTable.render.number(',', '.', 2, '%'), className: 'text-right' },
            { title: "% Iva", data: "numPrcntjeIva", render: $.fn.dataTable.render.number(',', '.', 2, '%'), className: 'text-right' },
            { title: "Descripción", data: "varDscrpcionPscionArnclria" }
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
                    var intIdPscionArnclria = dt.row({ selected: true }).data().intIdPscionArnclria;
                    window.open(ResolveUrl("~/tmPosicionesArancelarias/Edit/" + intIdPscionArnclria), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdPscionArnclria = dt.row({ selected: true }).data().intIdPscionArnclria;
                    alertify.confirm("Eliminar Posición Arancelaria", "¿Realmente desea eliminar la posición arancelaria seleccionada?", function () {
                        //Llamar método para eliminar
                        EliminarPosicionArancelaria(intIdPscionArnclria, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaPosicionArancelaria.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaPosicionArancelaria.button(2).enable(false);
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

    tablaPosicionArancelaria.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaPosicionArancelaria.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaPosicionArancelaria.button(2).enable(true);
    });

    tablaPosicionArancelaria.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaPosicionArancelaria.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaPosicionArancelaria.button(2).enable(false);
    });
}

function frmtmPosicionesArancelariasCrear_Submit() {

    //Validar
    frmtmPosicionesArancelariasCrear.validate({
        rules: {
            varPscionArnclria: "required",
            numPrcntjeArncel: "required",
            numPrcntjeIva: "required",
            varDscrpcionPscionArnclria: "required",
            intIdUndad: "required"
        },
        messages: {
            varPscionArnclria: "Asegúrese de ingresar la posición arancelaria",
            numPrcntjeArncel: "Asegúrese de ingresar el % arancel",
            numPrcntjeIva: "Asegúrese de ingresar el % iva",
            varDscrpcionPscionArnclria: "Asegúrese de ingresar la descripción",
            intIdUndad: "Asegúrese de ingresar la unidad"            
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmtmPosicionesArancelariasCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmPosicionesArancelarias/SetTmPosicionArancelaria'),
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

function frmtmPosicionesArancelariasEditar_Submit() {

    //Validar
    frmtmPosicionesArancelariasEditar.validate({
        rules: {
            varPscionArnclria: "required",
            numPrcntjeArncel: "required",
            numPrcntjeIva: "required",
            varDscrpcionPscionArnclria: "required",
            intIdUndad: "required"
        },
        messages: {
            varPscionArnclria: "Asegúrese de ingresar la posición arancelaria",
            numPrcntjeArncel: "Asegúrese de ingresar el % arancel",
            numPrcntjeIva: "Asegúrese de ingresar el % iva",
            varDscrpcionPscionArnclria: "Asegúrese de ingresar la descripción",
            intIdUndad: "Asegúrese de ingresar la unidad"   
        },
        submitHandler: function (Frm) {

            var formData = new FormData();
            $.each(frmtmPosicionesArancelariasEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmPosicionesArancelarias/Update'),
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

function EliminarPosicionArancelaria(intIdPscionArnclria, ActualizarTabla) {

    $.ajax({
        url: ResolveUrl('~/tmPosicionesArancelarias/Delete'),
        type: "POST",
        data: { id: intIdPscionArnclria },
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