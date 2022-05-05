
$(document).on('ready', function () {
    cargarComboLote();

    //Eventos
    $('#formGenerarArchivoContable').on('submit', formGenerarArchivoContable_submit);
});

function cargarComboLote() {
    $("#cmbLote").select2({
        language: "es",
        placeholder: "Seleccionar lote...",
        allowClear: true,
        ajax: {
            url: ResolveUrl('~/tmCompras/GetListLotes'),
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    paramSearch: params.term
                };
            },
            processResults: function (data) {
                // parse the results into the format expected by Select2
                // since we are using custom formatting functions we do not need to
                // alter the remote JSON data, except to indicate that infinite
                // scrolling can be used
                return {
                    results: $.map(data, function (item) {
                        return {
                            text: "Lote: " + item.lote + " # Compras: " + item.cantCmpras,
                            id: item.lote
                        };
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 3
    });
}

// Submit Formulario
function formGenerarArchivoContable_submit(e) {

    var formGenerarArchivoContable = $("#formGenerarArchivoContable");

    //Validar
    formGenerarArchivoContable.validate({

        rules: {
            cmbTipoArchivo: "required",
            cmbLote: "required"
        },
        messages: {
            cmbTipoArchivo: "Debe seleccionar un tipo de archivo",
            cmbLote: "Debe seleccionar un lote"
        },
        submitHandler: function (Frm) {

            var formData = new FormData();
            formData = formGenerarArchivoContable.serializeArray();

            $.ajax({
                url: ResolveUrl('~/GeneracionInformes/GenerarArchivoContable'),
                type: "POST",
                data: JSON.stringify({ datosArchivoContable: formData }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                cache: false,
                beforeSend: function () {
                    $("#btnProcesarArchivoContable").button('loading');
                    BlockUI("Procesando archivo contable...");
                },
                success: function (resultado) {
                    if (resultado.success) {

                        $("#btnProcesarArchivoContable").text('Procesar');
                        $("#btnProcesarArchivoContable").prop("disabled", true);

                        pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                            + resultado.mensaje + ' <a href="' + ResolveUrl('~/Explorer/Contables') + '" target="_blank">Abrir Carpeta</a></div>')
                            .fadeIn(100);
                    }
                    else {
                        $("#btnProcesarArchivoContable").button('reset');
                        pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                            .fadeIn(100);
                    }
                },
                error: function (data, success, error) {
                    console.log(data);
                    $("#btnProcesarArchivoContable").button('reset');
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.error + '</div>')
                        .fadeIn(100);
                },
                complete: function () {
                    UnblockUI();
                }
            });
        }
    });

    e.preventDefault();
}