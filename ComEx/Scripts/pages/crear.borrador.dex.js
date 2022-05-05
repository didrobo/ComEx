var borradores = [];
var lotes = [];

$(document).on('ready', function () {
    cargarComboBorradorDEX();
    cargarComboComprador();
    cargarComboLote();

    //Eventos
    $("#cmbBorrador").on("change", cmbBorrador_Change);
    $('#formCrearBorradorDEX').on('submit', formCrearBorradorDEX_submit);    
});

function cargarComboBorradorDEX() {
    $("#cmbBorrador").select2({
        allowClear: true,
        placeholder: "Nuevo",
        language: "es",
        ajax: {
            url: ResolveUrl('~/OperacionesMasivas/GetListBorradores'),
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
                //console.log(data);
                borradores = data.borradores;
                lotes = data.lotes;

                return {
                    results: $.map(data.borradores, function (item) {
                        return {
                            text: item.varCdAuxliar + ' - ' + dateJsonToString(item.fecAuxliar),
                            id: item.intIdDEX
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 3
    });
}

function cargarComboComprador() {
    $("#cmbComprador").select2({
        language: "es",
        ajax: {
            url: ResolveUrl('~/tmCompradores/GetCompradorByNameOrNit'),
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
                            text: item.varNmbre,
                            id: item.intIdCmprdor
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 3
    });
}

function cargarComboLote() {
    $("#cmbLote").select2({
        language: "es",
        ajax: {
            url: ResolveUrl('~/OperacionesMasivas/GetListLotesSinDEX'),
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
                            text: item.lote,
                            id: item.lote
                        }
                    })
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 3
    });
}

function cmbBorrador_Change() {

    var borrador = $("#cmbBorrador").val();
    $("#btnProcesarBorradorDEX").prop("disabled", false);

    if (borrador == null) {
        // Se habilitan los controles cuando es un nuevo borrador
        $("#txtNumBorrador").prop("disabled", false);
        $("#cmbComprador").prop("disabled", false);

        $("#txtNumBorrador").val("");
        $("#cmbComprador").val(null).trigger("change");
        $("#cmbLote").val(null).trigger("change");
    }
    else {
        // Se deshabilitan los controles cuando es un borrador existente
        $("#txtNumBorrador").prop("disabled", true);
        $("#cmbComprador").prop("disabled", true);

        // Se busca el borrador seleccionado en el array
        var borradorSelected = jQuery.grep(borradores, function (value) {
            return value.intIdDEX == borrador;
        });

        // Se buscan los lotes seleccionados en el array
        var loteSelected = jQuery.grep(lotes, function (value) {
            return value.intIdDEX == borrador && value.varNumLte != null;
        });


        // Se asignan valores a los controles
        if (borradorSelected.length > 0) {
            $("#txtNumBorrador").val(borradorSelected[0].varCdAuxliar);
            $("#cmbComprador").empty().append('<option value="' + borradorSelected[0].intIdCmprdor + '">' + borradorSelected[0].varNmbre + '</option>').val(borradorSelected[0].intIdCmprdor).trigger('change');
        } else {
            $("#txtNumBorrador").val("");
            $("#cmbComprador").val(null).trigger("change");
        }
        //console.log(loteSelected);
        if (loteSelected.length > 0) {
            var combo = "";
            loteSelected.forEach(function (col) {
                combo += "<option value='" + col.varNumLte + "' selected>" + col.varNumLte + "</option>";
            });

            $("#cmbLote").empty().append(combo).trigger("change");
        } else {
            $("#cmbLote").val(null).trigger("change");
        }
    }
}

// Submit Formulario
function formCrearBorradorDEX_submit(e) {
    
    var formCrearBorradorDEX = $("#formCrearBorradorDEX");

    //Validar
    formCrearBorradorDEX.validate({
        rules: {
            txtNumBorrador: "required",
            cmbComprador: "required",
            cmbLote: "required"
        },
        messages: {
            txtNumBorrador: "Debe seleccionar un borrador existente o crear uno nuevo",
            cmbComprador: "Debe seleccionar un comprador",
            cmbLote: "Debe seleccionar uno o más lotes"
        },
        submitHandler: function (Frm) {
            // Find disabled inputs, and remove the "disabled" attribute
            var disabled = formCrearBorradorDEX.find(':input:disabled').removeAttr('disabled');

            var formData = new FormData();
            formData = formCrearBorradorDEX.serializeArray();

            // re-disabled the set of inputs that you previously enabled
            disabled.attr('disabled', 'disabled');

            $.ajax({
                url: ResolveUrl('~/OperacionesMasivas/CrearBorradorDEX'),
                type: "POST",
                data: JSON.stringify({ datosBorradorDEX: formData }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                cache: false,
                beforeSend: function () {
                    $("#btnProcesarBorradorDEX").button('loading');
                    BlockUI("Procesando borrador del DEX...");
                },
                success: function (resultado) {
                    if (resultado.success) {

                        $("#btnProcesarBorradorDEX").text('Procesar');
                        $("#btnProcesarBorradorDEX").prop("disabled", true);                        
                        
                        pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                            + resultado.mensaje + '</div>')
                                        .fadeIn(100);
                    }
                    else {
                        $("#btnProcesarBorradorDEX").button('reset');
                        pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                                        .fadeIn(100);
                    }                  
                },
                error: function (data, success, error) {
                    $("#btnProcesarBorradorDEX").button('reset');
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.Message + '</div>')
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