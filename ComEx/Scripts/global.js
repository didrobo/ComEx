//Variable Global
var pnlMensajes = $("#pnlMensajes");
var listFiltrosDataTable = [];

$(document).on("ready", function () {
    $(".Numero").on("keypress", Numero_KeyPress);
    $(".Decimal").on("keypress", Decimal_KeyPress);
});

function dateJsonToString(fecha) {
    if (fecha !== null) {        
        if (fecha.substr(0, 6) === "/Date(") {
            var dateString = fecha.substr(6);
            var currentTime = new Date(parseInt(dateString));
            var month = currentTime.getMonth() + 1;
            var day = currentTime.getDate();
            var year = currentTime.getFullYear();
            var date = year + "/" + ('0' + month).slice(-2) + "/" + ('0' + day).slice(-2);

            return date;
        } else {
            return fecha;
        }
    } else {
        return null;
    }
}

function dateJsonToString_dd_mm_yyyy(fecha) {
    if (fecha !== null) {
        if (fecha.substr(0, 6) === "/Date(") {
            var dateString = fecha.substr(6);
            var currentTime = new Date(parseInt(dateString));
            var month = currentTime.getMonth() + 1;
            var day = currentTime.getDate();
            var year = currentTime.getFullYear();
            var date = ('0' + day).slice(-2) + "/" + ('0' + month).slice(-2) + "/" + year;

            return date;
        } else {
            return fecha;
        }
    } else {
        return null;
    }
}

function ResolveUrl(url) {
    if (url.indexOf("~/") === 0) {
        url = baseUrl + url.substring(2);
    }
    return url;
}

Number.prototype.formatNumber = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d === undefined ? "." : d,
        t = t === undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

function configExportServer(metodo, nombreTabla, parametros) {
    jQuery.fn.DataTable.Api.register('buttons.exportData()', function (options) {
        if (this.context.length) {
            var parameter = { "listFiltros": listFiltrosDataTable, "parametros": parametros };

            /*if (parametros !== undefined) {
                //parameter.push(parametros);
            }*/

            //console.log(parameter);
            var jsonResult = $.ajax({
                type: 'POST',
                url: metodo,
                data: JSON.stringify(parameter),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    //Do nothing
                },
                async: false
            });

            //Se obtiene el listado de columnas
            dt = $(nombreTabla).DataTable();
            var listColumns = [];
            var columns = dt.settings().init().columns;
            
            dt.columns().every(function (index) {
                if (columns[index].data !== null) {
                    listColumns.push({
                        columnTitle: columns[index].title,
                        columnField: columns[index].data
                    });
                }
            });
            //console.log(jsonResult);
            var headers;
            var myobj_array = $.map(jsonResult.responseJSON.data, function (value, index) {
                var arr = [];
                var hea = [];
                //console.log(value);
                listColumns.forEach(function (col) {
                    //console.log(key);
                    //get the value of name
                    var val;

                    if (col.columnField.substring(0, 3) === "fec") {

                        if (metodo === "GetInformeAnualCIReporte") {
                            val = dateJsonToString_dd_mm_yyyy(value[col.columnField]);
                        }
                        else {
                            val = dateJsonToString(value[col.columnField]);
                        }
                    }
                    else {
                        val = value[col.columnField];
                    }

                    //push the name string in the array
                    arr.push(val);
                    hea.push(col.columnTitle);
                });

                //var arr = jQuery.makeArray(hea);
                //console.log(new Array(hea));
                headers = $.map(hea, function (v) { return v; });
                return [arr];
            });

            //console.log(headers);
            return { body: myobj_array, header: headers };
        }
    });
}

function Numero_KeyPress(event) {
    if (event.which >= 48 && event.which <= 57) {
        return true;
    }
    else
    {
        return false;
    }
}

function Decimal_KeyPress(event)
{
    if ((event.which >= 48 && event.which <= 57) || event.which === 44) {
        return true;
    }
    else {
        return false;
    }
}

function BlockUI(Mensaje) {
    $.blockUI({ message: '<h4><i class="fa fa-spinner fa-spin fa-pulse fa-2x fa-fw margin-bottom"></i> ' + Mensaje + '</h4>' });
}

function UnblockUI() {
    $.unblockUI();
}

