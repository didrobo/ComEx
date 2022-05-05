var listColumns = [];
var listOperations = [
    { columnField: "like", columnTitle: "Contiene" },
    { columnField: "=", columnTitle: "Igual" },
    { columnField: "!=", columnTitle: "Diferente" },
    { columnField: "IsEmpty", columnTitle: "Está vacío" },
    { columnField: "IsNotEmpty", columnTitle: "No está vacío" },
    { columnField: ">", columnTitle: "Mayor que" },
    { columnField: "<", columnTitle: "Menor que" },
    { columnField: ">=", columnTitle: "Mayor o igual que" },
    { columnField: "<=", columnTitle: "Menor o igual que" }
];
var listLogicOperators = [
    { columnField: "and", columnTitle: "AND" },
    { columnField: "or", columnTitle: "OR" }
];
var contRows = 0;
var tbl;
var ejecutarFiltro = false;

function filtrarDataTable(dt) {    
    var columns = dt.settings().init().columns;    
    tbl = dt;

    listColumns = [];

    dt.columns().every(function (index) {
        if (columns[index].data != null) {
            listColumns.push({
                columnTitle: columns[index].title,
                columnField: columns[index].data
            });
        }
    });

    showBSModal({
        title: "Filtrar Tabla",
        body: "<a class='icn_addFilter' tabindex='0' href='#' title='Agregar Condición'><span><i class='fa fa-plus-circle'></i></span></a><div id='condicionesFilterForm'></div>",
        size: "large",
        actions: [
            {
                label: 'Filtrar',
                cssClass: 'btn-success',
                onClick: function (e) {
                    tbl.draw();
                    $(e.target).parents('.modal').modal('hide');
                }
            },
            {
            label: 'Cerrar',
            cssClass: 'btn-danger',
            onClick: function (e) {
                    $(e.target).parents('.modal').modal('hide');
                }
            }
        ],
        onShow: function (e) {
            $("a.icn_addFilter").on("click", "span i", function () {                
                ejecutarFiltro = true;
                agregarCondicion();
            });
        }
    });

    llenarCondiciones();
}

function agregarCondicion() {

    var divBody = $("#condicionesFilterForm");
    var contRows = divBody.find("div.row").length;

    var htmlCondicion = "<div class='row'>";
    
    if (contRows > 0) {
        htmlCondicion += "<div class='col-sm-2'>";
        htmlCondicion += "<div class='form-group'>";
        htmlCondicion += "<label>Operador</label>";
        htmlCondicion += getCombo(listLogicOperators, "listOperator-" + contRows);
        htmlCondicion += "</div>";
        htmlCondicion += "</div>";
    }

    htmlCondicion += "<div class='col-md-3'>";
    htmlCondicion += "<div class='form-group'>";
    htmlCondicion += "<label>Campo</label>";
    htmlCondicion += getCombo(listColumns, "listColumns-" + contRows);
    htmlCondicion += "</div>";
    htmlCondicion += "</div>";
    htmlCondicion += "<div class='col-md-2'>";
    htmlCondicion += "<div class='form-group'>";
    htmlCondicion += "<label>Operación</label>";
    htmlCondicion += getCombo(listOperations, "listOperations-" + contRows);
    htmlCondicion += "</div>";
    htmlCondicion += "</div>";
    htmlCondicion += "<div class='valor col-md-4'>";
    htmlCondicion += "</div>";
    htmlCondicion += "<div class='col-sm-1'>";
    htmlCondicion += "<a class='icn_removeFilter-" + contRows + "' tabindex='0' href='#' title='Eliminar Condición'><span><i class='del fa fa-minus-circle' intRow='" + contRows + "'></i></span></a>";
    htmlCondicion += "</div>";
    htmlCondicion += "</div>";

    divBody.append(htmlCondicion).on("click", "a.icn_removeFilter-" + contRows + " span i", function () {
        eliminarCondicion($(this));
    }).on("change", "select.listColumns-" + contRows, function () {
        crearControl($(this));
    }).on("change", "select.listOperations-" + contRows, function () {
        var logOp = $(this).val();
        var nameLogOp = $(this).attr("name");
        var intCond = nameLogOp.substring(nameLogOp.indexOf("-") + 1);

        if (logOp == "IsEmpty" || logOp == "IsNotEmpty") {

            var divRow = $(this).parent().parent().parent();
            var columInputVal = divRow.find(".valor");
            var columnField = $(".listColumns-" + intCond).val();
            var colOperator = $(".listOperator-" + intCond).val();

            columInputVal.empty();

            listFiltrosDataTable = jQuery.grep(listFiltrosDataTable, function (value) {
                return value.intCondicion != intCond;
            });

            listFiltrosDataTable.splice(intCond, 0, {
                intCondicion: intCond,
                columnField: columnField,
                logicOperator: logOp,
                value: "",
                operator: colOperator
            });

            if (!tbl.page.info().serverSide) {
                tbl.draw();
            }
        }
        else {
            crearControl($(".listColumns-" + intCond));
        }

    }).on("change", "select.listOperator-" + contRows, function () {

        var colOperator = $(this).val();
        var nameOperator = $(this).attr("name");
        var intCond = nameOperator.substring(nameOperator.indexOf("-") + 1);

        var condicion = jQuery.grep(listFiltrosDataTable, function (value) {
            return value.intCondicion === intCond;
        });

        if (condicion.length > 0) {
            condicion[0].operator = colOperator;
        }

    });

    contRows = contRows + 1;
}

