namespace TypeVisualiser.Geometry
{
    using System;
    using System.Windows;

    internal static class PointExtension
    {
        public static Point Clone(this Point instance)
        {
            return new Point(instance.X, instance.Y);
        }

        /// <summary>
        /// Measures the Distances to the given point from this instance. 
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// A double in UI units.
        /// </returns>
        public static double DistanceTo(this Point instance, Point destination)
        {
            double vertical = destination.Y > instance.Y ? destination.Y - instance.Y : instance.Y - destination.Y;
            double horizontal = destination.X > instance.X ? destination.X - instance.X : instance.X - destination.X;

            vertical = Math.Pow(vertical, 2);
            horizontal = Math.Pow(horizontal, 2);
            return Math.Sqrt(vertical + horizontal);
        }

        /// <summary>
        /// Angles to point in degrees.
        /// Measures the angle from the source (instance) point to the destination point.
        /// Zero degrees is aligned to point directly up (north), right is 90 degrees. 
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// A double in the Range 0 to 359.999999.
        /// </returns>
        public static double AngleToPointInDegrees(this Point instance, Point destination)
        {
            var horizontal = Math.Abs(destination.X - instance.X);
            var vertical = Math.Abs(destination.Y - instance.Y);
            double angle;
            if (destination.X >= instance.X && destination.Y <= instance.Y)
            {
                // quadrant 0
                if (Math.Abs(horizontal - 0) < 0.001)
                {
                    return 0; // if horizontal is zero, then angle is north, ie 0ddegrees. (ATan(Infinity) == 90 so must be changed to 0).
                }

                angle = TrigHelper.RadianToDegrees(Math.Atan(horizontal / vertical));
            }
            else if (destination.X >= instance.X && destination.Y >= instance.Y)
            {
                // quadrant 1
                angle = 90 + TrigHelper.RadianToDegrees(Math.Atan(vertical / horizontal));
            }
            else if (destination.X <= instance.X && destination.Y >= instance.Y)
            {
                // quadrant 2
                angle = 180 + TrigHelper.RadianToDegrees(Math.Atan(horizontal / vertical));
            }
            else
            {
                // quadrant 3
                angle = 270 + TrigHelper.RadianToDegrees(Math.Atan(vertical / horizontal));
            }

            return angle;
        }
    }
}