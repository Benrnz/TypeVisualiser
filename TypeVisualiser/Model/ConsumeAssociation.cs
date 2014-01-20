namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using TypeVisualiser.Model.Persistence;
    using TypeVisualiser.Properties;

    public class ConsumeAssociation : FieldAssociation
    {
        public ConsumeAssociation(IApplicationResources resources, ITrivialFilter trivialFilter, IModelBuilder modelBuilder, IDiagramDimensions diagramDimensions)
            : base(resources, trivialFilter, modelBuilder, diagramDimensions)
        {
        }

        public override string Name
        {
            get
            {
                int count = this.UsageDescriptorList.Count();
                if (count > 2)
                {
                    return string.Format(CultureInfo.CurrentCulture, "{0} methods use the {1} {2} (click for more details)", count, this.AssociatedTo.Name, this.AssociatedTo.Modifiers.TypeTextName);
                }

                return string.Format(
                    CultureInfo.CurrentCulture, "1 method uses the {0} {1} ({2})", this.AssociatedTo.Name, this.AssociatedTo.Modifiers.TypeTextName, this.UsageDescriptorList.FirstOrDefault());
            }
        }

        internal override Type PersistenceType
        {
            get
            {
                return typeof(ConsumeAssociationData);
            }
        }

        /// <summary>
        /// Must be called immediately after the constructor.
        /// It is separate from the constructor to allow this type to be created by an IoC container.
        /// </summary>
        /// <param name="associatedTo">
        /// The type this association is point to.
        /// </param>
        /// <param name="numberOfUsages">
        /// The number of times the type is used.
        /// </param>
        /// <param name="usageDescriptors">
        /// The descriptors that describe how the type is used.
        /// </param>
        /// <param name="depth">
        /// The distance from the main diagram subject.
        /// </param>
        /// <returns>
        /// Itself for chaining.
        /// </returns>
        public new ConsumeAssociation Initialise(Type associatedTo, int numberOfUsages, IEnumerable<AssociationUsageDescriptor> usageDescriptors, int depth)
        {
            this.InitialiseCommon(associatedTo, depth);
            if (usageDescriptors == null)
            {
                throw new ArgumentNullResourceException("usageDescriptors", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            this.UsageCount = numberOfUsages;
            this.UsageDescriptorList = new List<AssociationUsageDescriptor>(usageDescriptors);

            return this;
        }

        internal override void StyleLine(ConnectionLine line)
        {
            if (line == null)
            {
                throw new ArgumentNullResourceException("line", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            double thickness = 1.25 + (this.UsageCount * 0.75);
            if (thickness > 15)
            {
                thickness = 15;
            }

            line.Thickness = thickness;
            line.Style = this.IsTrivialAssociation() ? "ConsumptionLineTrivial" : "ConsumptionLineStrong";
        }
    }
}