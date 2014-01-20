using System;
using TypeVisualiser.Geometry;

namespace TypeVisualiser.Model
{
    /// <summary>
    /// An interface to encapsulate obtaining the best connection line from area to area.
    /// </summary>
    internal interface IConnectorBuilder
    {
        /// <summary>
        /// Calculate the best line connecting <see cref="fromArea"/> to <see cref="destinationArea"/>
        /// </summary>
        ConnectionLine CalculateBestConnection(Area fromArea, Area destinationArea, Func<Area, ProximityTestResult> isOverlappingWithOtherControls);
    }
}
