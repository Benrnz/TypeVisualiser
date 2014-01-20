namespace TypeVisualiser.Geometry
{
    using System;
    using System.Windows;

    internal class CircleCalculator
    {
        public CircleCalculator(Point centre, double angle)
        {
            this.Centre = centre;
            this.Angle = angle;
        }

        public double Angle { get; private set; }

        public Point Centre { get; private set; }

        public Point CalculatePointOnCircle(double radius)
        {
            // (x?,y?)=(x + d * cos(angle), y + d * sin(angle))
            double x = this.Centre.X + (radius * Math.Cos(this.Angle * (Math.PI / 180F)));
            double y = this.Centre.Y + (radius * Math.Sin(this.Angle * (Math.PI / 180F)));
            return new Point(x, y);
        }
    }
}