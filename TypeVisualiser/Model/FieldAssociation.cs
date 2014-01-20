namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;

    using TypeVisualiser.Geometry;
    using TypeVisualiser.Model.Persistence;
    using TypeVisualiser.Properties;

    /// <summary>
    /// The field association.
    /// </summary>
    public class FieldAssociation : Association
    {
        private readonly IModelBuilder modelBuilder;

        // ReSharper disable FieldCanBeMadeReadOnly.Local

        //// ReSharper restore FieldCanBeMadeReadOnly.Local
        private double angle;

        private IDiagramDimensions dimensions; // Need to set this in testing

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAssociation"/> class. 
        /// Only actual instances of <see cref="FieldAssociation"/> use this constructor. Sub-classes use the other.
        /// </summary>
        /// <param name="resources">
        /// The application resources.
        /// </param>
        /// <param name="trivialFilter">
        /// The trivial filter to use to determine the kind of relationship. Used for styling decisions.
        /// </param>
        /// <param name="modelBuilder">
        /// The model Builder to be used when constructing the related <see cref="IVisualisableType"/> from the given type in <see cref="Initialise"/>.
        /// </param>
        /// <param name="diagramDimensions">
        /// The diagram Dimensions.
        /// </param>
        public FieldAssociation(IApplicationResources resources, ITrivialFilter trivialFilter, IModelBuilder modelBuilder, IDiagramDimensions diagramDimensions)
            : base(resources, trivialFilter)
        {
            this.dimensions = diagramDimensions;
            this.modelBuilder = modelBuilder;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <exception cref="InvalidOperationException">Will occur when the Usage Descriptor collection is empty.</exception>
        public override string Name
        {
            get
            {
                if (!this.UsageDescriptorList.Any())
                {
                    throw new InvalidOperationException("Usage Descriptor collection should not be empty.");
                }

                if (this.UsageDescriptorList.Any(x => x.Kind == MemberKind.Method))
                {
                    return "Field and consumption relationships (click for more details)";
                }

                int count = this.UsageDescriptorList.Count();
                if (count > 2)
                {
                    return string.Format(CultureInfo.CurrentCulture, "{0} fields (click for more details)", count);
                }

                if (count == 2)
                {
                    return string.Format(CultureInfo.CurrentCulture, "2 fields ({0}, {1})", this.UsageDescriptorList.First().Description, this.UsageDescriptorList.ElementAt(1).Description);
                }

                return string.Format(CultureInfo.CurrentCulture, "1 field ({0})", this.UsageDescriptorList.First().Description);
            }
        }

        /// <summary>
        /// Gets or sets the UsageCount. This is the number of times this associated type is used by the subject.
        /// This number can be more than the count of UsageDescriptorList. This is because the number of fields using
        /// the association can be less the total consumption references.
        /// </summary>
        public int UsageCount { get; set; }

        /// <summary>
        /// Gets the usage descriptors.
        /// </summary>
        public IEnumerable<AssociationUsageDescriptor> UsageDescriptors
        {
            get
            {
                return this.UsageDescriptorList;
            }
        }

        /// <summary>
        /// Gets the persistence type.
        /// </summary>
        internal virtual Type PersistenceType
        {
            get
            {
                return typeof(FieldAssociationData);
            }
        }

        /// <summary>
        /// Gets or sets the usage descriptor list.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Must be settable by subclass, this is simpler.")]
        protected IList<AssociationUsageDescriptor> UsageDescriptorList { get; set; }

        /// <summary>
        /// The equality comparison operation.
        /// </summary>
        /// <param name="obj">
        /// The other Object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

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

            return this.AssociatedTo.AssemblyQualifiedName.Equals(otherField.AssociatedTo.AssemblyQualifiedName);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.AssociatedTo.AssemblyQualifiedName.GetHashCode();
        }

        /// <summary>
        /// Must be called immediately after the constructor.
        /// It is separate from the constructor to allow this type to be created by an IoC container.
        /// </summary>
        /// <param name="associatedTo">
        /// The .NET type of the type the main type is related to.
        /// </param>
        /// <param name="numberOfUsages">
        /// The number of times the main type uses the <paramref name="associatedTo"/> type.
        /// </param>
        /// <param name="usageDescriptors">
        /// The collection of descriptors describing how the main type uses the <paramref name="associatedTo"/> type.
        /// </param>
        /// <param name="depth">
        /// The distance back to the main subject of the diagram.
        /// </param>
        /// <returns>
        /// Itself for chaining.
        /// </returns>
        public FieldAssociation Initialise(Type associatedTo, int numberOfUsages, IEnumerable<AssociationUsageDescriptor> usageDescriptors, int depth)
        {
            this.InitialiseCommon(associatedTo, depth);
            if (usageDescriptors == null)
            {
                throw new ArgumentNullResourceException("usageDescriptors", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            this.UsageCount = numberOfUsages;

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

            this.UsageDescriptorList = usageList;

            return this;
        }

        /// <summary>
        /// Proposes a new position given the input parameters.
        /// </summary>
        /// <param name="actualWidth">
        /// The actual width.
        /// </param>
        /// <param name="actualHeight">
        /// The actual height.
        /// </param>
        /// <param name="subjectArea">
        /// The subject area.
        /// </param>
        /// <param name="overlapsWithOthers">
        /// The overlaps with others.
        /// </param>
        /// <returns>
        /// The <see cref="Area"/>. that represents the new position.
        /// </returns>
        /// <exception cref="ArgumentNullResourceException">
        /// Will be thrown if <see cref="subjectArea"/> is null.
        /// </exception>
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

            return this.GetProposedAreaSemiCircle(actualWidth, actualHeight, subjectArea.Centre, overlapsWithOthers);
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
            }
            else if (isTrivial)
            {
                line.Style = "AssociationLineTrivial";
                line.Thickness = thickness;
            }
            else
            {
                line.Style = "AssociationLineStrong";
                line.Thickness = thickness + 1;
            }
        }

        /// <summary>
        /// The create line head.
        /// </summary>
        /// <returns>
        /// The <see cref="ArrowHead"/>.
        /// </returns>
        internal override ArrowHead CreateLineHead()
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

            return new AssociationArrowHead();
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="association">
        /// The association.
        /// </param>
        internal void Merge(FieldAssociation association)
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

            this.UsageCount += association.UsageCount;
            foreach (AssociationUsageDescriptor descriptor in association.UsageDescriptorList)
            {
                this.UsageDescriptorList.Add(descriptor);
            }
        }

        /// <summary>
        /// The style line.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        internal override void StyleLine(ConnectionLine line)
        {
            if (!this.IsInitialised)
            {
                CannotUseWithoutInitializationFirst();
            }

            StyleLineForNonParentAssociation(line, this.UsageCount, this.AssociatedTo, this.IsTrivialAssociation());
        }

        /// <summary>
        /// The initialization common code. This will be called by sub-classes.
        /// </summary>
        /// <param name="associatedTo">
        /// The associated To type.
        /// </param>
        /// <param name="depth">
        /// The distance from the main diagram subject.
        /// </param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "argument name")]
        protected void InitialiseCommon(Type associatedTo, int depth)
        {
            if (associatedTo == null)
            {
                throw new ArgumentNullResourceException("type", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (this.modelBuilder == null)
            {
                throw new ArgumentNullResourceException("modelBuilder", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (associatedTo.IsGenericParameter && associatedTo.GUID == Guid.Empty)
            {
                this.AssociatedTo = this.modelBuilder.BuildVisualisableType(associatedTo.BaseType, depth);
            }
            else
            {
                this.AssociatedTo = this.modelBuilder.BuildVisualisableType(associatedTo, depth);
            }

            this.IsInitialised = true;
        }

        private Area GetProposedAreaSemiCircle(double actualWidth, double actualHeight, Point centre, Func<Area, ProximityTestResult> overlapsWithOthers)
        {
            // A angle of 0 degrees in this context is moving directly to the right
            this.angle = this.dimensions.CalculateNextAvailableAngle();
            Area proposedArea;
            var calc = new CircleCalculator(centre, this.angle);
            double radius = 250;
            ProximityTestResult proximityResult = null;

            do
            {
                if (proximityResult != null && proximityResult.Proximity == Proximity.VeryClose)
                {
                    radius += LayoutConstants.MinimumDistanceBetweenObjects / 2;
                }
                else
                {
                    radius += LayoutConstants.MinimumDistanceBetweenObjects;
                }

                proposedArea = new Area(calc.CalculatePointOnCircle(radius), actualWidth, actualHeight);
                proposedArea = proposedArea.OffsetToMakeTopLeftCentre();
                proximityResult = overlapsWithOthers(proposedArea);
            }
            while (proximityResult.Proximity == Proximity.Overlapping || proximityResult.Proximity == Proximity.VeryClose);

            return proposedArea;
        }
    }
}