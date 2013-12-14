using Microsoft.Owin;
using Owin;
using ServerNotify;

[assembly: OwinStartup(typeof(Startup))]

namespace ServerNotify
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}