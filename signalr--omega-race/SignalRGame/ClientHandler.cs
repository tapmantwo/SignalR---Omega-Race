using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR;

namespace SignalRGame
{
    public class ClientHandler : PersistentConnection
    {
        protected override System.Threading.Tasks.Task OnConnectedAsync(HttpContextBase context, string clientId)
        {
            Game.AddGameHandler(this);
            return base.OnConnectedAsync(context, clientId);
        }

        internal void Draw(List<Ship> ships, List<Enemy> enemies, List<Missile> blasts, Arena arena)
        {
            Connection.Broadcast(new DrawInfo
                                     {
                                         Ships = ships.ToArray(),
                                         Enemies = enemies.ToArray(),
                                         Blasts = blasts.ToArray(),
                                         Arena = arena
                                     });
        }
    }

    public class DrawInfo
    {
        public Ship[] Ships;
        public Enemy[] Enemies;
        public Missile[] Blasts;
        public Arena Arena;
    }
}