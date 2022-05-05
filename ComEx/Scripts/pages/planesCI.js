var frmTmPlanesCICrear = $("#frmTmPlanesCICrear");
var frmTmPlanesCIEditar = $("#frmTmPlanesCIEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");

$(document).on('ready', function () {
    crearTabla();

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });

    frmTmPlanesCICrear_Submit();
    frmTmPlanesCIEditar_Submit();
});


function crearTabla() {

    tablaPlanesCI = $('#table-planesCI').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmPlanesCi/GetPlanesCI'),
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
            { title: "intIdPlanCI", data: "intIdPlanCI", visible: false },
            { title: "intIdImprtdorExprtdor", data: "intIdImprtdorExprtdor", visible: false },            
            { title: "Código", data: "varCdPlanCI" },
            { title: "Descripción", data: "varDscrpcionPlanCI" },
            { title: "Exportador", data: "tmImportadorExportador.varNmbre" },
            { title: "F. Presentación", data: "FecPrsntcion", render: dateJsonToString },
            { title: "F. Fin Expo.", data: "FecFnalExprtcion", render: dateJsonToString },
            { title: "F. Fin Comp.", data: "FecFnalCmpras", render: dateJsonToString },
            {
                title: "Bloqueado",
                data: "bitBloqueo",
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
                    window.open(ResolveUrl("~/tmPlanesCi/Create/"), '_self');
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditar"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdPlanCI = dt.row({ selected: true }).data().intIdPlanCI;
                    window.open(ResolveUrl("~/tmPlanesCi/Edit/" + intIdPlanCI), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdPlanCI = dt.row({ selected: true }).data().intIdPlanCI;
                    alertify.confirm("Eliminar Plan CI", "¿Realmente desea eliminar el plan seleccionado?", function () {
                        //Llamar método para eliminar
                        EliminarPlanCI(intIdPlanCI, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaPlanesCI.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaPlanesCI.button(2).enable(false);
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
        order: [[6, 'desc']]
    });

    tablaPlanesCI.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaPlanesCI.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaPlanesCI.button(2).enable(true);
    });

    tablaPlanesCI.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaPlanesCI.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaPlanesCI.button(2).enable(false);
    });
}

function frmTmPlanesCICrear_Submit() {

    //Validar
    frmTmPlanesCICrear.validate({
        rules: {
            varCdPlanCI: "required",
            varDscrpcionPlanCI: "required",
            FecPrsntcion: "required",
            FecFnalExprtcion: "required",
            FecFnalCmpras: "required",
            intIdImprtdorExprtdor: "required"
        },
        messages: {
            varCdPlanCI: "Asegúrese de ingresar el código",
            varDscrpcionPlanCI: "Asegúrese de ingresar la descripción",
            FecPrsntcion: "Asegúrese de ingresar la fecha de presentación",
            FecFnalExprtcion: "Asegúrese de ingresar la fecha final de exportación",
            FecFnalCmpras: "Asegúrese de ingresar la fecha final de compras",
            intIdImprtdorExprtdor: "Asegúrese de ingresar el exportador"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmTmPlanesCICrear.serializeArray(), function (index, value) {
                if (value.name != "bitBloqueo") {
                    formData.append(value.name, value.value);
                }
            });

            var bitBloqueo = $("#bitBloqueo").is(":checked") == true ? true : false;

            formData.append("bitBloqueo", bitBloqueo);

            $.ajax({
                url: ResolveUrl('~/tmPlanesCi/SetTmPlanesCI'),
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

function frmTmPlanesCIEditar_Submit() {
    //Validar
    frmTmPlanesCIEditar.validate({
        rules: {
            varCdPlanCI: "required",
            varDscrpcionPlanCI: "required",
            FecPrsntcion: "required",
            FecFnalExprtcion: "required",
            FecFnalCmpras: "required",
            intIdImprtdorExprtdor: "required"
        },
        messages: {
            varCdPlanCI: "Asegúrese de ingresar el código",
            varDscrpcionPlanCI: "Asegúrese de ingresar la descripción",
            FecPrsntcion: "Asegúrese de ingresar la fecha de presentación",
            FecFnalExprtcion: "Asegúrese de ingresar la fecha final de exportación",
            FecFnalCmpras: "Asegúrese de ingresar la fecha final de compras",
            intIdImprtdorExprtdor: "Asegúrese de ingresar el exportador"
        },
        submitHandler: function (Frm) {

            var formData = new FormData();

            $.each(frmTmPlanesCIEditar.serializeArray(), function (index, value) {
                if (value.name != "bitBloqueo") {
                    formData.append(value.name, value.value);
                }
            });

            var bitBloqueo = $("#bitBloqueo").is(":checked") == true ? true : false;

            formData.append("bitBloqueo", bitBloqueo);

            $.ajax({
                url: ResolveUrl('~/tmPlanesCi/Update'),
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

function EliminarPlanCI(intIdPlanCI, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmPlanesCi/Delete'),
        type: "POST",
        data: { id: intIdPlanCI },
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