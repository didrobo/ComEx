var localCache = {
    /**
     * timeout for cache in millis
     * @type {number}
     */
    timeout: 900000,
    /** 
     * @type {{_: number, data: {}}}
     **/
    data: {},
    remove: function (url) {
        delete localCache.data[url];
    },
    exist: function (url) {
        return !!localCache.data[url] && ((new Date().getTime() - localCache.data[url]._) < localCache.timeout);
    },
    get: function (url) {
        console.log('Getting in cache for url' + url);
        return localCache.data[url].data;
    },
    set: function (url, cachedData, callback) {
        localCache.remove(url);
        localCache.data[url] = {
            _: new Date().getTime(),
            data: cachedData
        };
        if ($.isFunction(callback)) callback(cachedData);
    }
};

$(document).on('ready', function () {
    cargarAlertas();

    if ($('#ChkFiltroPorAno').is(':checked')) {
        $("#AppAnoFiltro").removeAttr('disabled');
    } else {
        $('#AppAnoFiltro').attr('disabled', 'disabled');
    }
});

//Función que llena el DropDown de alertas de la barra superior de la aplicación
function cargarAlertas() {
    var url = ResolveUrl('~/tmNotificaciones/ConsultarAlertas');
    $.ajax({
        url: url,
        data: '{}',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: true,
        beforeSend: function () {
            if (localCache.exist(url)) {
                procesarNotificaciones(localCache.get(url));
                return false;
            }
            return true;
        },
        complete: function (resultado) {
            if (resultado.responseJSON.success) {
                localCache.set(url, resultado.responseJSON, procesarNotificaciones);
            }
        }
    });
}

//Procesa las notificaciones recibidas
function procesarNotificaciones(resultado) {

    if (resultado.cantAlertas > 0) {
        $("#intCntAlertas").empty().append(resultado.cantAlertas);
        $(".dropdown-alerts").empty();

        resultado.alertas.forEach(function (alerta) {
            addAlerta(alerta.varUrl, alerta.varIcno, alerta.varNmbre, alerta.intCntdad, alerta.varDscrpcion);
        });
    }
    else {
        $("#intCntAlertas").empty();
        $(".dropdown-alerts").empty();
    }

}

//Agrega una alerta al DropDown
function addAlerta(url, icono, nombre, cant, descripcion) {
    
    var item = '<li>' +
                    '<a href="' + url + '">' +
                        '<div>' +
                            '<i class="' + icono + '"></i> ' + nombre +
                            ' <span class="badge">' + cant + '</span>' +
                            '<span class="pull-right text-muted small">' + descripcion + '</span>' +
                        '</div>' +
                    '</a>' +
                '</li>' +
                '<li class="divider"></li>';

    $(".dropdown-alerts").append(item);
}

$('#AppAnoFiltro').on('input', function (e) {

    var ano = $('#AppAnoFiltro').val();

    if (ano.length == 4) {

        var url = ResolveUrl('~/Home/SetAppAnoFiltro');
        $.ajax({
            url: url,
            type: "POST",
            data: { intAno: ano },
            async: true
        });

    }
});

$('#ChkFiltroPorAno').change(function () {

    if (this.checked) {
        $("#AppAnoFiltro").removeAttr('disabled');        
    } else {
        $('#AppAnoFiltro').attr('disabled', 'disabled');
    }

    var url = ResolveUrl('~/Home/SetChkFiltroPorAno');
    $.ajax({
        url: url,
        type: "POST",
        data: { filtrarAno: this.checked },
        async: true
    });

});

