var mensaje = $("#Mensaje");
var principal = $("#Principal");
var informacion = $("#Informacion");
var page_hd = $("#page-hd");
var tablaPrincipal = $("#table-UIAFImpExp");
var intAno = $("#intAno");
var intCuatrimestre = $("#intCuatrimestre");

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

    //Validar el cuatrimestre
    if (intCuatrimestre.val() === "" || intCuatrimestre.val() === " " || intCuatrimestre === undefined || intCuatrimestre === null) {
        intCuatrimestre.parent().children(".ocultar").fadeIn();
        intCuatrimestre.parent().addClass("has-error");
        success = false;
    }

    //Si no hay algún error agregamos los datos y ejecutamos el ajax
    if (success) {
        principal.fadeOut(1000);
        page_hd.fadeOut();
        //Mostrar la información
        informacion.fadeIn(1000);

        var ano = intAno.val();
        var cuatrimestre = intCuatrimestre.val();

        //Se crea la tabla
        configExportServer('GetInformeUIAFImpExp', tablaPrincipal, [ano, cuatrimestre]);

        tablaUIAFCompVent = $(tablaPrincipal).DataTable({
            dom: "<'row'<'col-md-6'B><'col-md-6'f>><'row'<'col-md-12'tr>><'row'<'col-md-2'l><'col-md-4'i><'col-md-6'p>>",
            serverSide: true,
            ajax: {
                type: "POST",
                url: 'GetInformeUIAFImpExp',
                contentType: 'application/json; charset=utf-8',
                data: function (data) {
                    data.listFiltros = listFiltrosDataTable;
                    data.parametros = [ano, cuatrimestre];

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
                { title: "Consecutivo", data: "intConsec" },
                { title: "F. Transacción", data: "fecha" },
                { title: "Vr. Transacción", data: "numVlrfob", render: $.fn.dataTable.render.number('', '.', 2, ''), className: 'text-right' },
                { title: "Vr. Transacción Pesos", data: "numVlrfob2", render: $.fn.dataTable.render.number('', '.', 2, ''), className: 'text-right' },
                { title: "Tipo Moneda", data: "varTipomon" },
                { title: "Tipo Documento", data: "varTipoDoc" },
                { title: "Identificación", data: "varNit" },
                { title: "Primer Apellido", data: "varPrimerApellido" },
                { title: "Segundo Apellido", data: "varSegundoApellido" },
                { title: "Primer Nombre", data: "varPrimerNombre" },
                { title: "Otros Nombres", data: "varSegundoNombre" },
                { title: "Razón Social", data: "varRazonSocial" },
                { title: "Cd. Dpto/Mpio", data: "varCoddane" },
                { title: "Tipo Transacción", data: "intTipotra" },
                { title: "Tipo Documento", data: "varNitdetalle" },
                { title: "Identificación", data: "varNit2" },
                { title: "Primer Apellido", data: "varPrimerApellido1" },
                { title: "Segundo Apellido", data: "varSegundoApellido1" },
                { title: "Primer Nombre", data: "varPrimerNombre1" },
                { title: "Otros Nombres", data: "varSegundoNombre1" },
                { title: "Razón Social", data: "varRazonSocial1" },
                { title: "País", data: "varPais" },
                { title: "Ciudad", data: "varCiudad" },
                { title: "Partida Arancelaria", data: "varPartidaa" },
                { title: "No. Declaración", data: "varDex" },
                { title: "Forma de Pago", data: "intFormap" },
                { title: "Detalles", data: "varDetalle" }
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

                $(api.column(2).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numVlrfob));
                $(api.column(3).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(json.totals.numVlrfob2));

            }
        });
    }
}