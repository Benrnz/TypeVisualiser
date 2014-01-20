using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using TypeVisualiser.Geometry;
using TypeVisualiser.Messaging;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Model
{
    /// <summary>
    /// A class to represent a diagram element's layout properties. Layout properties such as position and width etc should be kept
    /// separate from the model.  This type should not be tied to the Type Visualiser type.
    /// </summary>
    [DebuggerDisplay("DiagramElement Content={DiagramContent}")]
    public class DiagramElement : INotifyPropertyChanged, ICleanup
    {
        private readonly Guid diagramId;
        private bool clean;
        private IDiagramContent doNotUseDiagramContent;

        /// <summary>
        /// This flag indicates that position changed message should not be sent. Used to stop excessive messages from being sent when multiple properties are
        /// being sent.
        /// </summary>
        private bool postponeEvent;

        private bool show;
        private Point topLeft;

        public DiagramElement(Guid diagramId, IDiagramContent diagramContent)
        {
            if (diagramContent == null)
            {
                throw new ArgumentNullResourceException("diagramContent", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            RelatedDiagramElements = new List<DiagramElement>();
            this.diagramId = diagramId;
            DiagramContent = diagramContent;
            ZOrder = 1;
            Show = true;
            IsVisibleAdditionalLogic = (_, show) => show; // By default no additional logic when determining if something should be visible. Just return what you're given.
            MessagingGate.Register<DiagramElementChangedMessage>(this, OnParentPositionChanged);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Area Area
        {
            get { return new Area(TopLeft, Width, Height) { Tag = DiagramContent.ToString() }; }
        }

        public IDiagramContent DiagramContent
        {
            get { return this.doNotUseDiagramContent; }

            private set
            {
                this.doNotUseDiagramContent = value;
                RaisePropertyChanged("DiagramContent");
            }
        }

        public double Height { get; set; }
        public Func<DiagramElement, bool, bool> IsVisibleAdditionalLogic { get; set; }
        public IEnumerable<DiagramElement> RelatedDiagramElements { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this diagram element is shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shown; otherwise, <c>false</c>.
        /// </value>
        public bool Show
        {
            get { return this.show; }

            set
            {
                this.show = value;
                RaisePropertyChanged("Show");
                RaisePositionChangedEvent();
            }
        }

        public Point TopLeft
        {
            get { return this.topLeft; }
            set
            {
                if (value == this.topLeft)
                {
                    return;
                }

                this.topLeft = value;
                RaisePropertyChanged("TopLeft");
                RaisePositionChangedEvent();
            }
        }

        public double Width { get; set; }
        public int ZOrder { get; set; }

        /// <summary>
        /// Adjusts the coordinates after canvas expansion.
        /// If the diagram content needs to adjust itself after being repositioned when the canvas is expanded.
        /// </summary>
        /// <param name="horizontalExpandedBy">The horizontalExpandedBy adjustment.</param>
        /// <param name="verticalExpandedBy">The verticalExpandedBy adjustment.</param>
        public void AdjustCoordinatesAfterCanvasExpansion(double horizontalExpandedBy, double verticalExpandedBy)
        {
            TopLeft = new Point(TopLeft.X + horizontalExpandedBy, TopLeft.Y + verticalExpandedBy);
            DiagramContent.AdjustCoordinatesAfterCanvasExpansion(horizontalExpandedBy, verticalExpandedBy);
        }

        public void CenterOnPoint(Point point)
        {
            TopLeft = new Point(point.X - Width / 2, point.Y - Height / 2);
        }

        public void Cleanup()
        {
            if (this.clean)
            {
                return;
            }

            if (RelatedDiagramElements != null)
            {
                foreach (DiagramElement element in RelatedDiagramElements)
                {
                    element.Cleanup();
                }
            }

            var contentRequiringCleanup = DiagramContent as ICleanup;
            if (contentRequiringCleanup != null)
            {
                contentRequiringCleanup.Cleanup();
            }

            this.clean = true;
        }

        public void RefreshPosition()
        {
            RaisePositionChangedEvent();
        }

        public void RegisterPositionDependency(IEnumerable<DiagramElement> dependentElements, Func<Area, ProximityTestResult> isOverlappingFunction)
        {
            RelatedDiagramElements = dependentElements.ToList();
            DiagramContent.RegisterPositionDependency(RelatedDiagramElements, isOverlappingFunction);
        }

        private void OnParentPositionChanged(DiagramElementChangedMessage message)
        {
            DiagramElement parentElement = message.ChangedElement;
            // Is this message to do with my diagram?
            if (this.diagramId != message.DiagramId || parentElement == null)
            {
                return;
            }

            // Is this message related to another diagram element I'm interested in?
            if (!RelatedDiagramElements.Contains(parentElement))
            {
                return;
            }

            ParentHasMovedNotificationResult result = DiagramContent.NotifyDiagramContentParentHasMoved(parentElement);
            if (result.LayoutChangesRequired)
            {
                this.postponeEvent = true;
                bool shouldShow = RelatedDiagramElements.All(e => e.Show);
                shouldShow = IsVisibleAdditionalLogic(this, shouldShow);
                Show = shouldShow;
                ZOrder = result.NewZOrder;
                TopLeft = result.NewTopLeftLocation;
                this.postponeEvent = false;
                RaisePositionChangedEvent();
            }
        }

        private void RaisePositionChangedEvent()
        {
            if (this.postponeEvent)
            {
                return;
            }

            MessagingGate.Send(new DiagramElementChangedMessage(this.diagramId) { ChangedElement = this });
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}