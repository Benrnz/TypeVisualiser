namespace TypeVisualiser.Geometry
{
    [System.Diagnostics.DebuggerDisplay("ProximityResult {Proximity}")]
    public class ProximityTestResult
    {
        public ProximityTestResult(Proximity proximity)
        {
            Proximity = proximity;
            DistanceToClosestObject = double.NaN;
        }

        public double DistanceToClosestObject { get; set; }

        /// <summary>
        /// Gets or sets the direction to other object. Only if it is very close.
        /// </summary>
        /// <value>The direction to other object.</value>
        public Direction DirectionToOtherObject { get; set; }

        /// <summary>
        /// Gets or sets the proximity to the compared area.
        /// </summary>
        /// <value>The proximity.</value>
        public Proximity Proximity { get; private set; }
    }
}