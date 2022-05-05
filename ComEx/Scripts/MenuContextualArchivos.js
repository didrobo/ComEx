//Cuando se cargue el documento ejecutar la función Inicio
$(document).on("ready", Inicio);

// ##############################################################
// ##############################################################

//Variables globales
var NombreMenuContextual = "MenuContextual";
var IdMenuContextual = $("#" + NombreMenuContextual);
var NombreAlertar = "Alerta";
var IdAlerta = $("#" + NombreAlertar);

// ##############################################################
// ##############################################################

//Funcion inicial que se ejecuta cuando se carga el documento
function Inicio() {
    // ##############################################################
    // ###### Declarar Evento (Funciones) que se van a ejecutar #####
    // ##############################################################
    //Explicar
    $("#page-wrapper").bind("contextmenu", OcultarMenuContextualPorDefecto);
  
    //Evento que se ejecuta cuando le damos click en el documento
    $(document).on("click", Document_Click);

    //Evento que se ejecuta cuando se presiona una tecla en el documento
    $(document).on("keydown", Document_KeyDown);

    $("#DropZone").on("dragover", SeleccionarArchvios_Dragover);
    $("#DropZone").on("drop", SeleccionarArchvios_Drop);

    //Validar que el navegador soporte el Api de los archivos
    if (!window.File && !window.FileReader && !window.FileList) {
        MostrarAlerta("Las API de archivos no son totalmente compatibles con este navegador.", "alert alert-warning");
    }
}

//Evento para ocultar el menú contextual por defecto del navegador y mostrar el personalizado
function OcultarMenuContextualPorDefecto(e) {
    e.preventDefault();
    IdMenuContextual.css({ 'display': 'block', 'left': e.pageX, 'top': e.pageY });
}

//Evento que se ejecuta cuando le damos click en el documento
function Document_Click(e) {
    if (e.button == 0 && e.target.parentNode.parentNode.id != NombreMenuContextual) {
        CerrarMenuContextual();
    }
}

//Evento que se ejecuta cuando se presiona una tecla en el documento
function Document_KeyDown(e) {
    //Cerrar el menú contextual cuando presione la tecla escape
    if (e.keyCode == 27) {
        CerrarMenuContextual();
    }
}

function SeleccionarArchvios_Dragover(evt) {
    evt.preventDefault();
    evt.stopPropagation();
    evt.originalEvent.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
}

function SeleccionarArchvios_Drop(evt) {
    evt.preventDefault();
    evt.stopPropagation();

    var files = evt.originalEvent.dataTransfer.files; // FileList object.

    // files is a FileList of File objects. List some properties.
    var output = [];
    for (var i = 0, f; f = files[i]; i++) {
        output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
                    f.size, ' bytes, last modified: ',
                    f.lastModifiedDate.toLocaleDateString(), '</li>');
    }
    $("#lista").html('<ul>' + output.join('') + '</ul>');
}

// ##############################################################
// ##############################################################

//Fucnión para cerrar el menú contextual
function CerrarMenuContextual() {
    IdMenuContextual.css("display", "none");
}

//Función para Mostrar el div de Alerta
function MostrarAlerta(Mensaje, Clases) {

    //Limpiar div de alerta
    IdAlerta.empty();

    //Limpiar clases
    IdAlerta.attr("class", "");

    if (Mensaje === undefined || Mensaje === "" || Mensaje === " " || Mensaje === null) {
        return;
    }

    if (Clases === undefined || Clases === "" || Clases === " " || Clases === null) {
        return;
    }

    //Agregar mensaje
    IdAlerta.html(Mensaje);
    //Agregar clases
    IdAlerta.addClass(Clases);

    //Mostrar div
    IdAlerta.css({ "display": "block" });
}