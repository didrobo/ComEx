//Variables
var btnGuardar = $("#btnGuardar");
var btnEditar = $("#btnEditar");
var frmTmComprasDetalleCrear = $("#frmTmComprasDetalleCrear");
var frmTmComprasDetalleEditar = $("#frmTmComprasDetalleEditar");

$(document).on("ready", inicio);

function inicio() {
	//Métodos
    frmTmComprasDetalleCrear_Submit();
    frmTmComprasDetalleEditar_Submit();
}

function frmTmComprasDetalleCrear_Submit()
{
    //Validar
    frmTmComprasDetalleCrear.validate({
        rules: {
            intIdInsmo: "required",
            numCntdadFctra: "required",
            numVlorUntrio: "required",
            numCntdadCP: "required",
            numSbttal: "required",
            numFnos: "required",
            numLey: "required",
        },
        messages: {
            intIdInsmo: "Asegúrese de seleccionar el insumo.",
            numCntdadFctra: "Asegúrese de ingresar la cantidad factura.",
            numVlorUntrio: "Asegúrese de ingresar el valor unitario.",
            numCntdadCP: "Asegúrese ingresar la cantidad CP.",
            numSbttal: "Asegúrese de ingresar el subtotal.",
            numFnos: "Asegúrese de ingresar el fino.",
            numLey: "Asegúrese de ingresar la ley."
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmComprasDetalleCrear.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            //Ajax
            $.ajax({
                url: ResolveUrl('~/tmComprasDetalle/SetTmComprasDetalle'),
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

function frmTmComprasDetalleEditar_Submit()
{
    //Validar
    frmTmComprasDetalleEditar.validate({
        rules: {
            intIdInsmo: "required",
            numCntdadFctra: "required",
            numVlorUntrio: "required",
            numCntdadCP: "required",
            numSbttal: "required",
            numFnos: "required",
            numLey: "required",
        },
        messages: {
            intIdInsmo: "Asegúrese de seleccionar el insumo.",
            numCntdadFctra: "Asegúrese de ingresar la cantidad factura.",
            numVlorUntrio: "Asegúrese de ingresar el valor unitario.",
            numCntdadCP: "Asegúrese ingresar la cantidad CP.",
            numSbttal: "Asegúrese de ingresar el subtotal.",
            numFnos: "Asegúrese de ingresar el fino.",
            numLey: "Asegúrese de ingresar la ley."
        },
        submitHandler: function (Frm) {
            var formData = new FormData();
            $.each(frmTmComprasDetalleEditar.serializeArray(), function (index, value) {
                formData.append(value.name, value.value);
            });

            //Ajax
            $.ajax({
                url: ResolveUrl('~/tmComprasDetalle/Update'),
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