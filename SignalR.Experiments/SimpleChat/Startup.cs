﻿using Microsoft.Owin;
using Owin;
using SimpleChat;

[assembly: OwinStartup(typeof(Startup))]

namespace SimpleChat
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