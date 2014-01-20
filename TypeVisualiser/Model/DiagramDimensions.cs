namespace TypeVisualiser.Model
{
    using System.Collections.Generic;
    using System.Linq;

    internal class DiagramDimensions : IDiagramDimensions
    {
        private List<double> angles;

        public double AngleIncrement { get; private set; }
        public double FinishAngle { get; private set; }
        public double StartAngle { get; private set; }
        public int TotalAssociationsOnDiagram { get; private set; }

        public void Initialise(int totalAssociationsOnDiagram, bool areParentsShown)
        {
            TotalAssociationsOnDiagram = totalAssociationsOnDiagram;
            if (areParentsShown)
            {
                AngleIncrement = 200.0 / TotalAssociationsOnDiagram;
                StartAngle = -10.0;
                FinishAngle = 190.0;
            } else
            {
                AngleIncrement = 360.0 / TotalAssociationsOnDiagram;
                StartAngle = 0.0;
                FinishAngle = 360.0;
            }

            if (AngleIncrement > 90)
            {
                AngleIncrement = 90;
            }

            angles = new List<double>();
        }

        public double CalculateNextAvailableAngle()
        {
            if (angles.Any())
            {
                double a = angles.Max() + AngleIncrement;
                angles.Add(a);
                return a;
            }

            angles.Add(StartAngle);
            return StartAngle;
        }

    }
}
