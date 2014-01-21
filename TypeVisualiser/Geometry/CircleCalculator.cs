using System;
using System.Windows;

namespace TypeVisualiser.Geometry
{
    internal class CircleCalculator
    {
        public CircleCalculator(Point centre, double angle)
        {
            Centre = centre;
            Angle = angle;
        }

        public double Angle { get; private set; }
        public Point Centre { get; private set; }

        public Point CalculatePointOnCircle(double radius)
        {
            // (x?,y?)=(x + d * cos(angle), y + d * sin(angle))
            double x = Centre.X + radius*Math.Cos(Angle*Math.PI/180F);
            double y = Centre.Y + radius*Math.Sin(Angle*Math.PI/180F);
            return new Point(x, y);
        }
    }
}