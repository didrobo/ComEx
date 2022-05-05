using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ComEx.Hubs
{
    [HubName("Reportes")]
    /// <summary>
    /// Hub para notificacion de los reportes
    /// </summary>
    public class ReportHub : Hub
    {
        [HubMethodName("Mostrar")]
        public void Mostrar(string strMensaje)
        {
            Clients.All.Mostrar(strMensaje);
        }

        [HubMethodName("Ocultar")]
        public void Ocultar()
        {
            Clients.All.Ocultar();
        }

        [HubMethodName("OcultarConMensaje")]
        public void OcultarConMensaje(string strMensaje)
        {
            Clients.All.OcultarConMensaje(strMensaje);
        }
    }
}