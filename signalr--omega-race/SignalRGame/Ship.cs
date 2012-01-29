using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace SignalRGame
{
    public class Ship : GameElement
    {
        public string Name { get; set; }
        public double XSpeed { get; set; }
        public double YSpeed { get; set; }
        public double SpinSpeed { get; set; }
        public bool MovingLeft { get; set; }
        public bool MovingRight { get; set; }
        public bool MovingUp { get; set; }
        public bool MovingDown { get; set; }
        public bool SpinningLeft { get; set; }
        public bool SpinningRight { get; set; }

        public void Move()
        {
            if (MovingLeft)
                XSpeed--;

            if (MovingRight)
                XSpeed++;

            if (MovingUp)
                YSpeed--;

            if (MovingDown)
                YSpeed++;

            if (SpinningLeft)
                SpinSpeed--;

            if (SpinningRight)
                SpinSpeed++;

            ConstrainToTrack();

            X += (XSpeed*1.1);
            Y += (YSpeed*1.1);
            Angle += SpinSpeed;
        }

        public void Decelerate()
        {
            SpinSpeed *= .9;
            XSpeed *= .97;
            YSpeed *= .97;
        }

        public void FireMissile()
        {
            if (_missiles.Count <= 3)
            {
                var angle = (Angle + 90)*Math.PI/180;
                _missiles.Add(new Missile() {Angle = angle, X = this.X, Y = this.Y});
            }
        }

        public void MoveMissiles()
        {
            foreach (var missile in _missiles)
            {
                missile.X += 20*Math.Cos(missile.Angle);
                missile.Y += 20*Math.Sin(missile.Angle);
            }

            // clean any that have left the arena
            for (int i = _missiles.Count - 1; i >= 0; i--)
            {
                var missile = _missiles[i];
                if (Game.Arena.IsInCentralReservation(missile.X, missile.Y))
                {
                    _missiles.Remove(missile);
                }
                if (Game.Arena.IsOutOfArena(missile.X, missile.Y))
                {
                    _missiles.Remove(missile);
                }
            }
        }

        public List<Missile> HitTest(List<Enemy> enemies)
        {
            var hits = new List<Missile>();
            for (var missileIndex = _missiles.Count - 1; missileIndex >= 0; missileIndex--)
            {
                bool hit = false;
                var missile = _missiles[missileIndex];
                for (var enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
                {
                    var enemy = enemies[enemyIndex];
                    if (missile.X.IsBetween( enemy.X - 13, enemy.X + 13) && missile.Y.IsBetween(enemy.Y -13, enemy.Y + 13) )
                    {
                        hit = true;
                        hits.Add(new Missile() {Angle = 10, X = enemy.X, Y = enemy.Y});
                        enemies.Remove(enemy);
                    }
                }
                if (hit)
                    _missiles.Remove(missile);
            }
            return hits;
        }

        public Missile[] Missiles
        {
            get { return _missiles.ToArray(); }
        }

        private void ConstrainToTrack()
        {
            BounceOffCentralReservation();

            if (X <= Game.Arena.Boundary.Left)
            {
                X = Game.Arena.Boundary.Left + 1;
                XSpeed *= -1;
            }
            if (X >= Game.Arena.Boundary.Right)
            {
                X = Game.Arena.Boundary.Right - 1;
                XSpeed *= -1;
            }
            if (Y >= Game.Arena.Boundary.Bottom)
            {
                Y = Game.Arena.Boundary.Bottom - 1;
                YSpeed *= -1;
            }
            if (Y <= Game.Arena.Boundary.Top)
            {
                Y = Game.Arena.Boundary.Top + 1;
                YSpeed *= -1;
            }
        }

        private void BounceOffCentralReservation()
        {
            if (Game.Arena.CentralReservation.IsHittingLeftSide(X, Y) || Game.Arena.CentralReservation.IsHittingRightSide(X, Y))
                XSpeed *= -1;

            if (Game.Arena.CentralReservation.IsHittingBottomSide(X, Y) || Game.Arena.CentralReservation.IsHittingTopSide(X, Y))
                YSpeed *= -1;

        }

        private List<Missile> _missiles = new List<Missile>();
    }

    public static class Extensions
    {
        public static bool IsBetween(this double value, double begin, double end)
        {
            return (value >= begin && value <= end);
        }

        public static bool IsHittingLeftSide(this RectangleF area, double x, double y)
        {
            return (x.IsBetween(area.Left, area.Left + 10) && y.IsBetween(area.Top, area.Bottom));
        }

        public static bool IsHittingRightSide(this RectangleF area, double x, double y)
        {
            return (x.IsBetween(area.Right - 10, area.Right) && y.IsBetween(area.Top, area.Bottom));
        }

        public static bool IsHittingBottomSide(this RectangleF area, double x, double y)
        {
            return (x.IsBetween(area.Left, area.Right) && y.IsBetween(area.Bottom, area.Bottom + 10));
        }

        public static bool IsHittingTopSide(this RectangleF area, double x, double y)
        {
            return (x.IsBetween(area.Left, area.Right) && y.IsBetween(area.Top - 10, area.Top));
        }
    }
}