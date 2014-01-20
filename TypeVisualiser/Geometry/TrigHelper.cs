using System;

namespace TypeVisualiser.Geometry
{
    internal static class TrigHelper
    {
        public static double RadianToDegrees(double angleInRadians)
        {
            return angleInRadians * (180.0 / Math.PI);
        }

        public static double DegreesToRadians(double angleInDegress)
        {
            return angleInDegress / (180.0 / Math.PI);
        }

        public static double InverseAngle(double angleInDegrees)
        {
            var result = angleInDegrees + 180;
            if (result > 360)
            {
                result = result % 360;
            }

            return result;
        }
    }
}