//Variables
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");
var frmTmvFacturasExportacionDetalleCrear = $("#frmTmvFacturasExportacionDetalleCrear");
var frmTmvFacturasExportacionDetalleEditar = $("#frmTmvFacturasExportacionDetalleEditar");

$(document).on("ready", inicio);

function inicio() {
    //Métodos
    frmTmvFacturasExportacionDetalleCrear_Submit();
    frmTmvFacturasExportacionDetalleEditar_Submit();
}

function frmTmvFacturasExportacionDetalleCrear_Submit() {

    //Validar
    frmTmvFacturasExportacionDetalleCrear.validate({
        rules: {
            intIdPrdctoFctrcion: "required",
            numCntdad: "required",
            numVlorUntrio: "required",
            numPsoBrto: "required",
            numPsoNto: "required",
            numSbttal: "required",
            intItem: "required"
        },
        messages: {
            intIdPrdctoFctrcion: "Asegúrese de ingresar el producto",
            numCntdad: "Asegúrese de ingresar la cantidad",
            numVlorUntrio: "Asegúrese de ingresar el valor unitario",
            numPsoBrto: "Asegúrese de ingresar el peso bruto",
            numPsoNto: "Asegúrese de ingresar el peso neto",
            numSbttal: "Asegúrese de ingresar el subtotal",
            intItem: "Asegúrese de ingresar el ítem"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmvFacturasExportacionDetalleCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            //Ajax
            $.ajax({
                url: ResolveUrl('~/tmvFacturasExportacionDetalle/SetTmFacturaDetalle'),
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

function frmTmvFacturasExportacionDetalleEditar_Submit() {

    //Validar
    frmTmvFacturasExportacionDetalleEditar.validate({
        rules: {
            intIdPrdctoFctrcion: "required",
            numCntdad: "required",
            numVlorUntrio: "required",
            numPsoBrto: "required",
            numPsoNto: "required",
            numSbttal: "required",
            intItem: "required"
        },
        messages: {
            intIdPrdctoFctrcion: "Asegúrese de ingresar el producto",
            numCntdad: "Asegúrese de ingresar la cantidad",
            numVlorUntrio: "Asegúrese de ingresar el valor unitario",
            numPsoBrto: "Asegúrese de ingresar el peso bruto",
            numPsoNto: "Asegúrese de ingresar el peso neto",
            numSbttal: "Asegúrese de ingresar el subtotal",
            intItem: "Asegúrese de ingresar el ítem"
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmvFacturasExportacionDetalleEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            //Ajax
            $.ajax({
                url: ResolveUrl('~/tmvFacturasExportacionDetalle/Update'),
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