$(document).on('ready', function () {
    crearTabla();

    $(".calendario").datetimepicker({
        locale: "es",
        format: "DD/MM/YYYY"
    });
});

function crearTabla() {

    configExportServer('GetComprasReporte', '#table-compras-reporte');

    tablaCompras = $('#table-compras-reporte').DataTable({
        dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
        serverSide: true,
        ajax: {
            type: "POST",
            url: 'GetComprasReporte',
            contentType: 'application/json; charset=utf-8',
            data: function (data) {
                data.listFiltros = listFiltrosDataTable;
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
            { title: "intIdCmpra", data: "intIdCmpra", visible: false },
            { title: "Cargue", data: "intCnsctvoCrgue" },
            { title: "Lote", data: "varNumLte" },
            { title: "Aux", data: "varCdAuxliarCmpra" },
            { title: "Factura", data: "varNmroFctra" },
            { title: "Fecha", data: "fecCmpra", render: dateJsonToString },
            { title: "Proyecto", data: "varPrycto" },
            { title: "Llave", data: "varLlve" },
            { title: "Documento", data: "varNitPrvdor" },
            { title: "Cliente", data: "varNmbrePrvdor" },
            { title: "Material", data: "varDscrpcionInsmo" },
            { title: "Gramos", data: "numCntdadFctra", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Finos", data: "numFnos", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Finos AG", data: "numFnosAG", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Finos PT", data: "numFnosPT", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Ley", data: "numLey", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Ley AG", data: "numLeyAG", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Ley PT", data: "numLeyPT", render: $.fn.dataTable.render.number(',', '.', 2, ''), className: 'text-right' },
            { title: "Vr. Unitario", data: "numVlorUntrio", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Regalías", data: "numRglias", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Regalías AG", data: "numRgliasAG", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Regalías PT", data: "numRgliasPT", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Retención", data: "numVlorRtncionFuente", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Vr. Pagar", data: "numValorPagar", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Vr. Antes Ret.", data: "numValorAntesRetencion", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Subtotal", data: "numSbttal", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
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
            { title: "Ciudad", data: "varDscrpcionCiudad" },
            { title: "Dirección", data: "varDrccion" },
            { title: "Id Régimen", data: "intIdRgmen" },
            { title: "Régimen", data: "varNbreRgmen" },
            { title: "Año Compra", data: "intAnoCmpra" },
            { title: "Mes Compra", data: "intMesCmpra" },
            { title: "Día Compra", data: "intDiaCmpra" },
            { title: "Ciudad Regalías", data: "varCiudadRglias" },
            { title: "Dpto. Regalías", data: "varDepartamentoRglias" },
            { title: "Cd. CP", data: "varCdCP" },
            { title: "Fec. CP", data: "fecCp", render: dateJsonToString },
            { title: "Nmro. CP", data: "varNmroCP" },
            { title: "F. Aprb. CP", data: "fecAprbcionCP", render: dateJsonToString },

            { title: "Aux. DEX", data: "varCdAuxliarDEX" },
            { title: "Fec. Aux. DEX", data: "fecAuxliar", render: dateJsonToString },
            { title: "Nmro. DEX", data: "varNmroDEX" },
            { title: "F. Aprb. DEX", data: "fecAprbcionDEX", render: dateJsonToString },
            { title: "Vr. FOB", data: "numVlorFOB", render: $.fn.dataTable.render.number(',', '.', 2, '$'), className: 'text-right' },
            { title: "Comprador", data: "varNmbreCmprdor" },
            { title: "País Comprador", data: "varDscrpcionPais" },
            { title: "Exportación", data: "varNmroExprtcion" }
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
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        order: [[2, 'desc']],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();
            var json = api.ajax.json();

            $(api.column(12).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numCntdadFctra));
            $(api.column(13).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numFnos));
            $(api.column(14).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numFnosAG));
            $(api.column(15).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numFnosPT));

            $(api.column(20).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numRglias));
            $(api.column(21).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numRgliasAG));
            $(api.column(22).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numRgliasPT));
            $(api.column(23).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numVlorRtncionFuente));
            $(api.column(24).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numValorPagar));
            $(api.column(25).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numValorAntesRetencion));
            $(api.column(26).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numSbttal));

            $(api.column(45).footer()).html($.fn.dataTable.render.number(',', '.', 2, '$').display(json.totals.numVlorFOB));

        }
    });
}