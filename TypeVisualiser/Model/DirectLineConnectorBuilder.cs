namespace TypeVisualiser.Model
{
    using System;
    using System.Windows;

    using TypeVisualiser.Geometry;

    /// <summary>
    /// A connector builder implementation that creates connections in the most direct and shortest possible route.
    /// </summary>
    internal class DirectLineConnectorBuilder : IConnectorBuilder
    {
        /// <summary>
        /// Calculate the best line connecting <see cref="fromArea"/> to <see cref="destinationArea"/>
        /// </summary>
        /// <param name="fromArea">
        /// The from Area.
        /// </param>
        /// <param name="destinationArea">
        /// The destination Area.
        /// </param>
        /// <param name="isOverlappingWithOtherControls">
        /// The is Overlapping With Other Controls.
        /// </param>
        /// <returns>
        /// The <see cref="ConnectionLine"/>.
        /// </returns>
        public ConnectionLine CalculateBestConnection(Area fromArea, Area destinationArea, Func<Area, ProximityTestResult> isOverlappingWithOtherControls)
        {
            double fromAngle = fromArea.Centre.AngleToPointInDegrees(destinationArea.Centre);
            Point fromIdealPoint = fromArea.CalculateCircumferenceIntersectionPoint(fromAngle);
            double toAngle = TrigHelper.InverseAngle(fromAngle);
            Point toIdealPoint = destinationArea.CalculateCircumferenceIntersectionPoint(toAngle);
            Point toLineEnd = toIdealPoint;

            // Leave room for arrow head
            double offset1 = (ArrowHead.ArrowWidth - 2) * Math.Sin(CalculateLineAngleToPreviousAxis(toAngle));
            double offset2 = (ArrowHead.ArrowWidth - 2) * Math.Cos(CalculateLineAngleToPreviousAxis(toAngle));
            if (toAngle >= 0 && toAngle < 90)
            {
                toLineEnd.Offset(offset1, -offset2);
            }
            else if (toAngle >= 90 && toAngle <= 180)
            {
                toLineEnd.Offset(offset2, offset1);
            }
            else if (toAngle > 180 && toAngle < 270)
            {
                toLineEnd.Offset(-offset1, offset2);
            }
            else
            {
                toLineEnd.Offset(-offset2, -offset1);
            }

            return new ConnectionLine
                {
                    Distance = fromIdealPoint.DistanceTo(toIdealPoint), 
                    ExitAngle = CalculateExitAngle(fromAngle), 
                    From = fromIdealPoint, 
                    To = toIdealPoint, 
                    ToLineEnd = toLineEnd, 
                    ToAngle = toAngle, 
                    FromAngle = fromAngle, 
                };
        }

        private static double CalculateExitAngle(double angleOfLine)
        {
            double exitAngle = angleOfLine + 270;
            if (exitAngle > 360)
            {
                exitAngle = exitAngle % 360;
            }

            return exitAngle;
        }

        private static double CalculateLineAngleToPreviousAxis(double northOrientedAngle)
        {
            while (northOrientedAngle > 90)
            {
                northOrientedAngle -= 90;
            }

            return TrigHelper.DegreesToRadians(northOrientedAngle);
        }
    }
}