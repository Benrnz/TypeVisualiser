namespace TypeVisualiserUnitTests.Model
{
    using System;
    using TypeVisualiser.Geometry;
    using TypeVisualiser.Model;

    public class TestAssociation : Association
    {
        public TestAssociation(VisualisableType associatedTo)
        {
            AssociatedTo = associatedTo;
        }

        public override string Name
        {
            get
            {
                return AssociatedTo.AssemblyQualifiedName;
            }
        }

        internal override ArrowHead CreateLineHead()
        {
            throw new NotImplementedException();
        }

        public override Area ProposePosition(double actualWidth, double actualHeight, Area subjectArea, Func<Area, ProximityTestResult> overlapsWithOthers)
        {
            throw new NotImplementedException();
        }

        internal override void StyleLine(ConnectionLine line)
        {
            throw new NotImplementedException();
        }
    }
}