using System;
using System.Collections.Generic;
using TypeVisualiser.Geometry;

namespace TypeVisualiser.Model
{
    /// <summary>
    /// An interface to encapsulate any diagram content that can appear inside a <see cref="DiagramElement"/>.
    /// </summary>
    public interface IDiagramContent
    {
        /// <summary>
        /// Gets an id that uniquely identifies the diagram content. (This is probably the VisualisableType.Id - which is the Type.GUID or a concatentation 
        /// of Type.GUID's for generic types.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Adjusts the coordinates after canvas expansion.
        /// If the diagram content needs to adjust itself after being repositioned when the canvas is expanded.
        /// </summary>
        /// <param name="horizontalExpandedBy">The horizontalExpandedBy adjustment.</param>
        /// <param name="verticalExpandedBy">The verticalExpandedBy adjustment.</param>
        void AdjustCoordinatesAfterCanvasExpansion(double horizontalExpandedBy, double verticalExpandedBy);

        /// <summary>
        /// Notifies the data context (represented by this Diagram Content) that a previously registered position-dependent diagram element has moved.
        /// </summary>
        /// <param name="dependentElement">The dependent element.</param>
        /// <returns>A result containing information if layout changes are required with new suggested values.</returns>
        ParentHasMovedNotificationResult NotifyDiagramContentParentHasMoved(DiagramElement dependentElement);

        /// <summary>
        /// Gives a data context (represented by this Diagram Content) for a diagram element the opportunity to listen to parent events when the <see cref="DiagramElement"/> is created.
        /// </summary>
        /// <param name="dependentElements">This object is dependent on the position of these elements</param>
        /// <param name="isOverlappingFunction">The delegate function to determine if a proposed position overlaps with any existing elements.</param>
        void RegisterPositionDependency(IEnumerable<DiagramElement> dependentElements, Func<Area, ProximityTestResult> isOverlappingFunction);
    }
}