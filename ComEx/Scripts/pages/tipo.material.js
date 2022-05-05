var frmtmTipoMaterialCrear = $("#frmtmTipoMaterialCrear");
var frmTmTipoMaterialEditar = $("#frmTmTipoMaterialEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");

$(document).on('ready', function () {
    crearTabla();
    
    frmTmTipoMaterialCrear_Submit();
    frmTmTipoMaterialEditar_Submit();
});

function crearTabla() {
    
    tablaTipoMaterial = $('#table-tipo-material').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmTipoMaterial/GetTipoMaterial'),
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
            { title: "intIdLnea", data: "intIdLnea", visible: false },
            { title: "Código", data: "varCdLnea" },
            { title: "Descripción", data: "varDscrpcionLnea" },
            { title: "Descripción Dian", data: "varDscrpcionDian" },
            { title: "% Base Regalías", data: "numPrcntjeBseRglias", render: $.fn.dataTable.render.number(',', '.', 2, '%'), className: 'text-right' },
            { title: "% Rte Fuente", data: "tmRetefuente.numPrcntjeRtncionfuente", render: $.fn.dataTable.render.number(',', '.', 2, '%'), className: 'text-right' },          
            { title: "Factor", data: "numFctor", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' }
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
                    var intIdLnea = dt.row({ selected: true }).data().intIdLnea;
                    window.open(ResolveUrl("~/tmTipoMaterial/Edit/" + intIdLnea), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdLnea = dt.row({ selected: true }).data().intIdLnea;
                    alertify.confirm("Eliminar Tipo de Material", "¿Realmente desea eliminar el tipo de material seleccionado?", function () {
                        //Llamar método para eliminar
                        EliminarTipoMaterial(intIdLnea, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaTipoMaterial.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaTipoMaterial.button(2).enable(false);
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

    tablaTipoMaterial.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaTipoMaterial.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaTipoMaterial.button(2).enable(true);
    });

    tablaTipoMaterial.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaTipoMaterial.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaTipoMaterial.button(2).enable(false);
    });
}

function frmTmTipoMaterialCrear_Submit() {

    //Validar
    frmtmTipoMaterialCrear.validate({
        rules: {
            varCdLnea: "required",
            varDscrpcionLnea: "required",
            varDscrpcionDian: "required",
            numPrcntjeBseRglias: "required",
            numFctor: "required"
        },
        messages: {
            varCdLnea: "Asegúrese de ingresar el código",
            varDscrpcionLnea: "Asegúrese de ingresar la descripción",
            varDscrpcionDian: "Asegúrese de ingresar la descripción DIAN",
            numPrcntjeBseRglias: "Asegúrese de ingresar el % base de las regalías",
            numFctor: "Asegúrese de ingresar el factor"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmtmTipoMaterialCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmTipoMaterial/SetTmTipoMaterial'),
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

function frmTmTipoMaterialEditar_Submit() {    
    //Validar
    frmTmTipoMaterialEditar.validate({
        rules: {
            varCdLnea: "required",
            varDscrpcionLnea: "required",
            varDscrpcionDian: "required",
            numPrcntjeBseRglias: "required",
            numFctor: "required"
        },
        messages: {
            varCdLnea: "Asegúrese de ingresar el código",
            varDscrpcionLnea: "Asegúrese de ingresar la descripción",
            varDscrpcionDian: "Asegúrese de ingresar la descripción DIAN",
            numPrcntjeBseRglias: "Asegúrese de ingresar el % base de las regalías",
            numFctor: "Asegúrese de ingresar el factor"
        },
        submitHandler: function (Frm) {
            
            var formData = new FormData();
            $.each(frmTmTipoMaterialEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            $.ajax({
                url: ResolveUrl('~/tmTipoMaterial/Update'),
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

function EliminarTipoMaterial(intIdLnea, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmTipoMaterial/Delete'),
        type: "POST",
        data: { id: intIdLnea },
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