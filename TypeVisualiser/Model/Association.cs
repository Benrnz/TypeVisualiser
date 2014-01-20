namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using TypeVisualiser.Geometry;
    using TypeVisualiser.Model.Persistence;
    using TypeVisualiser.Properties;

    [Persistent]
    public abstract class Association : ICalculatedPositionDiagramContent
    {
        private readonly ITrivialFilter trivialFilter;

        protected Association(IApplicationResources resources, ITrivialFilter trivialFilter)
        {
            this.ApplicationResources = resources;
            this.trivialFilter = trivialFilter;
        }

        public IVisualisableType AssociatedTo { get; protected set; }

        public string Id
        {
            get
            {
                return this.AssociatedTo.Id;
            }
        }

        public abstract string Name { get; }

        protected IApplicationResources ApplicationResources { get; set; }

        protected bool IsInitialised { get; set; }

        /// <summary>
        /// Adjusts the coordinates after canvas expansion.
        /// If the diagram content needs to adjust itself after being repositioned when the canvas is expanded.
        /// </summary>
        /// <param name="horizontalExpandedBy">
        /// The horizontalExpandedBy adjustment.
        /// </param>
        /// <param name="verticalExpandedBy">
        /// The verticalExpandedBy adjustment.
        /// </param>
        public void AdjustCoordinatesAfterCanvasExpansion(double horizontalExpandedBy, double verticalExpandedBy)
        {
            // Not required for this sub-class
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "TrivialFilter is passed into the class. ArgumentNullException is an adaquate way of describing this.")]
        public bool IsTrivialAssociation()
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

            if (this.trivialFilter == null)
            {
                throw new ArgumentNullResourceException("trivialFilter", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (this.AssociatedTo.Modifiers.Kind == TypeKind.Interface)
            {
                return true;
            }

            if (this.AssociatedTo.Modifiers.Kind == TypeKind.Enum)
            {
                return true;
            }

            if (this.AssociatedTo.Modifiers.Kind == TypeKind.ValueType)
            {
                return true;
            }

            return this.trivialFilter.IsTrivialType(this.AssociatedTo.NamespaceQualifiedName);
        }

        /// <summary>
        /// Notifies the data context (represented by this Diagram Content) that a previously registered position-dependent diagram element has moved.
        /// </summary>
        /// <param name="dependentElement">
        /// The dependent element.
        /// </param>
        /// <returns>
        /// A result containing information if layout changes are required with new suggested values.
        /// </returns>
        public ParentHasMovedNotificationResult NotifyDiagramContentParentHasMoved(DiagramElement dependentElement)
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

            // Not required for this sub-class
            return new ParentHasMovedNotificationResult();
        }

        /// <summary>
        /// Proposes a position for the associate.
        /// </summary>
        /// <param name="actualWidth">
        /// The actual width of the associate UI element.
        /// </param>
        /// <param name="actualHeight">
        /// The actual height of the associate UI element.
        /// </param>
        /// <param name="subjectArea">
        /// The subject area.
        /// </param>
        /// <param name="overlapsWithOthers">
        /// The function delegate that determines if the proposed area overlaps with others.
        /// </param>
        /// <returns>
        /// The <see cref="Area"/>.
        /// </returns>
        public abstract Area ProposePosition(double actualWidth, double actualHeight, Area subjectArea, Func<Area, ProximityTestResult> overlapsWithOthers);

        /// <summary>
        /// Gives a data context for a diagram element the opportunity to listen to parent events when the <see cref="DiagramElement"/> is created.
        /// </summary>
        /// <param name="dependentElements">
        /// This object is dependent on the position of these elements
        /// </param>
        /// <param name="isOverlappingFunction">
        /// The delegate function to determine if a proposed position overlaps with any existing elements.
        /// </param>
        public void RegisterPositionDependency(IEnumerable<DiagramElement> dependentElements, Func<Area, ProximityTestResult> isOverlappingFunction)
        {
            // Not required for this sub-class
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}: {1}", this.GetType().Name, this.AssociatedTo.Name);
        }

        internal abstract ArrowHead CreateLineHead();

        internal abstract void StyleLine(ConnectionLine line);

        protected static void CannotUseWithoutInitializationFirst()
        {
            throw new InvalidOperationException("Usage Error: This instance has not been initialized and cannot be used until it is.");
        }
    }
}