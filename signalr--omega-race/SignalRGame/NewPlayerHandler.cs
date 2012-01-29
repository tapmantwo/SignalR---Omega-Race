using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR;

namespace SignalRGame
{
    public class NewPlayerHandler : PersistentConnection
    {
        protected override System.Threading.Tasks.Task OnReceivedAsync(string clientId, string data)
        {
            int colourIndex = Game.NumberOfShips;
            if ( Game.NumberOfShips > _colours.Length - 1 )
                colourIndex = Game.NumberOfShips % _colours.Length;

            var colour = _colours[colourIndex];
            var ship = new Ship() {Colour = colour, Name = data, X = 50, Y = 50};
            Game.AddGameShip(ship);
            return Connection.Broadcast(ship);
        }


        private readonly string[] _colours = new string[]
                                            {
                                                "red",
                                                "white",
                                                "blue",
                                                "yellow"
                                            };
    }
}