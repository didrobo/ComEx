var file = $("#file");
var mensaje = $("#Mensaje");
var principal = $("#Principal");
var page_hd = $("#page-hd");
var informacion = $("#Informacion");
var tablaPrincipal = $("#tablaResultados");

$(document).on("ready", (function () {
    $("#file").fileinput({
        uploadUrl: "",
        uploadAsync: true,
        showUpload: false,
        showRemove: false,
        showPreview: false,
        language: "es",
        mainClass: "input-group-lg",
        maxFileSize: 10240,
        allowedFileExtensions: ['xml']
    });

    $("#btnProcesar").on("click", btnProcesar_Click);  
    file.on("change", file_Change);
}));

function btnProcesar_Click(e) {

    //Variables
    var success = true;
    var formData = new FormData();

    //Validaciones
    //Validar que el archivo contega algún dato.
    if (file.val() === "" || file.val() === " " || file === undefined || file === null || file.val().length < 2) {
        var parent = file.parent().parent().parent().parent().parent();
        parent.children(".ocultar").fadeIn();
        parent.children().addClass("has-error");
        success = false;
    }

    //Si no hay algún error agregamos los datos y ejecutamos el ajax
    if (success) {
        //Agregar los datos en el formData
        formData.append("Archivo", file[0].files[0]);

        //Ajax
        $.ajax({
            url: ResolveUrl("~/tmRemisionDocumentos/ProcesarXmlRemisiones"),
            type: "POST",
            contentType: false,
            cache: false,
            dataType: "json",
            processData: false,
            data: formData,
            beforeSend: function () {
                principal.fadeOut(1000);
                mensaje.html('<div class="alert alert-info text-center"><i class="fa fa-cog fa-spin fa-3x fa-fw margin-bottom"></i> Procesando archivo, por favor espere.</div>');
            },
            success: function (resultado) {
                if (resultado.success) {
                    mensaje.html('<div class="alert alert-info" role="alert">' + resultado.Message + '</div>')
                        .fadeIn(1000).slideToggle(1000);
                    //console.log(resultado);
                    
                    ////Llamar función para cargar los datos
                    TablaDatos(resultado.data);

                    page_hd.fadeOut();
                    ////Mostrar la información
                    informacion.fadeIn(1000);
                }
                else {
                    mensaje.html('<div class="alert alert-danger alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + resultado.Message + '</div>')
                        .fadeIn(1000);

                    principal.fadeIn(1000);
                }
            },
            error: function (jqXHR) {
                mensaje.html('<div class="alert alert-danger alert-dismissable" role="alert"><a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>' + jqXHR.statusText + '</div>')
                    .fadeIn(1000);
            }
        });
    }
}

function file_Change(e) {
    if (file.val().length > 2) {
        var parent = file.parent().parent().parent().parent().parent();
        parent.children(".ocultar").fadeOut();
        parent.children().removeClass("has-error");
    }
}

function TablaDatos(data) {
    tablaPrincipal = tablaPrincipal.DataTable({
        dom: "<'row'<'col-md-6'><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-12'i>>",
        data: data,
        processing: true,
        colReorder: true,
        scrollX: true,
        scrollY: 420,
        responsive: true,
        scrollCollapse: true,
        scroller: {
            loadingIndicator: true
        },
        paging: false,
        autoWidth: false,
        deferRender: true,
        columns: [
            { title: "Documento", data: "tmProveedores.varCdPrvdor" },
            { title: "Proveedor", data: "tmProveedores.varNmbre" },
            { title: "Mes", data: "intMes" },
            { title: "Fec. Envío", data: "fecEnvio", render: dateJsonToString },
            { title: "Guía Envío", data: "varNumGuiaEnvio" },
            { title: "Ciudad", data: "tmProveedores.tmCiudades.varDscrpcionCiudad" },
            { title: "F. Recibido", data: "fecRcbdo", render: dateJsonToString },
            { title: "F. Entrega Cli.", data: "fecEntrgaCliente", render: dateJsonToString },
            { title: "Quien Recibe", data: "varQuienRcbe" },
            { title: "F. Despacho", data: "fecDspcho", render: dateJsonToString },
            { title: "Guía Despacho", data: "varNumGuiaDspacho" }
        ],
        order: [[3, 'desc']]
    });

    $('#thFecEnvio').click();
}