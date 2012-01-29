using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using SignalR.Routing;

namespace SignalRGame
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            BootstrapSignalR();
            Game.StartGame();
        }

        private static void BootstrapSignalR()
        {
            RouteTable.Routes.MapConnection<NewPlayerHandler>("playerconnection", "playerconnection/{*operation}");
            RouteTable.Routes.MapConnection<KeyPressHandler>("keypressconnection", "keypressconnection/{*operation}");
            RouteTable.Routes.MapConnection<ClientHandler>("gameconnection", "gameconnection/{*operation}");
        }
    }
}