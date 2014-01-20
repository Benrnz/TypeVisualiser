using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using TypeVisualiser.Geometry;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Model
{
    /// <summary>
    /// A model class that describes the line between the subject and an association
    /// </summary>
    internal class ConnectionLine : IComparable, IComparable<ConnectionLine>, IDiagramContent, INotifyPropertyChanged
    {
        private Point from;
        private Func<Area, ProximityTestResult> isOverlapping;
        private DiagramElement parent1;
        private DiagramElement parent2;
        private DiagramElement primaryParent; // This is either parent1 or parent2
        private Point to;
        private Point toLineEnd;

        internal ConnectionLine()
        {
            Thickness = 2;
            Id = Guid.NewGuid().ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static IConnectorBuilder ConnectorBuilder { get; set; }

        public double Distance { get; set; }

        /// <summary>
        /// Gets the exit angle.
        /// 0 = Pointing right (used to point at the left side of a rectangle)
        /// 90 = Pointing down (used to point at the top side of a rectangle)
        /// 180 = Pointing left (used to point at the right side of a rectangle)
        /// 270 = Pointing up (used to point at the bottom side of a rectangle)
        /// </summary>
        public double ExitAngle { get; set; }

        /// <summary>
        /// Gets or sets the From point. This is the beginning of the line. The From end is aligned to the diagram subject usually.
        /// </summary>
        public Point From
        {
            get { return this.from; }
            set
            {
                this.from = value;
                RaisePropertyChanged("From");
            }
        }

        /// <summary>
        /// Used only for debug purposes
        /// The north oriented angle of the line from the pov of the 'to' end.
        /// </summary>
        public double FromAngle { get; set; }

        public string Id { get; private set; }

        public IDiagramContent PointingAt
        {
            get { return this.primaryParent.DiagramContent; }
        }

        public string Style { get; set; }

        public double Thickness { get; set; }

        /// <summary>
        /// Gets or sets the To position of the connection. 
        /// This is the ultimate end of the connection line including the arrow head. The arrow head is aligned to this point, where the line end is 
        /// aligned to the <see cref="ToLineEnd"/>.
        /// </summary>
        public Point To
        {
            get { return this.to; }
            set
            {
                this.to = value;
                RaisePropertyChanged("To");
            }
        }

        /// <summary>
        /// Used only for debug purposes
        /// The north oriented angle of the line from the pov of the 'from' end.
        /// </summary>
        public double ToAngle { get; set; }

        /// <summary>
        /// Gets or sets the To Line End position.  
        /// This is the end of the straight line portion of the connection and the begining of the arrow head. The arrow head tip is aligned to the
        /// <see cref="To"/> point not this point.
        /// </summary>
        public Point ToLineEnd
        {
            get { return this.toLineEnd; }
            set
            {
                this.toLineEnd = value;
                RaisePropertyChanged("ToLineEnd");
            }
        }

        public string ToolTip { get; private set; }

        /// <summary>
        /// Finds the best connection location.
        /// </summary>
        /// <param name="fromArea">From visual.</param>
        /// <param name="destinationArea">To visual.</param>
        /// <param name="isOverlappingWithOtherControls">A function delegate to find out if a proposed area is overlapping with other controls.</param>
        /// <returns>A pair of points, from and to</returns>
        public static ConnectionLine FindBestConnectionRoute(
            Area fromArea, 
            Area destinationArea, 
            Func<Area, ProximityTestResult> isOverlappingWithOtherControls)
        {
            ConnectionLine result = ConnectorBuilder.CalculateBestConnection(fromArea, destinationArea, isOverlappingWithOtherControls);
            return result;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "ConnectionRoute ({0:F1},{1:F1}) {4:F1}deg to ({2:F1},{3:F1}) {5:F1}deg", From.X, From.Y, To.X, To.Y, FromAngle, ToAngle);
        }

        /// <summary>
        /// Adjusts the coordinates after canvas expansion.
        /// If the diagram content needs to adjust itself after being repositioned when the canvas is expanded.
        /// </summary>
        /// <param name="horizontalExpandedBy">The horizontalExpandedBy adjustment.</param>
        /// <param name="verticalExpandedBy">The verticalExpandedBy adjustment.</param>
        public void AdjustCoordinatesAfterCanvasExpansion(double horizontalExpandedBy, double verticalExpandedBy)
        {
            From = new Point(From.X + horizontalExpandedBy, From.Y + verticalExpandedBy);
            To = new Point(To.X + horizontalExpandedBy, To.Y + verticalExpandedBy);
        }

        public int CompareTo(object obj)
        {
            return CompareTo((ConnectionLine)obj);
        }

        public int CompareTo(ConnectionLine other)
        {
            if (other == null)
            {
                throw new ArgumentNullResourceException("other", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            return Distance.CompareTo(other.Distance);
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
            ConnectionLine route = FindBestConnectionRoute(this.parent1.Area, this.parent2.Area, this.isOverlapping);
            ClonePropertiesIntoThisInstance(route);

            return new ParentHasMovedNotificationResult(route.From);
        }

        /// <summary>
        /// Gives a data context for a diagram element the opportunity to listen to parent events when the <see cref="DiagramElement"/> is created.
        /// </summary>
        /// <param name="dependentElements">This object is dependent on the position of these elements</param>
        /// <param name="isOverlappingFunction">The delegate function to determine if a proposed position overlaps with any existing elements.</param>
        public void RegisterPositionDependency(IEnumerable<DiagramElement> dependentElements, Func<Area, ProximityTestResult> isOverlappingFunction)
        {
            this.isOverlapping = isOverlappingFunction;
            List<DiagramElement> elements = dependentElements.ToList();
            this.parent1 = elements.FirstOrDefault();
            this.parent2 = elements.Skip(1).FirstOrDefault();

            if (SetToolTipText(this.parent1))
            {
                return;
            }

            SetToolTipText(this.parent2);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ClonePropertiesIntoThisInstance(ConnectionLine cloneSource)
        {
            From = cloneSource.From;
            To = cloneSource.To;
            ToLineEnd = cloneSource.ToLineEnd;
            ToAngle = cloneSource.ToAngle;
            FromAngle = cloneSource.FromAngle;
            ExitAngle = cloneSource.ExitAngle;
            Distance = cloneSource.Distance;
            // Must not clone the Id, the clone is intended to seemlessly replace the original. 
            // Style = cloneSource.Style; // Don't clone style or thickness these are already set appropriately for the new type based on usage
            // Thickness = cloneSource.Thickness;
            ToolTip = cloneSource.ToolTip;
        }

        private bool SetToolTipText(DiagramElement parent)
        {
            if (parent != null)
            {
                var association = parent.DiagramContent as Association; // Must be association to include parents and fields.
                if (association != null && !(association is SubjectAssociation)) // Must not be the main subject but rather the element being pointed at.
                {
                    this.primaryParent = parent;
                    ToolTip = association.Name;
                    return true;
                }
            }

            return false;
        }
    }
}