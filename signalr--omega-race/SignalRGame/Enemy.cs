using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace SignalRGame
{
    public class Enemy : GameElement
    {
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public double Margin { get; set; }

        public Enemy()
        {
            Colour = "green";
        }

        public void Move()
        {
            if (X <= Game.Arena.CentralReservation.Left && Y >= Game.Arena.CentralReservation.Bottom + 20 && X <= Margin)
            {
                SpeedX = 0;
                SpeedY = -1;
            }

            if (Y <= Game.Arena.CentralReservation.Top && X <= Game.Arena.CentralReservation.Left && Y <= Margin)
            {
                SpeedX = 1;
                SpeedY = 0;
            }

            if (Y <= Game.Arena.CentralReservation.Top && X >= Game.Arena.Boundary.Right - Margin)
            {
                SpeedX = 0;
                SpeedY = 1;
            }

            if (Y >= Game.Arena.CentralReservation.Bottom + 20 + ( Margin * .8) && X >= Game.Arena.CentralReservation.Right )
            {
                SpeedX = -1;
                SpeedY = 0;
            }

            X += (SpeedX * EnemySpeedFactor);
            Y += (SpeedY * EnemySpeedFactor);
        }

        private const double EnemySpeedFactor = 2;
    }
}