namespace TypeVisualiser.Model
{
    /// <summary>
    /// An interface for diagram dimension data for the whole diagram. This data is mostly layout data.
    /// </summary>
    public interface IDiagramDimensions
    {
        double AngleIncrement { get; }
        double FinishAngle { get; }
        double StartAngle { get; }
        int TotalAssociationsOnDiagram { get; }
        void Initialise(int totalAssociationsOnDiagram, bool areParentsShown);
        double CalculateNextAvailableAngle();
    }
}