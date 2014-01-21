namespace TypeVisualiser.Model
{
    using System;
    using Geometry;

    internal class SubjectAssociation : Association
    {
        public SubjectAssociation(IVisualisableTypeWithAssociations subject)
        {
            AssociatedTo = subject;
        }

        internal override void StyleLine(ConnectionLine line)
        {
            // A subject association may still be asked to style a line if a secondary relationship points back to the subject.
            FieldAssociation.StyleLineForNonParentAssociation(line, 1, AssociatedTo, IsTrivialAssociation());
        }

        public override string Name
        {
            get { return "Subject"; }
        }

        internal override ArrowHead CreateLineHead()
        {
            // A subject association may still be asked to style an arrowhead if a secondary relationship points back to the subject.
            return new AssociationArrowHead();
        }

        public override Area ProposePosition(double actualWidth, double actualHeight, Area subjectArea, Func<Area, ProximityTestResult> overlapsWithOthers)
        {
            return subjectArea;
        }
    }
}