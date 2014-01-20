using System;
using System.Collections.Generic;
using System.ComponentModel;
using TypeVisualiser.Geometry;

namespace TypeVisualiser.Model.Persistence
{
    [Persistent]
    public class AnnotationData : INotifyPropertyChanged, IDiagramContent
    {
        private string text;

        public AnnotationData()
        {
            Id = Guid.NewGuid().ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; set; }

        public string Text
        {
            get { return this.text; }

            set
            {
                this.text = value;
                RaisePropertyChanged("Text");
            }
        }

        /// <summary>
        /// Adjusts the coordinates after canvas expansion.
        /// If the diagram content needs to adjust itself after being repositioned when the canvas is expanded.
        /// </summary>
        /// <param name="horizontalExpandedBy">The horizontalExpandedBy adjustment.</param>
        /// <param name="verticalExpandedBy">The verticalExpandedBy adjustment.</param>
        public void AdjustCoordinatesAfterCanvasExpansion(double horizontalExpandedBy, double verticalExpandedBy)
        {
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
            return new ParentHasMovedNotificationResult();
        }

        /// <summary>
        /// Gives a data context (represented by this Diagram Content) for a diagram element the opportunity to listen to parent events when the <see cref="DiagramElement"/> is created.
        /// </summary>
        /// <param name="dependentElements">This object is dependent on the position of these elements</param>
        /// <param name="isOverlappingFunction">The delegate function to determine if a proposed position overlaps with any existing elements.</param>
        public void RegisterPositionDependency(IEnumerable<DiagramElement> dependentElements, Func<Area, ProximityTestResult> isOverlappingFunction)
        {
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}