var mensaje = $("#Mensaje");
var principal = $("#Principal");
var informacion = $("#Informacion");
var page_hd = $("#page-hd");
var tablaPrincipal = $("#table-informeAnualCI");
var intAno = $("#intAno");

$(document).on("ready", (function () {
    $("#btnProcesar").on("click", btnProcesar_Click);
}));

function btnProcesar_Click(e) {

    //Variables
    var success = true;

    //Validar el año
    if (intAno.val() === "" || intAno.val() === " " || intAno === undefined || intAno === null) {
        intAno.parent().children(".ocultar").fadeIn();
        intAno.parent().addClass("has-error");
        success = false;
    }

    //Si no hay algún error agregamos los datos y ejecutamos el ajax
    if (success) {

        principal.fadeOut(1000);
        page_hd.fadeOut();
        //Mostrar la información
        informacion.fadeIn(1000);

        var ano = intAno.val();

        //Se crea la tabla
        configExportServer('GetInformeAnualCIReporte', tablaPrincipal, [ano]);

        tablaInformeAnual = $(tablaPrincipal).DataTable({
            dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
            serverSide: true,
            ajax: {
                type: "POST",
                url: 'GetInformeAnualCIReporte',
                contentType: 'application/json; charset=utf-8',
                data: function (data) {
                    data.listFiltros = listFiltrosDataTable;
                    data.parametros = [ano];

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
                { title: "No.", data: "intNum" },
                { title: "CP", data: "varCdCP" },
                { title: "F. Bimestre", data: "fecBmstre", render: dateJsonToString_dd_mm_yyyy },
                { title: "F. CP", data: "fecCP", render: dateJsonToString_dd_mm_yyyy },
                { title: "No. Ítem", data: "intNumItem" },
                { title: "Factura", data: "varNmroFctra" },
                { title: "F. Factura", data: "fecCmpra", render: dateJsonToString_dd_mm_yyyy },
                { title: "Vr. CP", data: "numVlorCp", render: $.fn.dataTable.render.number(',', '.', 0, '$'), className: 'text-right' },
                { title: "Vr. Total CP", data: "numVlorTtalAnual", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
                { title: "Prod. Sin Trans", data: "varPrdctoSinTrnsfrmar", className: 'text-center' },
                { title: "Materia Prima", data: "varMtriaPrima" },
                { title: "Serv. Int. Producción", data: "varSrvcioIntrmdioPrdccion" },
                //{ title: "F. Envío CP", data: "fecEnvioCP", render: dateJsonToString_dd_mm_yyyy },
                { title: "DEX", data: "varNmroDEX" },
                { title: "F. DEX", data: "fecAprbcionDEX", render: dateJsonToString_dd_mm_yyyy },
                { title: "Aduana", data: "varCdAduana" },
                { title: "Vr. FOB DEX", data: "numVlrFobUSD", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
                { title: "F. Embarque", data: "fecEmbrque", render: dateJsonToString_dd_mm_yyyy },
                { title: "Vr. Total FOB", data: "numVlrTtalFobUSD", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' }
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
            order: [[0, 'asc']]
        });
    }
}