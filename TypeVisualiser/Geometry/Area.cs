using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Geometry
{
    [DebuggerDisplay("Area ({TopLeft}) ({BottomRight})")]
    public class Area
    {
        public Area(FrameworkElement control)
        {
            if (control == null)
            {
                throw new ArgumentNullResourceException("control", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            TopLeft = new Point(Canvas.GetLeft(control), Canvas.GetTop(control));
            BottomRight = new Point(TopLeft.X + control.ActualWidth, TopLeft.Y + control.ActualHeight);
        }

        public Area(Point topLeft, double actualWidth, double actualHeight)
        {
            TopLeft = topLeft.Clone();
            BottomRight = new Point(TopLeft.X + actualWidth, TopLeft.Y + actualHeight);
        }

        public Area(Point topLeft, Point bottomRight)
        {
            TopLeft = topLeft.Clone();
            BottomRight = bottomRight.Clone();
        }

        public Point BottomRight { get; private set; }

        public Point Centre
        {
            get
            {
                Point c = TopLeft.Clone();
                c.Offset(Width / 2, Height / 2);
                return c;
            }
        }

        public double Height
        {
            get { return BottomRight.Y - TopLeft.Y; }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// Used for debugging and logging.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag { get; set; }

        public Point TopLeft { get; private set; }

        public double Width
        {
            get { return BottomRight.X - TopLeft.X; }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Area: {0} ({1:F2}, {2:F2}) ({3:F2}, {4:F2})", Tag, TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
        }

        public Point CalculateCircumferenceIntersectionPoint(double northOrientedAngle)
        {
            Point intersectionPoint = Centre;
            if (northOrientedAngle >= 360)
            {
                northOrientedAngle = northOrientedAngle % 360;
            }

            // Fringe cases
            if (Math.Abs(northOrientedAngle - 0) < 0.01)
            {
                intersectionPoint.Offset(0, -Height / 2);
                return intersectionPoint;
            }

            if (Math.Abs(northOrientedAngle - 180) < 0.01)
            {
                intersectionPoint.Offset(0, Height / 2);
                return intersectionPoint;
            }

            int quadrant;
            for (quadrant = 0; quadrant < 4; quadrant++)
            {
                if (northOrientedAngle < 90.0)
                {
                    break;
                }

                northOrientedAngle -= 90.0;
            }

            // Try assuming line exits on vertical side
            double adjacent, opporsite;
            double angleInRadians = TrigHelper.DegreesToRadians(northOrientedAngle);
            Func<double, double> calculateOpporsite = a => a * Math.Tan(angleInRadians);
            Func<double, double> calculateAdjacent = o => o / Math.Tan(angleInRadians);

            switch (quadrant)
            {
                case 0:
                    opporsite = Width / 2;
                    adjacent = calculateAdjacent(opporsite);
                    if (adjacent > Height / 2)
                    {
                        // havent found intersection yet
                        adjacent = Height / 2;
                        opporsite = calculateOpporsite(adjacent);
                        if (opporsite > Width / 2)
                        {
                            UnableToCalculateIntersectionPoint(northOrientedAngle, opporsite, adjacent, quadrant);
                        }
                    }

                    intersectionPoint.Offset(opporsite, -adjacent);
                    return intersectionPoint;

                case 1:
                    adjacent = Width / 2;
                    opporsite = calculateOpporsite(adjacent);
                    if (opporsite > Height / 2)
                    {
                        // havent found intersection yet
                        opporsite = Height / 2;
                        adjacent = calculateAdjacent(opporsite);
                        if (adjacent > Width / 2)
                        {
                            UnableToCalculateIntersectionPoint(northOrientedAngle, opporsite, adjacent, quadrant);
                        }
                    }
                    
                    intersectionPoint.Offset(adjacent, opporsite);
                    return intersectionPoint;

                case 2:
                    opporsite = Width / 2;
                    adjacent = calculateAdjacent(opporsite);
                    if (adjacent > Height / 2)
                    {
                        // havent found intersection yet
                        adjacent = Height / 2;
                        opporsite = calculateOpporsite(adjacent);
                        if (opporsite > Width / 2)
                        {
                            UnableToCalculateIntersectionPoint(northOrientedAngle, opporsite, adjacent, quadrant);
                        }
                    }

                    intersectionPoint.Offset(-opporsite, adjacent);
                    return intersectionPoint;

                case 3:
                    adjacent = Width / 2;
                    opporsite = calculateOpporsite(adjacent);
                    if (opporsite > Height / 2)
                    {
                        // havent found intersection yet
                        opporsite = Height / 2;
                        adjacent = calculateAdjacent(opporsite);
                        if (adjacent > Width / 2)
                        {
                            UnableToCalculateIntersectionPoint(northOrientedAngle, opporsite, adjacent, quadrant);
                        }
                    }

                    intersectionPoint.Offset(-adjacent, -opporsite);
                    return intersectionPoint;
            
                default:
                    throw new NotSupportedException("Invalid quadrant detected in Calculate Intersection point: " + quadrant);
            }
        }

        private static void UnableToCalculateIntersectionPoint(double northOrientedAngle, double opporsite, double adjacent, int quadrant)
        {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                                                              "Unable to calculate intersection point - bad data: angle={0:F2}, o={1:F2}, a={2:F2}, quadrant={3}",
                                                              northOrientedAngle,
                                                              opporsite,
                                                              adjacent,
                                                              quadrant));
        }

        public double DistanceToPoint(Point destination)
        {
            return destination.DistanceTo(TopLeft);
        }

        public Area Offset(double moveX, double moveY)
        {
            Point newTopLeft = TopLeft.Clone();
            newTopLeft.Offset(moveX, moveY);
            Point newBottom = BottomRight.Clone();
            newBottom.Offset(moveX, moveY);
            var newArea = new Area(newTopLeft, newBottom);
            return newArea;
        }

        public Area OffsetToMakeTopLeftCentre()
        {
            Point topLeft = TopLeft.Clone();
            topLeft.Offset(-(Width / 2), -(Height / 2));
            Point bottomRight = BottomRight.Clone();
            bottomRight.Offset(-(Width / 2), -(Height / 2));
            return new Area(topLeft, bottomRight);
        }

        /// <summary>
        /// Determines if the given proposed area overlaps with this instance.
        /// Will return <see cref="Proximity.VeryClose"/> if within the <see cref="LayoutConstants.MinimumDistanceBetweenObjects"/>.
        /// If it is very close, a direction will be given, otherwise <see cref="Direction.Unknown"/> will be returned.
        /// </summary>
        /// <param name="proposedArea">The proposed area.</param>
        /// <returns></returns>
        public ProximityTestResult OverlapsWith(Area proposedArea)
        {
            if (Double.IsNaN(TopLeft.X) || Double.IsNaN(TopLeft.Y) || Double.IsNaN(BottomRight.X) || Double.IsNaN(BottomRight.Y))
            {
                return new ProximityTestResult(Proximity.NotOverlapping);
            }

            if (proposedArea == null)
            {
                throw new ArgumentNullResourceException("proposedArea", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (Double.IsNaN(proposedArea.TopLeft.X) || Double.IsNaN(proposedArea.TopLeft.Y) || Double.IsNaN(proposedArea.BottomRight.X) || Double.IsNaN(proposedArea.BottomRight.Y))
            {
                throw new ArgumentException(Resources.Area_OverlapsWith_Error_in_usage_Attempt_to_find_overlapping_areas_where_the_proposed_area_is_NaN, "proposedArea");
            }

            // Cond1.  If A's left edge is to the right of the B's right edge, then A is Totally to right Of B
            ProximityTestResult overlapsWith;
            if (IsProposedAreaToTheRightOfThis(proposedArea, out overlapsWith))
            {
                return overlapsWith;
            }

            // Cond2.  If A's right edge is to the left of the B's left edge, then A is Totally to left Of B
            if (IsProposedAreaToTheLeftOfThis(proposedArea, out overlapsWith))
            {
                return overlapsWith;
            }

            // Cond3.  If A's top edge is below B's bottom  edge, then A is Totally below B
            if (IsProposedAreaBelowThis(proposedArea, out overlapsWith))
            {
                return overlapsWith;
            }

            // Cond4.  If A's bottom edge is above B's top edge, then A is Totally above B
            if (IsProposedAreaAboveThis(proposedArea, out overlapsWith))
            {
                return overlapsWith;
            }

            return new ProximityTestResult(Proximity.Overlapping);
        }

        public void Update(FrameworkElement control)
        {
            if (control == null)
            {
                throw new ArgumentNullResourceException("control", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            TopLeft = new Point(Canvas.GetLeft(control), Canvas.GetTop(control));
            if (control.ActualHeight < 1 || control.ActualWidth < 1)
            {
                Logger.Instance.WriteEntry("WARNING: Actual width and/or height is zero for association control.");
            }

            BottomRight = new Point(TopLeft.X + control.ActualWidth, TopLeft.Y + control.ActualHeight);
        }

        private static bool ItMightBeClose(double coordinate1, double coordinate2)
        {
            return (Math.Abs(coordinate1 - coordinate2) < LayoutConstants.MinimumDistanceBetweenObjects);
        }

        private bool IsProposedAreaAboveThis(Area proposedArea, out ProximityTestResult comparisonResult)
        {
            if (proposedArea.BottomRight.Y <= TopLeft.Y)
            {
                // porposed area is completely above this area
                if (ItMightBeClose(proposedArea.BottomRight.Y, TopLeft.Y))
                {
                    if (proposedArea.TopLeft.X.IsBetween(TopLeft.X, BottomRight.X) || proposedArea.BottomRight.X.IsBetween(TopLeft.X, BottomRight.X))
                    {
                        {
                            comparisonResult = new ProximityTestResult(Proximity.VeryClose) { DirectionToOtherObject = Direction.Up, DistanceToClosestObject = TopLeft.Y - proposedArea.BottomRight.Y, };
                            return true;
                        }
                    }
                }

                {
                    comparisonResult = new ProximityTestResult(Proximity.NotOverlapping);
                    return true;
                }
            }

            comparisonResult = new ProximityTestResult(Proximity.Unknown);
            return false;
        }

        private bool IsProposedAreaBelowThis(Area proposedArea, out ProximityTestResult comparisonResult)
        {
            if (proposedArea.TopLeft.Y >= BottomRight.Y)
            {
                // proposed area is completely below this area
                if (ItMightBeClose(proposedArea.TopLeft.Y, BottomRight.Y))
                {
                    if (proposedArea.TopLeft.X.IsBetween(TopLeft.X, BottomRight.X) || proposedArea.BottomRight.X.IsBetween(TopLeft.X, BottomRight.X))
                    {
                        {
                            comparisonResult = new ProximityTestResult(Proximity.VeryClose) { DirectionToOtherObject = Direction.Down, DistanceToClosestObject = proposedArea.TopLeft.Y - BottomRight.Y, };
                            return true;
                        }
                    }
                }

                {
                    comparisonResult = new ProximityTestResult(Proximity.NotOverlapping);
                    return true;
                }
            }

            comparisonResult = new ProximityTestResult(Proximity.Unknown);
            return false;
        }

        private bool IsProposedAreaToTheLeftOfThis(Area proposedArea, out ProximityTestResult comparisonResult)
        {
            if (proposedArea.BottomRight.X <= TopLeft.X)
            {
                // proposed area is completely to the left of this area.
                if (ItMightBeClose(proposedArea.BottomRight.X, TopLeft.X))
                {
                    if (proposedArea.TopLeft.Y.IsBetween(TopLeft.Y, BottomRight.Y) || proposedArea.BottomRight.Y.IsBetween(TopLeft.Y, BottomRight.Y))
                    {
                        {
                            comparisonResult = new ProximityTestResult(Proximity.VeryClose) { DirectionToOtherObject = Direction.Left, DistanceToClosestObject = TopLeft.X - proposedArea.BottomRight.X, };
                            return true;
                        }
                    }
                }

                {
                    comparisonResult = new ProximityTestResult(Proximity.NotOverlapping);
                    return true;
                }
            }

            comparisonResult = new ProximityTestResult(Proximity.Unknown);
            return false;
        }

        private bool IsProposedAreaToTheRightOfThis(Area proposedArea, out ProximityTestResult comparisonResult)
        {
            if (proposedArea.TopLeft.X >= BottomRight.X)
            {
                // proposed area is completely to the right of this area.
                if (ItMightBeClose(proposedArea.TopLeft.X, BottomRight.X))
                {
                    if (proposedArea.TopLeft.Y.IsBetween(TopLeft.Y, BottomRight.Y) || proposedArea.BottomRight.Y.IsBetween(TopLeft.Y, BottomRight.Y))
                    {
                        {
                            comparisonResult = new ProximityTestResult(Proximity.VeryClose) { DirectionToOtherObject = Direction.Right, DistanceToClosestObject = proposedArea.TopLeft.X - BottomRight.X, };
                            return true;
                        }
                    }
                }

                {
                    comparisonResult = new ProximityTestResult(Proximity.NotOverlapping);
                    return true;
                }
            }

            comparisonResult = new ProximityTestResult(Proximity.Unknown);
            return false;
        }
    }
}