function eliminarCondicion(iconRemove) {
    var columnIcon = iconRemove.closest('div');
    var intCondicion = iconRemove.attr("intRow");

    listFiltrosDataTable = jQuery.grep(listFiltrosDataTable, function (value) {
        return value.intCondicion != intCondicion;
    });

    columnIcon.parent().remove();

    if (!tbl.page.info().serverSide) {
        tbl.draw();
    }
}

function getCombo(list, name) {

    var combo = "<select class='form-control " + name + "' name='" + name + "'>";

    list.forEach(function (col) {
        combo += "<option value='" + col.columnField + "'>" + col.columnTitle + "</option>";
    });

    combo += "</select>";
    return combo;
}

function crearControl(combo) {
    
    var nameOperator = combo.attr("name").replace("listColumns", "listOperations");
    var nameLogicOperator = combo.attr("name").replace("listColumns", "listOperator");

    //console.log(nameOperator);
    var columnField = combo.val();
    var divRow = combo.parent().parent().parent();
    var columInputVal = divRow.find(".valor");    
    var control = "";
    var indexColum;
    var intCond = nameOperator.substring(nameOperator.indexOf("-") + 1);
    var logicOpe = $("." + nameOperator).val();
    var operator = $("." + nameLogicOperator).val();

    tbl.columns().every(function (index) {
        if (tbl.settings().init().columns[index].data == columnField) {
            indexColum = index;
        }
    });

    var columTbl = tbl.columns(indexColum);

    // Se la operación lógica es "Está Vacío" o "No está vacío" no se crea ningún input
    if (logicOpe != "IsEmpty" && logicOpe != "IsNotEmpty") {
        switch (columnField.substring(0, 3)) {
            case "var":
                control = '<div class="form-group"><label>Valor</label><input class="input-val-' + intCond + ' form-control" type="text" campo="' + columnField + '"></div>';
                break;
            case "int":
                control = '<div class="form-group"><label>Valor</label><input class="input-val-' + intCond + ' form-control" type="number" campo="' + columnField + '"></div>';
                break;
            case "num":
                control = '<div class="form-group"><label>Valor</label><input class="input-val-' + intCond + ' form-control" type="number" campo="' + columnField + '"></div>';
                break;
            case "fec":
                control = '<div class="form-group"><label>Valor</label><input class="input-val-' + intCond + ' form-control" type="date" campo="' + columnField + '"></div>';
                break;
            case "bit":
                control = '<div class="form-group"><label>Valor</label><input class="input-val-' + intCond + ' form-control" type="checkbox" campo="' + columnField + '"></div>';
                break;
        }
    }
    else {
        listFiltrosDataTable = jQuery.grep(listFiltrosDataTable, function (value) {
            return value.intCondicion != intCond;
        });

        listFiltrosDataTable.splice(intCond, 0, {
            intCondicion: intCond,
            columnField: columnField,
            logicOperator: logicOpe,
            value: "",
            operator: operator
        });

        if (ejecutarFiltro && !tbl.page.info().serverSide) {
            columTbl.search("").draw();
        }
    }

    if (control != "") {
        columInputVal.empty().append(control).on("change", "input.input-val-" + intCond, function () {
            var logicOperator = $("." + nameOperator).val();
            var valor = $(this).val();
            var campo = $(this).attr("campo");
            var intCondicion = nameOperator.substring(nameOperator.indexOf("-") + 1);
            var operator = $("." + nameLogicOperator).val();
            //console.log("." + logicOperator);

            if (campo.substring(0, 3) === 'bit') {
                if ($(this).is(":checked")) {
                    valor = "true";
                } else {
                    valor = "false";
                }
            }

            listFiltrosDataTable = jQuery.grep(listFiltrosDataTable, function (value) {
                return value.intCondicion != intCondicion;
            });

            listFiltrosDataTable.splice(intCondicion, 0, {
                intCondicion: intCondicion,
                columnField: campo,
                logicOperator: logicOperator,
                value: valor,
                operator: operator
            });
            //console.log(listFiltrosDataTable);
            if (ejecutarFiltro && !tbl.page.info().serverSide) {
                columTbl.search(valor).draw();
            }
        });
    }
}


function llenarCondiciones() {

    $.each(listFiltrosDataTable, function (index, item) {
        
        //Se inicializa el número de la condición
        contRows = parseInt(item.intCondicion);

        //Se agrega una nueva condición al formulario
        agregarCondicion()

        //Se asigna un valor a los combos Campo y Operación
        var comboCampo = $("select.listColumns-" + item.intCondicion);
        var comboOperacion = $("select.listOperations-" + item.intCondicion);
        var comboOperator = $("select.listOperator-" + item.intCondicion);

        comboCampo.val(item.columnField);
        comboOperacion.val(item.logicOperator);
        comboOperator.val(item.operator);

        // Se la operación lógica es "Está Vacío" o "No está vacío" no se crea ningún input
        if (item.logicOperator != "IsEmpty" && item.logicOperator != "IsNotEmpty") {

            //Se crea el Input
            crearControl(comboCampo);

            //Se asigna un valor al Input
            var inputFilter = $("input.input-val-" + item.intCondicion);

            inputFilter.val(item.value);

            if (item.columnField.substring(0, 3) === 'bit') {
                if (item.value === "true") {
                    inputFilter.prop('checked', true);
                } else {
                    inputFilter.prop('checked', false);
                }
            }

        }
    });

}
