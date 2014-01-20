using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TypeVisualiser.Geometry;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Model
{
    /// <summary>
    /// This class models the arrow head on the diagram.  It is designed to be wrapped into a <see cref="DiagramElement"/>.  The arrow head visual data binds to this.
    /// </summary>
    internal abstract class ArrowHead : IDiagramContent, INotifyPropertyChanged
    {
        public const int ArrowHeight = 25;
        public const int ArrowWidth = 41;
        private double headRotation;
        private double headTranslateX;
        private double headTranslateY;

        public enum ArrowDirection
        {
            Left = 180,
            Up = 270,
            Right = 0,
            Down = 90,
        }

        internal enum LineEnd
        {
            ArrowHead,
            Tail,
        }

        protected ArrowHead()
        {
            CalculateArrowHeadRotationAndTranslation();
            Id = Guid.NewGuid().ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the head rotation.
        /// 0 = Pointing right (used to point at the left side of a rectangle)
        /// 90 = Pointing down (used to point at the top side of a rectangle)
        /// 180 = Pointing left (used to point at the right side of a rectangle)
        /// 270 = Pointing up (used to point at the bottom side of a rectangle)
        /// </summary>
        /// <value>
        /// The head rotation.
        /// </value>
        public double HeadRotation
        {
            get { return this.headRotation;
            }
            set
            {
                if (Math.Abs(value - this.headRotation) > 0.01)
                {
                    this.headRotation = value;
                    RaisePropertyChanged("HeadRotation");
                }
            }
        }

        public double HeadTranslateX
        {
            get { return this.headTranslateX; }
            set
            {
                if (Math.Abs(value - this.headTranslateX) > 0.01)
                {
                    this.headTranslateX = value;
                    RaisePropertyChanged("HeadTranslateX");
                }
            }
        }

        public double HeadTranslateY
        {
            get { return this.headTranslateY; }
            set
            {
                if (Math.Abs(value - this.headTranslateY) > 0.01)
                {
                    this.headTranslateY = value;
                    RaisePropertyChanged("HeadTranslateY");
                }
            }
        }

        public string ToolTip { get; private set; }

        public static Area GetArea(int rotation, Point lineEnd)
        {
            Point topLeft = lineEnd.Clone();
            Point bottomRight = lineEnd.Clone();
            if (rotation == (int) ArrowDirection.Up)
            {
                topLeft.Offset(-(ArrowHeight / 2), -ArrowWidth);
                bottomRight.Offset(ArrowHeight / 2, 0);
            } else if (rotation == (int) ArrowDirection.Down)
            {
                topLeft.Offset(-(ArrowHeight / 2), 0);
                bottomRight.Offset(ArrowHeight / 2, ArrowWidth);
            } else if (rotation == (int) ArrowDirection.Right)
            {
                topLeft.Offset(0, -(ArrowHeight / 2));
                bottomRight.Offset(ArrowWidth, ArrowHeight / 2);
            } else if (rotation == (int) ArrowDirection.Left)
            {
                topLeft.Offset(-(ArrowWidth), -(ArrowHeight / 2));
                bottomRight.Offset(0, ArrowHeight / 2);
            }

            return new Area(topLeft, bottomRight);
        }

        /// <summary>
        /// Adjusts the coordinates after canvas expansion.
        /// If the diagram content needs to adjust itself after being repositioned when the canvas is expanded.
        /// </summary>
        /// <param name="horizontalExpandedBy">The horizontalExpandedBy adjustment.</param>
        /// <param name="verticalExpandedBy">The verticalExpandedBy adjustment.</param>
        public void AdjustCoordinatesAfterCanvasExpansion(double horizontalExpandedBy, double verticalExpandedBy)
        {
            // Nothing required here.
        }

        /// <summary>
        /// Notifies the data context (represented by this Diagram Content) that a previously registered position-dependent diagram element has moved.
        /// </summary>
        /// <param name="dependentElement">The dependent element.</param>
        /// <returns>
        /// A result containing information if layout changes are required with new suggested values.
        /// </returns>
        public ParentHasMovedNotificationResult NotifyDiagramContentParentHasMoved(DiagramElement dependentElement)
        {
            if (dependentElement == null)
            {
                throw new ArgumentNullResourceException("dependentElement", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            var route = dependentElement.DiagramContent as ConnectionLine;
            if (route == null)
            {
                return new ParentHasMovedNotificationResult(); 
            }

            HeadRotation = route.ExitAngle;
            CalculateArrowHeadRotationAndTranslation();

            return new ParentHasMovedNotificationResult(route.To);
        }

        /// <summary>
        /// Gives a data context for a diagram element the opportunity to listen to parent events when the <see cref="DiagramElement"/> is created.
        /// </summary>
        /// <param name="dependentElements">This object is dependent on the position of these elements</param>
        /// <param name="isOverlappingFunction">The delegate function to determine if a proposed position overlaps with any existing elements.</param>
        public void RegisterPositionDependency(IEnumerable<DiagramElement> dependentElements, Func<Area, ProximityTestResult> isOverlappingFunction)
        {
            List<DiagramElement> elementList = dependentElements.ToList();
            DiagramElement route = elementList.FirstOrDefault(x => x.DiagramContent is ConnectionLine);
            if (route == null)
            {
                return;
            }

            HeadRotation = ((ConnectionLine) route.DiagramContent).ExitAngle;
            CalculateArrowHeadRotationAndTranslation();

            foreach (DiagramElement element in elementList)
            {
                if (SetToolTipText(element))
                {
                    break;
                }
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void CalculateArrowHeadRotationAndTranslation()
        {
            SetHeadTranslate(); // actualWidth, actualHeight, (int) HeadRotation // Head rotation already set by FindBestConnectionRoute.
        }

        private void SetHeadTranslate()
        {
            //double actualWidth, double actualHeight, int rotation - old parameters
            HeadTranslateX = -ArrowWidth;
            HeadTranslateY = -ArrowHeight / 2;
            //return;
            
            // Arrow heads are positioned by the given ConnectionLine point. ConnectionRoutes are determined by calling
            // FindBestConnectionRoute. The point given will be the end of the line which will not touch the intended
            // target.  A gap is left for the arrow head.  The head must be positioned into this gap, so its tail
            // touches the line, and its head touches the target.
            //if (rotation == (int) ArrowDirection.Right)
            //{
            //    // Pointing right
            //    HeadTranslateY = (-(actualHeight / 2));
            //    HeadTranslateX = 0;
            //} else if (rotation == (int) ArrowDirection.Down)
            //{
            //    // Pointing down
            //    HeadTranslateX = (-(actualWidth / 2));
            //    HeadTranslateY = ((actualWidth - actualHeight) / 2);
            //} else if (rotation == (int) ArrowDirection.Left)
            //{
            //    // pointing left
            //    HeadTranslateX = (-actualWidth);
            //    HeadTranslateY = (-(actualHeight / 2));
            //} else if (rotation == (int) ArrowDirection.Up)
            //{
            //    // pointing up
            //    HeadTranslateX = (-(actualWidth / 2));
            //    HeadTranslateY = (-(ArrowWidth * 0.8));
            //    // HACK: something wrong with bottom right point its not on the corner of the content presenter. Hacked to fix for now.
            //} else
            //{
            //    throw new ArgumentException(Resources.ConnectionRoute_SetHeadTranslate_Error_in_usage_angles_other_than_those_divisible_by_90_are_not_supported, "rotation");
            //}
        }

        private bool SetToolTipText(DiagramElement parent)
        {
            if (parent != null)
            {
                var route = parent.DiagramContent as ConnectionLine;
                if (route != null)
                {
                    ToolTip = route.ToolTip;
                    return true;
                }
            }

            return false;
        }
    }
}