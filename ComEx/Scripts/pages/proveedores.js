var frmTmProveedoresCrear = $("#frmTmProveedoresCrear");
var frmTmProveedoresEditar = $("#frmTmProveedoresEditar");
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");

var divFormResoluciones = $("#divFormResoluciones");
var divTablaResoluciones = $("#divTablaResoluciones");

$(document).on('ready', function () {
    crearTabla();

    frmTmProveedoresCrear_Submit();
    frmTmProveedoresEditar_Submit();
});

function crearTabla() {

    configExportServer(ResolveUrl('~/tmProveedor/GetProveedores'), '#table-proveedores');

    tablaProveedores = $('#table-proveedores').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmProveedor/GetProveedores'),
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
            {
                orderable: false,
                data: null,
                defaultContent: '<i class="buttonCrud btn_table_azul fa fa-book btnResoluciones" style="cursor: pointer;" title="Resoluciones de Facturación"></i>'
            },
            { title: "intIdPrvdor", data: "intIdPrvdor", visible: false },
            { title: "Código", data: "varCdPrvdor" },
            { title: "NIT", data: "varNitPrvdor" },
            { title: "Nombre", data: "varNmbre" },
            { title: "Cd. Ciudad", data: "varCdCiudad", visible: false },
            { title: "Ciudad", data: "varDscrpcionCiudad" },
            { title: "Cd. Dpto.", data: "varCdDprtmnto", visible: false },
            { title: "Departamento", data: "varDscrpcionDprtmnto" },
            { title: "Dirección", data: "varDrccion" },
            { title: "Teléfono", data: "varTlfno" },
            { title: "Régimen", data: "varNbreRgmen" },
            { title: "Cd. País", data: "varCdPais", visible: false },
            { title: "País", data: "varDscrpcionPais", visible: false },
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
            {
                title: "Bloqueado",
                data: "bitBlqueado",
                render: function (bloqueo) {
                    if (bloqueo) {
                        return "Si";
                    }
                    else {
                        return "No";
                    }
                }
            },
            { title: "Fairmined Id", data: "varFairminedId" }
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
                    window.open(ResolveUrl("~/tmProveedor/Create/"), '_self');
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditar"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdPrvdor = dt.row({ selected: true }).data().intIdPrvdor;
                    window.open(ResolveUrl("~/tmProveedor/Edit/" + intIdPrvdor), '_self');
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrar"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdPrvdor = dt.row({ selected: true }).data().intIdPrvdor;
                    alertify.confirm("Eliminar Proveedor", "¿Realmente desea eliminar el proveedor seleccionado?", function () {
                        //Llamar método para eliminar
                        EliminarProveedor(intIdPrvdor, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditar").addClass("disabled_control");
                                tablaProveedores.button(2).enable(false);

                                //Boton Eliminar
                                $("#btnBorrar").addClass("disabled_control");
                                tablaProveedores.button(3).enable(false);
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
        order: [[5, 'asc']]
    });

    tablaProveedores.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").removeClass("disabled_control");
        tablaProveedores.button(2).enable(true);

        //Boton Eliminar
        $("#btnBorrar").removeClass("disabled_control");
        tablaProveedores.button(3).enable(true);
    });

    tablaProveedores.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditar").addClass("disabled_control");
        tablaProveedores.button(2).enable(false);

        //Boton Eliminar
        $("#btnBorrar").addClass("disabled_control");
        tablaProveedores.button(3).enable(false);
    });

    tablaProveedores.on('click', '.btnResoluciones', function (e) {
        var data = tablaProveedores.row($(this).parents('tr')).data();

        LimpiarCamposFormResoluciones();
        divTablaResoluciones.fadeIn();
        divFormResoluciones.fadeOut();

        $('#modalResoluciones').modal({
            backdrop: 'static',
            keyboard: false
        });

        $('#modalResoluciones').on('shown.bs.modal', function () {
            cargarTablaResoluciones(data.intIdPrvdor);
        });
    });
}

