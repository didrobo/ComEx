//Variables
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");
var frmTmvDEXDetalleCrear = $("#frmTmvDEXDetalleCrear");
var frmTmvDEXDetalleEditar = $("#frmTmvDEXDetalleEditar");

$(document).on("ready", inicio);

function inicio() {
    //Métodos
    frmTmvDEXDetalleCrear_Submit();
    frmTmvDEXDetalleEditar_Submit();
}

function frmTmvDEXDetalleCrear_Submit() {

    //Validar
    frmTmvDEXDetalleCrear.validate({
        rules: {
            intIdCuadroInsmoPrdcto: "required",
            numCntdadExprtda: "required",
            numVlorFOB: "required",
            numPsoNto: "required",
            varCdItem: "required"
        },
        messages: {
            intIdCuadroInsmoPrdcto: "Asegúrese de ingresar el producto",
            numCntdadExprtda: "Asegúrese de ingresar la cantidad exportada",
            numVlorFOB: "Asegúrese de ingresar el valor FOB",
            numPsoNto: "Asegúrese de ingresar el peso neto",
            varCdItem: "Asegúrese de ingresar el ítem"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmvDEXDetalleCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            //Ajax
            $.ajax({
                url: ResolveUrl('~/tmvDEXDetalle/SetTmDexDetalle'),
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
                    }
                    else {
                        console.error(data.Message);

                        //Limpiar el botón
                        btnGuardar.button('reset');
                    }

                    UnblockUI();
                },
            });
        }
    });
}

function frmTmvDEXDetalleEditar_Submit() {

    //Validar
    frmTmvDEXDetalleEditar.validate({
        rules: {
            intIdCuadroInsmoPrdcto: "required",
            numCntdadExprtda: "required",
            numVlorFOB: "required",
            numPsoNto: "required",
            varCdItem: "required"
        },
        messages: {
            intIdCuadroInsmoPrdcto: "Asegúrese de ingresar el producto",
            numCntdadExprtda: "Asegúrese de ingresar la cantidad exportada",
            numVlorFOB: "Asegúrese de ingresar el valor FOB",
            numPsoNto: "Asegúrese de ingresar el peso neto",
            varCdItem: "Asegúrese de ingresar el ítem"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmvDEXDetalleEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            //Ajax
            $.ajax({
                url: ResolveUrl('~/tmvDEXDetalle/Update'),
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
                },
            });
        }
    });
}