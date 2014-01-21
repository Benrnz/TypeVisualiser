namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using Geometry;
    using Persistence;
    using Properties;

    public class FieldAssociation : Association
    {
        private double angle;
        private IDiagramDimensions doNotUseDimensions;

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It cannot be validated before constructor chaining.")]
        public FieldAssociation(Type associatedTo, int numberOfUsages, IEnumerable<AssociationUsageDescriptor> usageDescriptors, int depth)
            : this(associatedTo, depth)
        {
            if (usageDescriptors == null)
            {
                throw new ArgumentNullResourceException("usageDescriptors", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (associatedTo == null)
            {
                throw new ArgumentNullResourceException("associatedTo", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            UsageCount = numberOfUsages;

            // Clean the field names 
            var autoPropertyRegex = new Regex(@"^\<(.*)\>k__BackingField$");
            List<AssociationUsageDescriptor> usageList = usageDescriptors.ToList();
            foreach (AssociationUsageDescriptor usage in usageList)
            {
                Match autoPropertyMatch = autoPropertyRegex.Match(usage.Description);
                if (autoPropertyMatch.Success)
                {
                    usage.Description = autoPropertyMatch.Groups[1].Value;
                }
            }

            UsageDescriptorList = usageList;
        }

        protected FieldAssociation(Type type, int depth)
        {
            if (type == null)
            {
                throw new ArgumentNullResourceException("type", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (type.IsGenericParameter && type.GUID == Guid.Empty)
            {
                AssociatedTo = new ModelBuilder().BuildVisualisableType(type.BaseType, depth);
            } else
            {
                AssociatedTo = new ModelBuilder().BuildVisualisableType(type, depth);
            }
        }

        public override string Name
        {
            get
            {
                if (!UsageDescriptorList.Any())
                {
                    throw new InvalidOperationException("Usage Descriptor collection should not be empty.");
                }

                if (UsageDescriptorList.Any(x => x.Kind == MemberKind.Method))
                {
                    return "Field and consumption relationships (click for more details)";
                }

                int count = UsageDescriptorList.Count();
                if (count > 2)
                {
                    return string.Format(CultureInfo.CurrentCulture, "{0} fields (click for more details)", count);
                }

                if (count == 2)
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "2 fields ({0}, {1})",
                        UsageDescriptorList.First().Description,
                        UsageDescriptorList.ElementAt(1).Description);
                }

                return string.Format(CultureInfo.CurrentCulture, "1 field ({0})", UsageDescriptorList.First().Description);
            }
        }

        /// <summary>
        /// Gets and sets the usage count. This is the number of times this associated type is used by the subject.
        /// This number can be more than the count of UsageDescriptorList. This is because the number of fields using
        /// the association can be less the total consumption references.
        /// </summary>
        public int UsageCount { get; set; }

        public IEnumerable<AssociationUsageDescriptor> UsageDescriptors
        {
            get
            {
                return UsageDescriptorList;
            }
        }

        internal virtual Type PersistenceType
        {
            get
            {
                return typeof(FieldAssociationData);
            }
        }

        protected IDiagramDimensions Dimensions
        {
            get
            {
                return this.doNotUseDimensions ?? (this.doNotUseDimensions = Factory.GetInstance<IDiagramDimensions>());
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Must be settable by subclass, this is simpler.")]
        protected IList<AssociationUsageDescriptor> UsageDescriptorList { get; set; }

        internal override ArrowHead CreateLineHead()
        {
            return new AssociationArrowHead();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var otherField = obj as FieldAssociation;
            if (otherField == null)
            {
                return false;
            }

            return AssociatedTo.AssemblyQualifiedName.Equals(otherField.AssociatedTo.AssemblyQualifiedName);
        }

        public override int GetHashCode()
        {
            return AssociatedTo.AssemblyQualifiedName.GetHashCode();
        }

        public override Area ProposePosition(
            double actualWidth, double actualHeight, Area subjectArea, Func<Area, ProximityTestResult> overlapsWithOthers)
        {
            if (subjectArea == null)
            {
                throw new ArgumentNullResourceException("subjectArea", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            return GetProposedAreaSemiCircle(actualWidth, actualHeight, subjectArea.Centre, overlapsWithOthers);
        }

        internal override void StyleLine(ConnectionLine line)
        {
            StyleLineForNonParentAssociation(line, UsageCount, AssociatedTo, IsTrivialAssociation());
        }

        internal static void StyleLineForNonParentAssociation(ConnectionLine line, int usageCount, IVisualisableType associatedTo, bool isTrivial)
        {
            if (line == null)
            {
                throw new ArgumentNullResourceException("line", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            double thickness = 2.25 + (usageCount * 0.75);
            if (thickness > 15)
            {
                thickness = 15;
            }

            if (associatedTo.Modifiers.Kind == TypeKind.Interface)
            {
                line.Style = "AssociationLineInterface";
                line.Thickness = thickness;
            } else if (isTrivial)
            {
                line.Style = "AssociationLineTrivial";
                line.Thickness = thickness;
            } else
            {
                line.Style = "AssociationLineStrong";
                line.Thickness = thickness + 1;
            }
        }

        internal void Merge(FieldAssociation association)
        {
            UsageCount += association.UsageCount;
            foreach (AssociationUsageDescriptor descriptor in association.UsageDescriptorList)
            {
                UsageDescriptorList.Add(descriptor);
            }
        }

        private Area GetProposedAreaSemiCircle(double actualWidth, double actualHeight, Point centre, Func<Area, ProximityTestResult> overlapsWithOthers)
        {
            // A angle of 0 degrees in this context is moving directly to the right
            this.angle = Dimensions.CalculateNextAvailableAngle();
            Area proposedArea;
            var calc = new CircleCalculator(centre, this.angle);
            double radius = 250;
            ProximityTestResult proximityResult = null;
            
            do
            {
                if (proximityResult != null && proximityResult.Proximity == Proximity.VeryClose)
                {
                    radius += LayoutConstants.MinimumDistanceBetweenObjects / 2;
                } else
                {
                    radius += LayoutConstants.MinimumDistanceBetweenObjects;
                }

                proposedArea = new Area(calc.CalculatePointOnCircle(radius), actualWidth, actualHeight);
                proposedArea = proposedArea.OffsetToMakeTopLeftCentre();
                proximityResult = overlapsWithOthers(proposedArea);
            } while (proximityResult.Proximity == Proximity.Overlapping || proximityResult.Proximity == Proximity.VeryClose);

            return proposedArea;
        }
    }
}