function frmTmProveedoresCrear_Submit() {

    //Validar
    frmTmProveedoresCrear.validate({
        rules: {
            varCdPrvdor: "required",
            varNitPrvdor: "required",
            intIdPais: "required",
            intIdCmpnia: "required"
        },
        messages: {
            varCdPrvdor: "Asegúrese de ingresar el código",
            varNitPrvdor: "Asegúrese de ingresar el NIT",
            intIdPais: "Asegúrese de ingresar el país",
            intIdCmpnia: "Asegúrese de ingresar el exportador"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();

            $.each(frmTmProveedoresCrear.serializeArray(), function (index, value) {
                if (value.name !== "bitAcgdosLey1429" && value.name !== "bitBlqueado") {
                    formData.append(value.name, value.value);
                }
            });

            var bitAcgdosLey1429 = $("#bitAcgdosLey1429").is(":checked") === true ? true : false;
            var bitBlqueado = $("#bitBlqueado").is(":checked") === true ? true : false;

            formData.append("bitAcgdosLey1429", bitAcgdosLey1429);
            formData.append("bitBlqueado", bitBlqueado);

            $.ajax({
                url: ResolveUrl('~/tmProveedor/SetTmProveedor'),
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

function frmTmProveedoresEditar_Submit() {
    //Validar
    frmTmProveedoresEditar.validate({
        rules: {
            varCdPrvdor: "required",
            varNitPrvdor: "required",
            intIdPais: "required",
            intIdCmpnia: "required"
        },
        messages: {
            varCdPrvdor: "Asegúrese de ingresar el código",
            varNitPrvdor: "Asegúrese de ingresar el NIT",
            intIdPais: "Asegúrese de ingresar el país",
            intIdCmpnia: "Asegúrese de ingresar el exportador"
        },
        submitHandler: function (Frm) {

            var formData = new FormData();

            $.each(frmTmProveedoresEditar.serializeArray(), function (index, value) {
                if (value.name !== "bitAcgdosLey1429" && value.name !== "bitBlqueado") {
                    formData.append(value.name, value.value);
                }
            });

            var bitAcgdosLey1429 = $("#bitAcgdosLey1429").is(":checked") === true ? true : false;
            var bitBlqueado = $("#bitBlqueado").is(":checked") === true ? true : false;

            formData.append("bitAcgdosLey1429", bitAcgdosLey1429);
            formData.append("bitBlqueado", bitBlqueado);

            $.ajax({
                url: ResolveUrl('~/tmProveedor/Update'),
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

function EliminarProveedor(intIdPrvdor, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmProveedor/Delete'),
        type: "POST",
        data: { id: intIdPrvdor },
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

function cargarTablaResoluciones(intIdPrvdor) {
    
    $("#hfIntIdPrvdor").val(intIdPrvdor);

    if ($.fn.DataTable.isDataTable('#table-resoluciones')) {
        $('#bodyModal').empty().append('<table class="table table-striped table-bordered table-hover display compact" id="table-resoluciones" style="border-spacing: 0px; border-collapse: separate; width: 100%;"></table>');        
    }

    var tablaResoluciones = $('#table-resoluciones').DataTable({
        dom: "<'row'<'col-md-12'B>><'row'<'col-md-12'tr>>",
        ajax: {
            type: "POST",
            url: ResolveUrl('~/tmResolucionesProveedor/GetResolucionProveedor/' + intIdPrvdor),
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
            { title: "intIdRslcion", data: "intIdRslcion", visible: false },
            { title: "intIdPrvdor", data: "intIdPrvdor", visible: false },
            { title: "Número", data: "NmroRslcion" },
            { title: "Prefijo", data: "varPrfjo" },
            { title: "Desde", data: "NmroRslcionDsde" },
            { title: "Hasta", data: "NmroRslcionHsta" },
            { title: "F. Resolución", data: "fecRslcion", render: dateJsonToString },
            { title: "Vigencia", data: "intVgnciaMses" },
            { title: "F. Vencimiento", data: "FecVncmientoRslcion", render: dateJsonToString },
            {
                title: "Activa",
                data: "bitActva",
                render: function (Ley) {
                    if (Ley) {
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

                    LimpiarCamposFormResoluciones();

                    $("#hfOperacion").val("Nuevo");
                    $("#bitActva").attr('checked', 'checked');

                    divTablaResoluciones.fadeOut();
                    divFormResoluciones.fadeIn();
                }
            },
            {
                text: '<i class="buttonCrud fa fa-pencil-square-o disabled_control" id="btnEditarResolucion"></i>',
                titleAttr: 'Editar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdRslcion = dt.row({ selected: true }).data().intIdRslcion;
                    var NmroRslcion = dt.row({ selected: true }).data().NmroRslcion;
                    var varPrfjo = dt.row({ selected: true }).data().varPrfjo;
                    var NmroRslcionDsde = dt.row({ selected: true }).data().NmroRslcionDsde;
                    var NmroRslcionHsta = dt.row({ selected: true }).data().NmroRslcionHsta;
                    var fecRslcion = dt.row({ selected: true }).data().fecRslcion;
                    var intVgnciaMses = dt.row({ selected: true }).data().intVgnciaMses;
                    var FecVncmientoRslcion = dt.row({ selected: true }).data().FecVncmientoRslcion;
                    var bitActva = dt.row({ selected: true }).data().bitActva; 

                    var find = '/';
                    var re = new RegExp(find, 'g');

                    $("#hfOperacion").val("Editar");
                    $("#hfIntIdRslcion").val(intIdRslcion);

                    $("#NmroRslcion").val(NmroRslcion);
                    $("#varPrfjo").val(varPrfjo);
                    $("#NmroRslcionDsde").val(NmroRslcionDsde);
                    $("#NmroRslcionHsta").val(NmroRslcionHsta);
                    $("#fecRslcion").val(dateJsonToString(fecRslcion).replace(re, "-"));    
                    $("#intVgnciaMses").val(intVgnciaMses);
                    $("#FecVncmientoRslcion").val(dateJsonToString(FecVncmientoRslcion).replace(re, "-"));  
                    $("#bitActva").prop('checked', bitActva);

                    divTablaResoluciones.fadeOut();
                    divFormResoluciones.fadeIn();
                }
            },
            {
                text: '<i class="buttonCrud del fa fa-trash-o disabled_control" id="btnBorrarResolucion"></i>',
                titleAttr: 'Borrar',
                enabled: false,
                action: function (e, dt, node, config) {
                    var intIdRslcion = dt.row({ selected: true }).data().intIdRslcion;
                    alertify.confirm("Eliminar Resolución", "¿Realmente desea eliminar la resolución seleccionada?", function () {
                        //Llamar método para eliminar
                        EliminarResolucion(intIdRslcion, function (resultado) {
                            if (resultado) {
                                dt.row({ selected: true }).remove().draw(false);
                                //Boton Editar
                                $("#btnEditarResolucion").addClass("disabled_control");
                                tablaResoluciones.button(1).enable(false);

                                //Boton Eliminar
                                $("#btnBorrarResolucion").addClass("disabled_control");
                                tablaResoluciones.button(2).enable(false);
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
        order: [[7, 'desc']]
    });

    tablaResoluciones.on('select', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditarResolucion").removeClass("disabled_control");
        tablaResoluciones.button(1).enable(true);

        //Boton Eliminar
        $("#btnBorrarResolucion").removeClass("disabled_control");
        tablaResoluciones.button(2).enable(true);
    });

    tablaResoluciones.on('deselect', function (e, dt, type, indexes) {

        //Boton Editar
        $("#btnEditarResolucion").addClass("disabled_control");
        tablaResoluciones.button(1).enable(false);

        //Boton Eliminar
        $("#btnBorrarResolucion").addClass("disabled_control");
        tablaResoluciones.button(2).enable(false);
    });
}

function EliminarResolucion(intIdRslcion, ActualizarTabla) {
    $.ajax({
        url: ResolveUrl('~/tmResolucionesProveedor/Delete'),
        type: "POST",
        data: { id: intIdRslcion },
        beforeSend: function () {
            console.info("Enviando infromación...");
        },
        success: function (data) {
            if (data.Success) {
                ActualizarTabla(true);
            }
            else {
                console.error(data.Message);

                $("#messageResolucion").html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                    + data.Message + '</div>')
                    .fadeIn(100);

                ActualizarTabla(false);
            }
        }
    });
}

//Botón cancelar modal resoluciones
$("#btnCancelarResol").click(function () {
    divTablaResoluciones.fadeIn();
    divFormResoluciones.fadeOut();
});

//Botón guardar modal resoluciones
$("#btnGuardarResol").click(function () {

    //Campos
    var intIdRslcion = $("#hfIntIdRslcion").val();
    var intIdPrvdor = $("#hfIntIdPrvdor").val();
    var NmroRslcion = $("#NmroRslcion").val();
    var varPrfjo = $("#varPrfjo").val();
    var NmroRslcionDsde = $("#NmroRslcionDsde").val();
    var NmroRslcionHsta = $("#NmroRslcionHsta").val();
    var fecRslcion = $("#fecRslcion").val();
    var intVgnciaMses = $("#intVgnciaMses").val();
    var FecVncmientoRslcion = $("#FecVncmientoRslcion").val();
    var bitActva = $("#bitActva").is(":checked") === true ? true : false;
    var operacion = $("#hfOperacion").val();

    var valido = validarFormResoluciones();

    if (valido) {

        var formData = new FormData();

        formData.append("intIdRslcion", intIdRslcion);
        formData.append("intIdPrvdor", intIdPrvdor);
        formData.append("NmroRslcion", NmroRslcion);
        formData.append("varPrfjo", varPrfjo);
        formData.append("NmroRslcionDsde", NmroRslcionDsde);
        formData.append("NmroRslcionHsta", NmroRslcionHsta);
        formData.append("fecRslcion", fecRslcion);
        formData.append("intVgnciaMses", intVgnciaMses);
        formData.append("FecVncmientoRslcion", FecVncmientoRslcion);
        formData.append("bitActva", bitActva);
        formData.append("operacion", operacion);

        $.ajax({
            url: ResolveUrl('~/tmResolucionesProveedor/CUResolucion'),
            type: "POST",
            contentType: false,
            cache: false,
            dataType: "json",
            processData: false,
            data: formData,
            beforeSend: function () {
                console.info("Enviando infromación...");
                $("#btnGuardarResol").button('loading');
            },
            success: function (data) {
                if (data.Success) {
                    //Limpiar el botón
                    $("#btnGuardarResol").button('reset');

                    LimpiarCamposFormResoluciones();
                    cargarTablaResoluciones(intIdPrvdor);

                    divTablaResoluciones.fadeIn();
                    divFormResoluciones.fadeOut();
                }
                else {
                    console.error(data.Message);

                    $("#messageResolucion").html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                        + data.Message + '</div>')
                        .fadeIn(100);

                    //Limpiar el botón
                    $("#btnGuardarResol").button('reset');
                }
            }
        });
    }
});


function validarFormResoluciones() {

    var NmroRslcion = $("#NmroRslcion");
    var varPrfjo = $("#varPrfjo");
    var NmroRslcionDsde = $("#NmroRslcionDsde");
    var NmroRslcionHsta = $("#NmroRslcionHsta");
    var fecRslcion = $("#fecRslcion");
    var intVgnciaMses = $("#intVgnciaMses");
    var FecVncmientoRslcion = $("#FecVncmientoRslcion");

    //Validaciones
    var valid_NmroRslcion = $("#valid_NmroRslcion");
    var valid_NmroRslcionDsde = $("#valid_NmroRslcionDsde");
    var valid_NmroRslcionHsta = $("#valid_NmroRslcionHsta");
    var valid_fecRslcion = $("#valid_fecRslcion");
    var valid_intVgnciaMses = $("#valid_intVgnciaMses");
    var valid_FecVncmientoRslcion = $("#valid_FecVncmientoRslcion");

    var valido = true;

    //Se valida la resolución
    if (NmroRslcion.val() !== "") {
        //Se remueven las propiedades de error 
        NmroRslcion.parent().removeClass("has-error");
        valid_NmroRslcion.empty();
        valido = true;
    }
    else {
        NmroRslcion.parent().addClass("has-error");
        valid_NmroRslcion.empty().append("Requerido");
        $("#btnGuardarResol").button('reset');
        valido = false;
    }

    //Se valida el campo desde
    if (NmroRslcionDsde.val() !== "") {
        //Se remueven las propiedades de error 
        NmroRslcionDsde.parent().removeClass("has-error");
        valid_NmroRslcionDsde.empty();
        valido = true;
    }
    else {
        NmroRslcionDsde.parent().addClass("has-error");
        valid_NmroRslcionDsde.empty().append("Requerido");
        $("#btnGuardarResol").button('reset');
        valido = false;
    }

    //Se valida el campo hasta
    if (NmroRslcionHsta.val() !== "") {
        //Se remueven las propiedades de error 
        NmroRslcionHsta.parent().removeClass("has-error");
        valid_NmroRslcionHsta.empty();
        valido = true;
    }
    else {
        NmroRslcionHsta.parent().addClass("has-error");
        valid_NmroRslcionHsta.empty().append("Requerido");
        $("#btnGuardarResol").button('reset');
        valido = false;
    }

    //Se valida el campo fecha de resolución
    if (fecRslcion.val() !== "") {
        //Se remueven las propiedades de error 
        fecRslcion.parent().removeClass("has-error");
        valid_fecRslcion.empty();
        valido = true;
    }
    else {
        fecRslcion.parent().addClass("has-error");
        valid_fecRslcion.empty().append("Requerido");
        $("#btnGuardarResol").button('reset');
        valido = false;
    }

    //Se valida el campo vigencia
    if (intVgnciaMses.val() !== "") {
        //Se remueven las propiedades de error 
        intVgnciaMses.parent().removeClass("has-error");
        valid_intVgnciaMses.empty();
        valido = true;
    }
    else {
        intVgnciaMses.parent().addClass("has-error");
        valid_intVgnciaMses.empty().append("Requerido");
        $("#btnGuardarResol").button('reset');
        valido = false;
    }

    //Se valida el campo fecha de vencimiento
    if (FecVncmientoRslcion.val() !== "") {
        //Se remueven las propiedades de error 
        FecVncmientoRslcion.parent().removeClass("has-error");
        valid_FecVncmientoRslcion.empty();
        valido = true;
    }
    else {
        FecVncmientoRslcion.parent().addClass("has-error");
        valid_FecVncmientoRslcion.empty().append("Requerido");
        $("#btnGuardarResol").button('reset');
        valido = false;
    }

    return valido;
}

function LimpiarCamposFormResoluciones(){

    $("#NmroRslcion").val("");
    $("#varPrfjo").val("");
    $("#NmroRslcionDsde").val("");
    $("#NmroRslcionHsta").val("");
    $("#fecRslcion").val("");  
    $("#intVgnciaMses").val("");
    $("#FecVncmientoRslcion").val("");
    $("#bitActva").attr('checked', 'checked');
}

$("#intVgnciaMses").keyup(function () {
    CalcFecVenc();
});

function CalcFecVenc() {

    var fecRslcion = $("#fecRslcion");
    var intVgnciaMses = $("#intVgnciaMses");
    var FecVncmientoRslcion = $("#FecVncmientoRslcion");

    //Se valida los campos
    if (fecRslcion.val() !== "" && intVgnciaMses.val() !== "") {

        var date = new Date(fecRslcion.val());
        var months = parseInt(intVgnciaMses.val(), 10);

        date.setMonth(date.getMonth() + months);

        FecVncmientoRslcion.val(date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2));
    }

}