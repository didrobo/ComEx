using Microsoft.Owin;
using Owin;

[assembly:OwinStartup(typeof(ComEx.App_Start.Startup))]
namespace ComEx.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {            
            app.MapSignalR();
        }
    }
}