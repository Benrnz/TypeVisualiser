namespace TypeVisualiser.Model
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    using TypeVisualiser.Geometry;
    using TypeVisualiser.Model.Persistence;
    using TypeVisualiser.Properties;

    public class ParentAssociation : Association
    {
        private readonly IModelBuilder modelBuilder;

        private string fieldName;

        public ParentAssociation(IApplicationResources resources, ITrivialFilter trivialFilter, IModelBuilder modelFactory)
            : base(resources, trivialFilter)
        {
            this.modelBuilder = modelFactory;
        }

        public IVisualisableTypeWithAssociations AssociatedFrom { get; private set; }

        public override string Name
        {
            get
            {
                return this.fieldName;
            }
        }

        /// <summary>
        /// Must be called immediately after the constructor.
        /// It is separate from the constructor to allow this type to be created by an IoC container.
        /// </summary>
        /// <param name="parent">
        /// The .NET type of the superclass/base class for <paramref name="from"/>.
        /// </param>
        /// <param name="from">
        /// The sub-class of <paramref name="parent"/>.
        /// </param>
        /// <returns>
        /// Itself for chaining.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Simplist way to require model builder argument")]
        public ParentAssociation Initialise(Type parent, IVisualisableTypeWithAssociations from)
        {
            if (parent == null)
            {
                throw new ArgumentNullResourceException("parent", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (from == null)
            {
                throw new ArgumentNullResourceException("from", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (this.modelBuilder == null)
            {
                throw new ArgumentNullResourceException("modelBuilder", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            IVisualisableTypeWithAssociations parentVisualisableType = this.modelBuilder.BuildSubject(parent, 0);
            if (parentVisualisableType == null)
            {
                throw new ArgumentNullResourceException("parentVisualisableType", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            this.AssociatedTo = parentVisualisableType;
            if (parentVisualisableType.Modifiers.Kind == TypeKind.Interface)
            {
                this.fieldName = "Implements: " + parentVisualisableType.Name;
            }
            else
            {
                this.fieldName = "Inherits: " + parentVisualisableType.Name;
            }

            this.AssociatedFrom = from;

            this.IsInitialised = true;
            return this;
        }

        public override Area ProposePosition(double actualWidth, double actualHeight, Area subjectArea, Func<Area, ProximityTestResult> overlapsWithOthers)
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

            if (subjectArea == null)
            {
                throw new ArgumentNullResourceException("subjectArea", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (overlapsWithOthers == null)
            {
                throw new ArgumentNullResourceException("overlapsWithOthers", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            Point proposedTopLeft = subjectArea.TopLeft.Clone();

            // Suggest directly above
            proposedTopLeft.Offset(0, -(actualHeight + (2.5 * ArrowHead.ArrowWidth)));
            var proposedArea = new Area(proposedTopLeft, actualWidth, actualHeight);

            while (overlapsWithOthers(proposedArea).Proximity == Proximity.Overlapping)
            {
                // Try left
                Area moveLeftProposal = proposedArea;
                do
                {
                    moveLeftProposal = moveLeftProposal.Offset(-(actualWidth + LayoutConstants.MinimumDistanceBetweenObjects), 0);
                }
                while (overlapsWithOthers(moveLeftProposal).Proximity == Proximity.Overlapping);

                // Try right
                Area moveRightProposal = proposedArea;
                Proximity proximity = overlapsWithOthers(moveRightProposal).Proximity;
                while (proximity != Proximity.NotOverlapping)
                {
                    if (proximity == Proximity.Overlapping)
                    {
                        moveRightProposal = moveRightProposal.Offset(actualWidth + LayoutConstants.MinimumDistanceBetweenObjects, 0);
                    }
                    else
                    {
                        moveRightProposal = moveRightProposal.Offset(LayoutConstants.MinimumDistanceBetweenObjects / 2, 0);
                    }

                    proximity = overlapsWithOthers(moveRightProposal).Proximity;
                }

                proposedArea = moveLeftProposal.DistanceToPoint(subjectArea.TopLeft) <= moveRightProposal.DistanceToPoint(subjectArea.TopLeft) ? moveLeftProposal : moveRightProposal;
            }

            return proposedArea;
        }

        internal override ArrowHead CreateLineHead()
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

            return new InheritanceArrowHead();
        }

        internal override void StyleLine(ConnectionLine line)
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

            if (line == null)
            {
                throw new ArgumentNullResourceException("line", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            line.Style = this.AssociatedTo.Modifiers.Kind == TypeKind.Interface ? "ImplementsLine" : "InheritanceLine";
        }
    }
}