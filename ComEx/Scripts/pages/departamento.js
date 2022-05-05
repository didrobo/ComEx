var frmtmDepartamentosCrear = $("#frmtmDepartamentosCrear");
var frmtmDepartamentosEditar = $("#frmtmDepartamentosEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");

$(document).on('ready', function () {
    crearTabla();

    frmtmDepartamentosCrear_Submit();
    frmtmDepartamentosEditar_Submit();
});

function crearTabla() {

    tablaDepartamento = $('#table-departamento').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmDepartamentos/GetDepartamento'),
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
            { title: "intIdDprtmnto", data: "intIdDprtmnto", visible: false },
            { title: "Código", data: "varCdDprtmnto" },
            { title: "Descripción", data: "varDscrpcionDprtmnto" }
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
                    var intIdDprtmnto = dt.row({ selected: true }).data().intIdDprtmnto;
                    window.open(ResolveUrl("~/tmDepartamentos/Edit/" + intIdDprtmnto), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdDprtmnto = dt.row({ selected: true }).data().intIdDprtmnto;
                    alertify.confirm("Eliminar Departamento", "¿Realmente desea eliminar el departamento seleccionado?", function () {
                        //Llamar método para eliminar
                        EliminarDepartamento(intIdDprtmnto, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaDepartamento.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaDepartamento.button(2).enable(false);
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

    tablaDepartamento.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaDepartamento.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaDepartamento.button(2).enable(true);
    });

    tablaDepartamento.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaDepartamento.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaDepartamento.button(2).enable(false);
    });
}

function frmtmDepartamentosCrear_Submit() {

    //Validar
    frmtmDepartamentosCrear.validate({
        rules: {
            varCdDprtmnto: "required",
            varDscrpcionDprtmnto: "required"
        },
        messages: {
            varCdDprtmnto: "Asegúrese de ingresar el código",
            varDscrpcionDprtmnto: "Asegúrese de ingresar la descripción"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmtmDepartamentosCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmDepartamentos/SetTmDepartamento'),
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

function frmtmDepartamentosEditar_Submit() {

    //Validar
    frmtmDepartamentosEditar.validate({
        rules: {
            varCdDprtmnto: "required",
            varDscrpcionDprtmnto: "required"
        },
        messages: {
            varCdDprtmnto: "Asegúrese de ingresar el código",
            varDscrpcionDprtmnto: "Asegúrese de ingresar la descripción"
        },
        submitHandler: function (Frm) {

            var formData = new FormData();
            $.each(frmtmDepartamentosEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmDepartamentos/Update'),
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

function EliminarDepartamento(intIdDprtmnto, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmDepartamentos/Delete'),
        type: "POST",
        data: { id: intIdDprtmnto },
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
        }
    });
}