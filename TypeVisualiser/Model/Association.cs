using System;
using System.Collections.Generic;
using System.Globalization;
using TypeVisualiser.Geometry;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.Properties;
using TypeVisualiser.Startup;
using IContainer = StructureMap.IContainer;

namespace TypeVisualiser.Model
{
    [Persistent]
    public abstract class Association : ICalculatedPositionDiagramContent
    {
        private IContainer doNotUseFactory;
        private IApplicationResources doNotUseResources;

        public abstract string Name { get; }
        public IVisualisableType AssociatedTo { get; protected set; }

        public string Id
        {
            get { return AssociatedTo.Id; }
        }

        protected IApplicationResources ApplicationResources
        {
            get { return this.doNotUseResources ?? (this.doNotUseResources = Factory.GetInstance<IApplicationResources>()); }

            set { this.doNotUseResources = value; }
        }

        protected IContainer Factory
        {
            get { return this.doNotUseFactory ?? (this.doNotUseFactory = IoC.Default); }
        }

        /// <summary>
        /// Proposes a position for the associate.
        /// </summary>
        /// <param name="actualWidth">The actual width of the associate UI element.</param>
        /// <param name="actualHeight">The actual height of the associate UI element.</param>
        /// <param name="subjectArea">The subject area.</param>
        /// <param name="overlapsWithOthers">The function delegate that determines if the proposed area overlaps with others.</param>
        /// <returns></returns>
        public abstract Area ProposePosition(double actualWidth, double actualHeight, Area subjectArea, Func<Area, ProximityTestResult> overlapsWithOthers);

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}: {1}", GetType().Name, AssociatedTo.Name);
        }

        /// <summary>
        /// Adjusts the coordinates after canvas expansion.
        /// If the diagram content needs to adjust itself after being repositioned when the canvas is expanded.
        /// </summary>
        /// <param name="horizontalExpandedBy">The horizontalExpandedBy adjustment.</param>
        /// <param name="verticalExpandedBy">The verticalExpandedBy adjustment.</param>
        public void AdjustCoordinatesAfterCanvasExpansion(double horizontalExpandedBy, double verticalExpandedBy)
        {
            // Not required for this sub-class
        }

        public bool IsTrivialAssociation()
        {
            return IsTrivialAssociation(TrivialFilter.Current);
        }

        public bool IsTrivialAssociation(ITrivialFilter trivialFilter)
        {
            if (trivialFilter == null)
            {
                throw new ArgumentNullResourceException("trivialFilter", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (AssociatedTo.Modifiers.Kind == TypeKind.Interface)
            {
                return true;
            }

            if (AssociatedTo.Modifiers.Kind == TypeKind.Enum)
            {
                return true;
            }

            if (AssociatedTo.Modifiers.Kind == TypeKind.ValueType)
            {
                return true;
            }

            return trivialFilter.IsTrivialType(AssociatedTo.NamespaceQualifiedName);
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
            // Not required for this sub-class
            return new ParentHasMovedNotificationResult();
        }

        /// <summary>
        /// Gives a data context for a diagram element the opportunity to listen to parent events when the <see cref="DiagramElement"/> is created.
        /// </summary>
        /// <param name="dependentElements">This object is dependent on the position of these elements</param>
        /// <param name="isOverlappingFunction">The delegate function to determine if a proposed position overlaps with any existing elements.</param>
        public void RegisterPositionDependency(IEnumerable<DiagramElement> dependentElements, Func<Area, ProximityTestResult> isOverlappingFunction)
        {
            // Not required for this sub-class
        }

        internal abstract ArrowHead CreateLineHead();

        internal abstract void StyleLine(ConnectionLine line);
    }
}