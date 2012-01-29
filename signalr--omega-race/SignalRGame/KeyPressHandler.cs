using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR;

namespace SignalRGame
{

    public class KeyPressHandler : PersistentConnection 
    {
        protected override System.Threading.Tasks.Task OnReceivedAsync(string clientId, string data)
        {
            var args = data.Split(':');
            bool keyDown = ( args[0] == "d");
            string shipName = args[1];
            int key = int.Parse(args[2]);

            var ship = Game.GetShipByName(shipName);
            switch (key)
            {
                case 39:
                    {
                        ship.MovingRight = keyDown;
                        break;
                    }
                case 37:
                    {
                        ship.MovingLeft = keyDown;
                        break;
                    }
                case 38:
                    {
                        ship.MovingUp = keyDown;
                        break;
                    }
                case 40:
                    {
                        ship.MovingDown = keyDown;
                        break;
                    }
                case 68:
                    {
                        ship.SpinningRight = keyDown;
                        break;
                    }
                case 65:
                    {
                        ship.SpinningLeft = keyDown;
                        break;
                    }
                case 83:
                    {
                        ship.FireMissile();
                        break;
                    }
            }

            return base.OnReceivedAsync(clientId, data);
        }
    }
}