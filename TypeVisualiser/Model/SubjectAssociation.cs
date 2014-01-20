namespace TypeVisualiser.Model
{
    using System;

    using TypeVisualiser.Geometry;

    internal class SubjectAssociation : Association
    {
        public SubjectAssociation(IApplicationResources resources, ITrivialFilter trivialFilter)
            : base(resources, trivialFilter)
        {
        }

        public override string Name
        {
            get
            {
                return "Subject";
            }
        }

        public SubjectAssociation Initialise(IVisualisableTypeWithAssociations subject)
        {
            this.AssociatedTo = subject;
            this.IsInitialised = true;
            return this;
        }

        public override Area ProposePosition(double actualWidth, double actualHeight, Area subjectArea, Func<Area, ProximityTestResult> overlapsWithOthers)
        {
            return subjectArea;
        }

        internal override ArrowHead CreateLineHead()
        {
            // A subject association may still be asked to style an arrowhead if a secondary relationship points back to the subject.
            return new AssociationArrowHead();
        }

        internal override void StyleLine(ConnectionLine line)
        {
            // A subject association may still be asked to style a line if a secondary relationship points back to the subject.
            FieldAssociation.StyleLineForNonParentAssociation(line, 1, this.AssociatedTo, this.IsTrivialAssociation());
        }
    }
}