using Microsoft.AspNet.SignalR.Client;
using System;
using System.Linq;

namespace ComEx.Hubs
{
    public class PeticionSignalR
    {
        public static void EnviarServidor(string strUrl, string strHubName, string strMetodo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strUrl))
                {
                    throw new ArgumentNullException("strUrl");
                }

                if (string.IsNullOrWhiteSpace(strHubName))
                {
                    throw new ArgumentNullException("strHubName");
                }

                if (string.IsNullOrWhiteSpace(strMetodo))
                {
                    throw new ArgumentNullException("strMetodo");
                }

                HubConnection objConexionHub = new HubConnection(strUrl);
                IHubProxy proxy = objConexionHub.CreateHubProxy(strHubName);
                objConexionHub.Start().Wait();
                proxy.Invoke(strMetodo).Wait();
                objConexionHub.Stop();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void EnviarServidor(string strUrl, string strHubName, string strMetodo, params object[] objParametros)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strUrl))
                {
                    throw new ArgumentNullException("strUrl");
                }

                if (string.IsNullOrWhiteSpace(strHubName))
                {
                    throw new ArgumentNullException("strHubName");
                }

                if (string.IsNullOrWhiteSpace(strMetodo))
                {
                    throw new ArgumentNullException("strMetodo");
                }

                if (objParametros == null || objParametros.Count() == 0)
                {
                    throw new ArgumentNullException("objParametros");
                }

                HubConnection objConexionHub = new HubConnection(strUrl);
                IHubProxy proxy = objConexionHub.CreateHubProxy(strHubName);
                objConexionHub.Start().Wait();
                proxy.Invoke(strMetodo, objParametros).Wait();
                objConexionHub.Stop();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}