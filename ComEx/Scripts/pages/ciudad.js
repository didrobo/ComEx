var frmtmCiudadesCrear = $("#frmtmCiudadesCrear");
var frmtmCiudadesEditar = $("#frmtmCiudadesEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");

$(document).on('ready', function () {
    crearTabla();

    frmtmCiudadesCrear_Submit();
    frmtmCiudadesEditar_Submit();
});

function crearTabla() {

    tablaCiudad = $('#table-ciudad').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmCiudades/GetCiudad'),
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
            { title: "intIdCiudad", data: "intIdCiudad", visible: false },
            { title: "Código", data: "varCdCiudad" },
            { title: "Descripción", data: "varDscrpcionCiudad" },
            { title: "Abreviatura", data: "varAbrvtra" },
            { title: "Otro Cod.", data: "varCd" },
            { title: "Departamento", data: "tmDepartamentos.varDscrpcionDprtmnto" }
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
                    var intIdCiudad = dt.row({ selected: true }).data().intIdCiudad;
                    window.open(ResolveUrl("~/tmCiudades/Edit/" + intIdCiudad), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdCiudad = dt.row({ selected: true }).data().intIdCiudad;
                    alertify.confirm("Eliminar Ciudad", "¿Realmente desea eliminar la ciudad seleccionada?", function () {
                        //Llamar método para eliminar
                        EliminarCiudad(intIdCiudad, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaCiudad.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaCiudad.button(2).enable(false);
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

    tablaCiudad.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaCiudad.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaCiudad.button(2).enable(true);
    });

    tablaCiudad.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaCiudad.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaCiudad.button(2).enable(false);
    });
}

function frmtmCiudadesCrear_Submit() {

    //Validar
    frmtmCiudadesCrear.validate({
        rules: {
            varCdCiudad: "required",
            varDscrpcionCiudad: "required",
            varAbrvtra: "required",
            varCd: "required",
            intIdDprtmnto: "required"
        },
        messages: {
            varCdCiudad: "Asegúrese de ingresar el código",
            varDscrpcionCiudad: "Asegúrese de ingresar la descripción",
            varAbrvtra: "Asegúrese de ingresar la abrevietura",
            varCd: "Asegúrese de ingresar el otro código",
            intIdDprtmnto: "Asegúrese de ingresar el departamento"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmtmCiudadesCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmCiudades/SetTmCiudad'),
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

function frmtmCiudadesEditar_Submit() {

    //Validar
    frmtmCiudadesEditar.validate({
        rules: {
            varCdCiudad: "required",
            varDscrpcionCiudad: "required",
            varAbrvtra: "required",
            varCd: "required",
            intIdDprtmnto: "required"
        },
        messages: {
            varCdCiudad: "Asegúrese de ingresar el código",
            varDscrpcionCiudad: "Asegúrese de ingresar la descripción",
            varAbrvtra: "Asegúrese de ingresar la abrevietura",
            varCd: "Asegúrese de ingresar el otro código",
            intIdDprtmnto: "Asegúrese de ingresar el departamento"
        },
        submitHandler: function (Frm) {

            var formData = new FormData();
            $.each(frmtmCiudadesEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmCiudades/Update'),
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

function EliminarCiudad(intIdCiudad, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmCiudades/Delete'),
        type: "POST",
        data: { id: intIdCiudad },
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