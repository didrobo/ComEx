var frmTmRegaliasCrear = $("#frmTmRegaliasCrear");
var frmTmRegaliasEditar = $("#frmTmRegaliasEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");

$(document).on('ready', function () {
    crearTabla();

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });

    frmTmRegaliasCrear_Submit();
    frmTmRegaliasEditar_Submit();
});

function crearTabla() {

    tablaRegalias = $('#table-regalias').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmRegalias/GetRegalias'),
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
            { title: "intIdRglia", data: "intIdRglia", visible: false },
            { title: "intIdLnea", data: "intIdLnea", visible: false },
            { title: "Tipo Material", data: "tmLineas.varCdLnea" },
            { title: "Año", data: "intAno" },
            { title: "Mes", data: "intMes" },
            { title: "Valor Base", data: "numVlorPorGrmo", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "F. Inicio", data: "fecIncioPriodo", render: dateJsonToString },
            { title: "F. Fin", data: "fecFinPriodo", render: dateJsonToString }
        ],
        buttons: [
            {
                text: '<i class="buttonCrud fa fa-file-o"></i>',
                titleAttr: 'Nuevo',
                action: function (e, dt, node, config) {
                    window.open(ResolveUrl("~/tmRegalias/Create/"), '_self');
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditar"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdRglia = dt.row({ selected: true }).data().intIdRglia;
                    window.open(ResolveUrl("~/tmRegalias/Edit/" + intIdRglia), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdRglia = dt.row({ selected: true }).data().intIdRglia;
                    alertify.confirm("Eliminar Regalía", "¿Realmente desea eliminar la regalía seleccionada?", function () {
                        //Llamar método para eliminar
                        EliminarRegalia(intIdRglia, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaRegalias.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaRegalias.button(2).enable(false);
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
        order: [[7, 'desc']]
    });

    tablaRegalias.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaRegalias.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaRegalias.button(2).enable(true);
    });

    tablaRegalias.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaRegalias.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaRegalias.button(2).enable(false);
    });
}

function frmTmRegaliasCrear_Submit() {

    //Validar
    frmTmRegaliasCrear.validate({
        rules: {
            intIdLnea: "required",
            intAno: "required",
            intMes: "required",
            numVlorPorGrmo: "required",
            fecIncioPriodo: "required",
            fecFinPriodo: "required"
        },
        messages: {
            intIdLnea: "Asegúrese de ingresar el tipo de material",
            intAno: "Asegúrese de ingresar el año",
            intMes: "Asegúrese de ingresar el mes",
            numVlorPorGrmo: "Asegúrese de ingresar el valor base",
            fecIncioPriodo: "Asegúrese de ingresar la fecha inicio",
            fecFinPriodo: "Asegúrese de ingresar la fecha fin"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmTmRegaliasCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmRegalias/SetTmRegalias'),
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
                //error: function (jqXHR) {
                //    mensaje.html('<div class="alert alert-danger" role="alert">' + jqXHR.statusText + '</div>')
                //        .fadeIn(1000);
                //}
            });
        }
    });
}

function frmTmRegaliasEditar_Submit() {
    //Validar
    frmTmRegaliasEditar.validate({
        rules: {
            intIdLnea: "required",
            intAno: "required",
            intMes: "required",
            numVlorPorGrmo: "required",
            fecIncioPriodo: "required",
            fecFinPriodo: "required"
        },
        messages: {
            intIdLnea: "Asegúrese de ingresar el tipo de material",
            intAno: "Asegúrese de ingresar el año",
            intMes: "Asegúrese de ingresar el mes",
            numVlorPorGrmo: "Asegúrese de ingresar el valor base",
            fecIncioPriodo: "Asegúrese de ingresar la fecha inicio",
            fecFinPriodo: "Asegúrese de ingresar la fecha fin"
        },
        submitHandler: function (Frm) {

            var formData = new FormData();
            $.each(frmTmRegaliasEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmRegalias/Update'),
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

function EliminarRegalia(intIdRglia, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmRegalias/Delete'),
        type: "POST",
        data: { id: intIdRglia },
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