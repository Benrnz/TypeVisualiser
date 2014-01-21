using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using TypeVisualiser.Geometry;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Model
{
    public class ParentAssociation : Association
    {
        private readonly string fieldName;

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Reviewed ok here")]
        public ParentAssociation(IModelBuilder modelFactory, Type parent, IVisualisableTypeWithAssociations from) : this(modelFactory.BuildSubject(parent, 0))
        {
            AssociatedFrom = from;
        }

        private ParentAssociation(IVisualisableType parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullResourceException("parent", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            AssociatedTo = parent;
            if (parent.Modifiers.Kind == TypeKind.Interface)
            {
                this.fieldName = "Implements: " + parent.Name;
            } else
            {
                this.fieldName = "Inherits: " + parent.Name;
            }
        }

        public override string Name
        {
            get { return this.fieldName; }
        }

        public IVisualisableTypeWithAssociations AssociatedFrom { get; private set; }

        public override Area ProposePosition(double actualWidth, double actualHeight, Area subjectArea, Func<Area, ProximityTestResult> overlapsWithOthers)
        {
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
            proposedTopLeft.Offset(0, -(actualHeight + 2.5 * ArrowHead.ArrowWidth));
            var proposedArea = new Area(proposedTopLeft, actualWidth, actualHeight);

            while (overlapsWithOthers(proposedArea).Proximity == Proximity.Overlapping)
            {
                // Try left
                Area moveLeftProposal = proposedArea;
                do
                {
                    moveLeftProposal = moveLeftProposal.Offset(-(actualWidth + LayoutConstants.MinimumDistanceBetweenObjects), 0);
                } while (overlapsWithOthers(moveLeftProposal).Proximity == Proximity.Overlapping);

                // Try right
                Area moveRightProposal = proposedArea;
                Proximity proximity = overlapsWithOthers(moveRightProposal).Proximity;
                while (proximity != Proximity.NotOverlapping)
                {
                    if (proximity == Proximity.Overlapping)
                    {
                        moveRightProposal = moveRightProposal.Offset(actualWidth + LayoutConstants.MinimumDistanceBetweenObjects, 0);
                    } else
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
            return new InheritanceArrowHead();
        }

        internal override void StyleLine(ConnectionLine line)
        {
            if (line == null)
            {
                throw new ArgumentNullResourceException("line", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (AssociatedTo.Modifiers.Kind == TypeKind.Interface)
            {
                line.Style = "ImplementsLine";
            } else
            {
                line.Style = "InheritanceLine";
            }
        }
    }
}