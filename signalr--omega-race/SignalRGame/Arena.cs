using System.Drawing;

namespace SignalRGame
{
    public class Arena
    {        
        public Arena()
        {
            Height = 500;
            Width = 1000;
            Margin = 10;
            Boundary = new RectangleF(5, 5, Width - Margin, Height - Margin);
            CentralReservation = new RectangleF(200,200,600,100);
        }

        public float Height { get; set; }
        public float Width { get; set; }
        public float Margin { get; set; }
        public RectangleF Boundary  { get; set; }
        public RectangleF CentralReservation { get; set; }

        public bool IsInCentralReservation(double x, double y)
        {
            return CentralReservation.Contains((float)x, (float)y);
        }

        public bool IsOutOfArena(double x, double y)
        {
            return !Boundary.Contains((float)x, (float)y);
        }

    }
}