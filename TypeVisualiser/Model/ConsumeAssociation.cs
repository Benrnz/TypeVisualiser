namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Persistence;
    using Properties;

    public class ConsumeAssociation : FieldAssociation
    {
        public ConsumeAssociation(Type type, int numberOfUsages, IEnumerable<AssociationUsageDescriptor> usageDescriptors, int depth)
            : base(type, depth)
        {
            if (type == null)
            {
                throw new ArgumentNullResourceException("type", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (usageDescriptors == null)
            {
                throw new ArgumentNullResourceException("usageDescriptors", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            UsageCount = numberOfUsages;
            UsageDescriptorList = new List<AssociationUsageDescriptor>(usageDescriptors);
        }

        public override string Name
        {
            get
            {
                int count = UsageDescriptorList.Count();
                if (count > 2)
                {
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        "{0} methods use the {1} {2} (click for more details)",
                        count,
                        AssociatedTo.Name,
                        AssociatedTo.Modifiers.TypeTextName);
                }

                return string.Format(
                    CultureInfo.CurrentCulture,
                    "1 method uses the {0} {1} ({2})",
                    AssociatedTo.Name,
                    AssociatedTo.Modifiers.TypeTextName,
                    UsageDescriptorList.FirstOrDefault());
            }
        }

        internal override Type PersistenceType
        {
            get
            {
                return typeof(ConsumeAssociationData);
            }
        }

        internal override void StyleLine(ConnectionLine line)
        {
            if (line == null)
            {
                throw new ArgumentNullResourceException("line", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            double thickness = 1.25 + (UsageCount * 0.75);
            if (thickness > 15)
            {
                thickness = 15;
            }

            line.Thickness = thickness;
            if (IsTrivialAssociation())
            {
                line.Style = "ConsumptionLineTrivial";
            } else
            {
                line.Style = "ConsumptionLineStrong";
            }
        }
    }
}