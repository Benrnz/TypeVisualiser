namespace TypeVisualiserUnitTests.Model
{
    using System;

    using TypeVisualiser;
    using TypeVisualiser.Geometry;
    using TypeVisualiser.Model;

    public class TestAssociation : Association
    {
        public TestAssociation(VisualisableType associatedTo, ITrivialFilter filter)
            : base(null, filter)
        {
            AssociatedTo = associatedTo;
        }

        public TestAssociation(IApplicationResources stubApplicationResources, ITrivialFilter filter, VisualisableType associatedTo)
            : base(stubApplicationResources, filter)
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

        public TestAssociation Initialise()
        {
            IsInitialised = true;
            return this;
        }
    }
}