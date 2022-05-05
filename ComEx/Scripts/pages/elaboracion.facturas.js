var fecDesde = $("#fecDesde");
var fecHasta = $("#fecHasta");

$(document).on('ready', function () {

    //Eventos
    $("#btnProcesar").on("click", btnProcesar_Click);

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });
});

$(function () {
    $('#fecDesde').datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });
    $('#fecHasta').datetimepicker({
        useCurrent: false, //Important! See issue #1075
        locale: "es",
        format: "DD/MM/YYYY"
    });
    $("#fecDesde").on("dp.change", function (e) {
        $('#fecHasta').data("DateTimePicker").minDate(e.date).locale("es").format("DD/MM/YYYY");
    });
    $("#fecHasta").on("dp.change", function (e) {
        $('#fecDesde').data("DateTimePicker").maxDate(e.date).locale("es").format("DD/MM/YYYY");
    });
});

function btnProcesar_Click(e) {

    //Variables
    var success = true;

    //Validar la fecha desde
    if (fecDesde.val() === "" || fecDesde.val() === " " || fecDesde === undefined || fecDesde === null) {
        fecDesde.parent().children(".ocultar").fadeIn();
        fecDesde.parent().addClass("has-error");
        success = false;
    }

    //Validar la fecha hasta
    if (fecHasta.val() === "" || fecHasta.val() === " " || fecHasta === undefined || fecHasta === null) {
        fecHasta.parent().children(".ocultar").fadeIn();
        fecHasta.parent().addClass("has-error");
        success = false;
    }

    //Si no hay algún error agregamos los datos y ejecutamos el ajax
    if (success) {

        var desde = fecDesde.val();
        var hasta = fecHasta.val();

        $.ajax({
            url: ResolveUrl('~/GeneracionInformes/GenerarElaboracionFacturas'),
            type: "POST",
            data: JSON.stringify({ fecDesde: desde, fecHasta: hasta }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            cache: false,
            beforeSend: function () {
                $("#btnProcesar").button('loading');
                BlockUI("Procesando archivos...");
            },
            success: function (resultado) {
                if (resultado.success) {

                    $("#btnProcesar").text('Procesar');
                    $("#btnProcesar").prop("disabled", true);

                    pnlMensajes.html('<div class="alert alert-success alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>'
                        + resultado.mensaje + ' <a href="' + ResolveUrl('~/Explorer/Facturas') + '" target="_blank">Abrir Carpeta</a></div>')
                        .fadeIn(100);
                }
                else {
                    $("#btnProcesar").button('reset');
                    pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + resultado.error + '</div>')
                        .fadeIn(100);
                }
            },
            error: function (data, success, error) {
                console.log(data);
                $("#btnProcesar").button('reset');
                pnlMensajes.html('<div class="alert alert-danger alert-dismissable"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a><strong>Error! </strong>' + data.responseJSON.error + '</div>')
                    .fadeIn(100);
            },
            complete: function () {
                UnblockUI();
            }
        });
    }
}