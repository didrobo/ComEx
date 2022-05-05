var mensaje = $("#Mensaje");
var principal = $("#Principal");
var informacion = $("#Informacion");
var page_hd = $("#page-hd");
var tablaPrincipal = $("#table-InventarioCPs");
var fecDesde = $("#fecDesde");
var fecHasta = $("#fecHasta");

$(document).on('ready', function () {

    $("#btnProcesar").on("click", btnProcesar_Click);

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });
});

$(function () {
    $('#fecDesde').datetimepicker();
    $('#fecHasta').datetimepicker({
        useCurrent: false //Important! See issue #1075
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
        principal.fadeOut(1000);
        page_hd.fadeOut();
        //Mostrar la información
        informacion.fadeIn(1000);

        var desde = fecDesde.val();
        var hasta = fecHasta.val();

        //Se crea la tabla
        configExportServer('GetInventarioCPs', tablaPrincipal, [desde, hasta]);

        tablaInventarioCPs = $(tablaPrincipal).DataTable({
            dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
            serverSide: true,
            ajax: {
                type: "POST",
                url: 'GetInventarioCPs',
                contentType: 'application/json; charset=utf-8',
                data: function (data) {
                    data.listFiltros = listFiltrosDataTable;
                    data.parametros = [desde, hasta];

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
            createdRow: function (row, data, dataIndex) {
                if (data['varSemaforo'] === 'Rojo') {
                    $(row).addClass('redClass');
                }
                else if (data['varSemaforo'] === 'Verde') {
                    $(row).addClass('greenClass');
                }
                else if (data['varSemaforo'] === 'Amarillo') {
                    $(row).addClass('yellowClass');
                }
            },
            columns: [
                { title: "intIdCP", data: "intIdCP", visible: false },
                { title: "Cd. CP", data: "varCdCP" },
                { title: "Fec. CP", data: "fecCP", render: dateJsonToString },
                { title: "Nmro. CP", data: "varNmroCP" },
                { title: "F. Aprb. CP", data: "fecAprbcionCP", render: dateJsonToString },
                { title: "Proveedor", data: "varProveedor" },
                { title: "CI", data: "varCI" },
                { title: "Insumo", data: "varDscrpcionInsmo" },
                { title: "Unidad", data: "varUndadCP" },
                { title: "Cnt. CP", data: "numCntdadCp", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
                { title: "Cnt. Consumida", data: "numCntdadCnsmda", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
                { title: "Cnt. Consumida DEX", data: "numCntCnsmdaDEX", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
                { title: "Cnt. Pendiente", data: "numCntdadPndienteCP", render: $.fn.dataTable.render.number(',', '.', 5, ''), className: 'text-right' },
                { title: "Vr. CP", data: "numVlorCpPorCI", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
                { title: "Vr. Consumido", data: "numVlorCnsmdoCpPorCI", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
                { title: "Vr. Pendiente", data: "numVlorPndienteCpPorCI", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
                { title: "Semáforo", data: "varSemaforo" },
                {
                    title: "Archivo", data: "varRtaArchvoAdjnto",
                    render: function (ruta) {
                        return '<a href="' + ruta + '" target="_blank">Ver PDF</a>';
                    }
                }
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
            order: [[0, 'asc']],
            footerCallback: function (row, data, start, end, display) {
                var api = this.api();
                var json = api.ajax.json();

                $(api.column(9).footer()).html($.fn.dataTable.render.number(',', '.', 5, '').display(json.totals.numCntdadCp));
                $(api.column(10).footer()).html($.fn.dataTable.render.number(',', '.', 5, '').display(json.totals.numCntdadCnsmda));
                $(api.column(11).footer()).html($.fn.dataTable.render.number(',', '.', 5, '').display(json.totals.numCntCnsmdaDEX));
                $(api.column(12).footer()).html($.fn.dataTable.render.number(',', '.', 5, '').display(json.totals.numCntdadPndienteCP));
                $(api.column(13).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numVlorCpPorCI));
                $(api.column(14).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numVlorCnsmdoCpPorCI));
                $(api.column(15).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numVlorPndienteCpPorCI));
            }
        });
    }
}